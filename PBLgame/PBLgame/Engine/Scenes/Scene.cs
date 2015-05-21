using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using PBLgame.Engine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.Components;
using PBLgame.Engine.Singleton;
using PBLgame.Engine.Physics;

namespace PBLgame.Engine.Scenes
{
    public class Scene : IXmlSerializable
    {
        #region Variables
        #region Private
        //It's gonna be scene graph later
        private List<GameObject> _gameObjects;
        private List<int> _takenIdNumbers;
        private List<Light> _sceneLights;
        private Camera _mainCamera;

        //!!!! NEW STUFF 
        private PhysicsSystem _physicsSystem;

        private readonly XmlSerializer _serializer;
        private GraphicsDevice _graphics;
        
        // Shadows
        private const int ALL_LIGHTS = 8;
        private const int DIR_LIGHTS = 3;
        private const int POINT_LIGHTS = ALL_LIGHTS - DIR_LIGHTS;
        private bool[] _hasShadows = new bool[ALL_LIGHTS];
        private int _shadowMapSize = 1024;
        private float[] _shadowFarPlanes = new float[ALL_LIGHTS];
        private RenderTarget2D[] _shadowDirDepthTargets = new RenderTarget2D[DIR_LIGHTS];
        private RenderTargetCube[] _shadowPointDepthTargets = new RenderTargetCube[POINT_LIGHTS];

        private readonly CubeMapFace[] _faces =
        {
            CubeMapFace.PositiveX,
            CubeMapFace.NegativeX,
            CubeMapFace.PositiveY,
            CubeMapFace.NegativeY,
            CubeMapFace.PositiveZ,
            CubeMapFace.NegativeZ
        };

        #endregion
        #endregion

        #region Properties

        public Camera MainCamera
        {
            get { return _mainCamera; }
            set { _mainCamera = value; }
        }

        public List<GameObject> GameObjects
        {
            get { return _gameObjects; }
            private set { _gameObjects = value; }
        }

        public List<Light> SceneLights
        {
            get
            {
                return _sceneLights;
            }
            set
            {
                _sceneLights = value;
            }
        }

        #endregion

        #region Methods

        public Scene()
        {
            _gameObjects = new List<GameObject>();
            _sceneLights = new List<Light>();
            _serializer = new XmlSerializer(typeof(Scene));
            _takenIdNumbers = new List<int> { 0 };
            _physicsSystem = new PhysicsSystem();
            _graphics = GlobalInventory.Instance.GraphicsDevice;

            for (int dir = 0; dir < DIR_LIGHTS; dir++)
            {
                _shadowDirDepthTargets[dir] = new RenderTarget2D(_graphics, _shadowMapSize, _shadowMapSize, false, SurfaceFormat.Single, DepthFormat.Depth24);
            }

            for (int pt = 0; pt < ALL_LIGHTS - DIR_LIGHTS; pt++)
            {
                _shadowPointDepthTargets[pt] = new RenderTargetCube(_graphics, _shadowMapSize, false, SurfaceFormat.Single, DepthFormat.Depth24);
            }
            
        }

        public void Draw(GameTime gameTime)
        {
            RasterizerState oldRasterizer = _graphics.RasterizerState;
            _graphics.RasterizerState = RasterizerState.CullNone;

            ParameterizeEffectsWithLightsAndShadows(gameTime, ResourceManager.Instance.ShaderEffects.Where(effect => effect.Name.Contains("BasicShader")));

            _graphics.SetRenderTarget(null);
            _graphics.RasterizerState = oldRasterizer;

            //foreach (Effect effect in ResourceManager.Instance.ShaderEffects.Where(effect => effect.Name.Contains("BasicShader")))
            //{
            //    effect.Parameters["shadowMap3"].SetValue(_shadowPointDepthTargets[0]);
            //}
            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.Draw(gameTime);
            }
        }

        private void CreatePointLightMatrices(Vector3 pos, Matrix[] matrices)
        {
            matrices[0] = Matrix.CreateLookAt(pos, pos - Vector3.UnitX, - Vector3.UnitY);
            matrices[1] = Matrix.CreateLookAt(pos, pos + Vector3.UnitX, - Vector3.UnitY);
            matrices[2] = Matrix.CreateLookAt(pos, pos - Vector3.UnitY,   Vector3.UnitZ);   // up
            matrices[3] = Matrix.CreateLookAt(pos, pos + Vector3.UnitY, - Vector3.UnitZ);   // down
            matrices[4] = Matrix.CreateLookAt(pos, pos - Vector3.UnitZ, - Vector3.UnitY);
            matrices[5] = Matrix.CreateLookAt(pos, pos + Vector3.UnitZ, - Vector3.UnitY);
        }


        private void ParameterizeEffectsWithLightsAndShadows(GameTime gameTime, IEnumerable<Effect> effects)
        {
            const int FIRST_POINT_INDEX = DIR_LIGHTS;
            Vector3[] pos_dir = new Vector3[ALL_LIGHTS];
            Vector4[] colors = new Vector4[ALL_LIGHTS];
            float[] att_int = new float[ALL_LIGHTS];
            float[] falloff = new float[ALL_LIGHTS];
            
            int dir = 0;
            int pt = FIRST_POINT_INDEX;    // start after last directional

            effects = effects as Effect[] ?? effects.ToArray();

            foreach (Light light in SceneLights)
            {
                if (light.Type == LightType.Directional && dir < DIR_LIGHTS)
                {
                    MyDirectionalLight dLight = light as MyDirectionalLight;
                    pos_dir[dir] = dLight.Direction;
                    colors[dir]  = dLight.Color * dLight.Color.W;   // use alpha as a multiplier
                    att_int[dir] = dLight.Intensity;
                    dir++;
                }
                else if (pt < ALL_LIGHTS)
                {
                    PointLight pLight = light as PointLight;
                    pos_dir[pt] = pLight.Position;
                    colors[pt]  = pLight.Color * pLight.Color.W;
                    att_int[pt] = pLight.Attenuation;
                    falloff[pt] = pLight.FallOff;

                    _shadowFarPlanes[pt] = pLight.Attenuation;

                    if (light.HasShadow)
                    {
                        _hasShadows[pt] = true;
                        Matrix lightProjMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1, 1f, _shadowFarPlanes[pt]);
                        Matrix[] lightViewMatrices = new Matrix[6];
                        CreatePointLightMatrices(pLight.Position, lightViewMatrices);

                        for (int face = 0; face < _faces.Length; face++)
                        {
                            _graphics.SetRenderTarget(_shadowPointDepthTargets[pt - FIRST_POINT_INDEX], _faces[face]);
                            _graphics.Clear(Color.White);


                            foreach (Effect effect in effects)
                            {
                                effect.Parameters["shadowFarPlane"].SetValue(_shadowFarPlanes[pt]);
                                effect.Parameters["shadowLightPos"].SetValue(pLight.Position);
                                effect.Parameters["view"].SetValue(lightViewMatrices[face]);
                                effect.Parameters["projection"].SetValue(lightProjMatrix);
                            }

                            foreach (GameObject gameObject in GameObjects)
                            {
                                gameObject.DrawSpecial(gameTime, Renderer.Technique.SHADOWS);
                            }
                        }

                        _graphics.SetRenderTarget(null);

                        foreach (Effect effect in effects)
                        {
                            effect.Parameters[string.Format("shadowMap{0}", pt)].SetValue(_shadowPointDepthTargets[pt - FIRST_POINT_INDEX]);
                        }
                    }
                    else
                    {
                        _hasShadows[pt] = false;
                    }
                    pt++;
                }
            }

            foreach (Effect effect in effects)
            {
                //!!!!!! lightsCount have to be less or equal ALL_LIGHTS [;
                effect.Parameters["lightsPositions"].SetValue(pos_dir);
                effect.Parameters["lightsColors"].SetValue(colors);
                effect.Parameters["lightsAttenuations"].SetValue(att_int);
                effect.Parameters["lightsFalloffs"].SetValue(falloff);
                effect.Parameters["shadowFarPlanes"].SetValue(_shadowFarPlanes);
                effect.Parameters["hasShadows"].SetValue(_hasShadows);
            }
        }


        public void Update(GameTime gameTime)
        {
            //if(FindGameObject(1).collision != null) Console.WriteLine(FindGameObject(1).collision.BoxColliders[0]._edgesRealSize.ToString());


            _physicsSystem.Update(GetAllObjectsWithCollider());
            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.Update(gameTime);
            }
            Camera.MainCamera.Update(gameTime);

        }

        public GameObject FindGameObject(int id)
        {
            return _gameObjects.FirstOrDefault(go => go.ID == id);
        }

        public GameObject FindGameObject(string name)
        {
            return _gameObjects.FirstOrDefault(go => go.Name == name);
        }

        public List<GameObject> FindGameObjectsWithTag(string tag)
        {
            return _gameObjects.Where(go => go.Tag == tag).ToList();
        }

        public void AddGameObject(GameObject obj)
        {
            while (_takenIdNumbers.Exists(item => item == obj.ID))
            {
                obj.ID += 1;
            }
            _takenIdNumbers.Add(obj.ID);

            if (obj is Light) _sceneLights.Add((Light)obj);
            else _gameObjects.Add(obj);
        }

        public void AddGameObjectWithDescendants(GameObject obj)
        {
            AddGameObject(obj);
            foreach (GameObject child in obj.GetChildren())
            {
                AddGameObjectWithDescendants(child);
            }
        }

        public void AddGameObjectAfter(GameObject obj, GameObject predecessor)
        {
            if (obj is Light && predecessor != null && !(predecessor is Light)) return;

            while (_takenIdNumbers.Exists(item => item == obj.ID))
            {
                obj.ID += 1;
            }
            _takenIdNumbers.Add(obj.ID);

            int index = 0;
            if (predecessor != null)
            {
                if (predecessor is Light)
                {
                    index = _sceneLights.IndexOf((Light) predecessor);
                }
                else
                {
                    index = _gameObjects.IndexOf(predecessor);
                }
            }

            if (obj is Light) _sceneLights.AddInsert(index, (Light)obj);
            else _gameObjects.AddInsert(index, obj);
        }

        public void RemoveGameObject(GameObject obj)
        {
            _takenIdNumbers.Remove(obj.ID);

            if (obj is Light) _sceneLights.Remove((Light)obj);
            else _gameObjects.Remove(obj);
        }

        public void RemoveGameObjectWithDescendants(GameObject obj)
        {
            RemoveGameObject(obj);
            foreach (GameObject child in obj.GetChildren())
            {
                RemoveGameObjectWithDescendants(child);
            }
        }

        public void RemoveGameObject(string name)
        {
            List<GameObject> gameObjects = GameObjects.FindAll(item => item.Name == name);
            foreach (GameObject gameObject in gameObjects)
            {
                _takenIdNumbers.Remove(gameObject.ID);
                _gameObjects.Remove(gameObject);
            }
        }

        public List<GameObject> GetAllObjectsWithCollider()
        {
            List<GameObject> tmp = new List<GameObject>();
            foreach(GameObject go in _gameObjects)
            {
                if (go.collision != null) tmp.Add(go);
            }
            return tmp;
        }

        public void AddLight(Light obj)
        {
            _sceneLights.Add(obj);
        }

        public void RemoveLight(Light obj)
        {
            _sceneLights.Remove(obj);
        }

        public void RemoveGameObject(int id)
        {
            _takenIdNumbers.RemoveAll(item => item == id);
            GameObjects.RemoveAll(item => item.ID == id);
        }

        /// <summary>
        /// Gets previous GameObject (or Light) on its scene list.
        /// </summary>
        /// <returns>previous object on scene lists</returns>
        public GameObject GetPreceding(GameObject obj)
        {
            if (obj is Light)
            {
                int index = _sceneLights.IndexOf((Light) obj);
                if (index == 0) return null;
                return _sceneLights[index - 1];
            }
            else
            {
                int index = _gameObjects.IndexOf(obj);
                if (index == 0) return null;
                return _gameObjects[index - 1];
            }
        }

        public void Save(string path)
        {
            using (FileStream writer = new FileStream(path, FileMode.Create))
            {
                _serializer.Serialize(writer, this);
            }
        }

        public void Load(string path)
        {
            using (FileStream file = new FileStream(path, FileMode.Open))
            {
                Scene scene = (Scene)_serializer.Deserialize(new SceneXmlReader(file, this));
                GameObjects = scene._gameObjects;
                SceneLights = scene._sceneLights;
            }
            //finding parents for gameobjects
            foreach (GameObject gameObject in GameObjects)
            {
                if (gameObject.parent != null)
                {
                    gameObject.parent = GameObjects.Find(parent => parent.ID == gameObject.parent.ID);
                    gameObject.parent.AddChild(gameObject);
                }

                //checking if id in object is unique
                while (_takenIdNumbers.Exists(item => item == gameObject.ID))
                {
                    gameObject.ID += 1;
                }

                //setting unique Id list
                _takenIdNumbers.Add(gameObject.ID);
            }

            //finding parents for lights
            foreach (Light light in SceneLights)
            {
                if (light.parent != null)
                {
                    light.parent = GameObjects.Find(parent => parent.ID == light.parent.ID);
                    light.parent.AddChild(light);
                }

                //checking if id in object is unique
                while (_takenIdNumbers.Exists(item => item == light.ID))
                {
                    light.ID += 1;
                }

                //setting unique Id list
                _takenIdNumbers.Add(light.ID);
            }

            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.Initialize();
            }
        }

        #region XML Serialization

        public class SceneXmlReader : XmlTextReader
        {
            public readonly Scene Scene;

            public SceneXmlReader(Stream stream, Scene scene)
                : base(stream)
            {
                Scene = scene;
                WhitespaceHandling = WhitespaceHandling.Significant;
                Normalization = true;
                XmlResolver = null;
            }
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            reader.ReadStartElement();
            reader.MoveToContent();
            reader.ReadStartElement();
            reader.MoveToContent();
            while (reader.Name != "Scene")
            {
                if (reader.Name == "GameObject")
                {
                    GameObject obj = new GameObject();
                    (obj as IXmlSerializable).ReadXml(reader);
                    GameObjects.Add(obj);
                }
                if (reader.Name == "Light")
                {
                    string type = reader.GetAttribute("Type");
                    Light l = null;
                    switch (type)
                    {
                        case "Directional":
                            l = new MyDirectionalLight();
                            break;
                        case "Point":
                            l = new PointLight();
                            break;
                    }

                    (l as IXmlSerializable).ReadXml(reader);
                    SceneLights.Add(l);
                }
                reader.Read();
            }

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("GameObjects");
            foreach (GameObject sceneGameObject in GameObjects)
            {
                writer.WriteStartElement("GameObject");
                (sceneGameObject as IXmlSerializable).WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteStartElement("Lights");
            foreach (Light sceneLight in SceneLights)
            {
                writer.WriteStartElement("Light");
                (sceneLight as IXmlSerializable).WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        #endregion
        #endregion


    }
}