using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using PBLgame.Engine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.Components;
using PBLgame.Engine.Singleton;
using PBLgame.Engine.Physics;
using PBLgame.Engine.AI;
using PBLgame.GamePlay;

namespace PBLgame.Engine.Scenes
{
    public class Scene : BaseScene, IXmlSerializable
    {
        #region Variables
        #region Private
        //It's gonna be scene graph later
        private List<GameObject> _gameObjects;
        private List<int> _takenIdNumbers;
        private List<Light> _sceneLights;
        private GameObject _mirror;
        //private Camera _mainCamera;

        //!!!! NEW STUFF 
        private PhysicsSystem _physicsSystem;

        private readonly XmlSerializer _serializer;
        private GraphicsDevice _graphics;
        
        // Shadows
        private const int ALL_LIGHTS = 6;
        private const int DIR_LIGHTS = 2;
        private const int POINT_LIGHTS = ALL_LIGHTS - DIR_LIGHTS;
        private bool[] _hasShadows = new bool[ALL_LIGHTS];
        private int _shadowMapSize = 1024;
        private float[] _shadowFarPlanes = new float[ALL_LIGHTS];
        private RenderTarget2D[] _shadowDirDepthTargets = new RenderTarget2D[DIR_LIGHTS];
        private RenderTargetCube[] _shadowPointDepthTargets = new RenderTargetCube[POINT_LIGHTS];
        private Matrix[] _shadowProjMatrices = new Matrix[DIR_LIGHTS];
        private Matrix[] _shadowViewMatrices = new Matrix[DIR_LIGHTS];

        private readonly CubeMapFace[] _faces =
        {
            CubeMapFace.PositiveX,
            CubeMapFace.NegativeX,
            CubeMapFace.PositiveY,
            CubeMapFace.NegativeY,
            CubeMapFace.PositiveZ,
            CubeMapFace.NegativeZ
        };

        private Plane _groundPlane = new Plane(Vector3.Up, 0);

        private const float DIR_SHADOW_FAR = 200f;      // far plane
        private const float DIR_SHADOW_SIZE = 200f;     // projection matrix dimension

        private RenderTarget2D _reflectionTarget;
        private RenderTarget2D _glowTarget;
        private GaussianBlur _gaussianBlur;
        
        private bool _editor;
        private CullingManager _cullingManager;
        //private List<GameObject> _dynamicObjects;
        //private List<GameObject> _staticObjects;
        private SpriteBatch _spriteBatch;
        private RenderTarget2D _halfRenderTarget;

        private event Action QueueAfterUpdate;
        #endregion
        #endregion

        #region Properties

        //public Camera MainCamera
        //{
        //    get { return _mainCamera; }
        //    set { _mainCamera = value; }
        //}

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

        public Scene(bool editor)
        {
            _editor = editor;
            _gameObjects = new List<GameObject>();
            _sceneLights = new List<Light>();
            _serializer = new XmlSerializer(typeof(Scene));
            _takenIdNumbers = new List<int> { 0 };
            _physicsSystem = new PhysicsSystem();
            _graphics = GlobalInventory.Instance.GraphicsDevice;
        }

        public Scene() : this(false)
        {
            // ctor for stupid serializer which cannot into default parameters
        }

        public override void Initialize()
        {
            for (int i = 0; i < DIR_LIGHTS; i++)
            {
                _shadowViewMatrices[i] = Matrix.Identity;
                _shadowProjMatrices[i] = Matrix.Identity;
            }

            for (int dir = 0; dir < DIR_LIGHTS; dir++)
            {
                _shadowDirDepthTargets[dir] = new RenderTarget2D(_graphics, _shadowMapSize, _shadowMapSize, false, SurfaceFormat.Single, DepthFormat.Depth24);
            }

            for (int pt = 0; pt < POINT_LIGHTS; pt++)
            {
                _shadowPointDepthTargets[pt] = new RenderTargetCube(_graphics, _shadowMapSize, false, SurfaceFormat.Single, DepthFormat.Depth24);
            }

            _reflectionTarget = new RenderTarget2D(_graphics, _graphics.Viewport.Width, _graphics.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            _glowTarget = new RenderTarget2D(_graphics, _graphics.Viewport.Width, _graphics.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);

            _gaussianBlur = new GaussianBlur(
                ResourceManager.Instance.ShaderEffects.First(e => e.Name == @"Effects\GaussianBlur"),
                3f,
                _glowTarget.Width,
                _glowTarget.Height
            );

            _halfRenderTarget = new RenderTarget2D(_graphics, _glowTarget.Width, _glowTarget.Height, false,
                _graphics.PresentationParameters.BackBufferFormat, DepthFormat.None);

            _spriteBatch = new SpriteBatch(_graphics);
        }

        public override void Draw(GameTime gameTime)
        {
            IEnumerable<Effect> effects_ = ResourceManager.Instance.ShaderEffects.Where(effect => effect.Name.Contains("BasicShader"));
            Effect[] effects = effects_ as Effect[] ?? effects_.ToArray();

            //RasterizerState oldRasterizer = _graphics.RasterizerState;
            //_graphics.RasterizerState = RasterizerState.CullNone;   // [possible slowdown] more realistic shadows
            
            ParameterizeEffectsWithLightsAndShadows(gameTime, effects);
            ParameterizeEffectsWithGlow(gameTime, effects);
            
            _graphics.SetRenderTarget(null);
            //_graphics.RasterizerState = oldRasterizer;
            
            if (_mirror != null)
            {
                _graphics.SetRenderTarget(_reflectionTarget);
                Camera cam = Camera.MainCamera;
                // mirror normal is hard-coded as +Y
                // TODO could be extracted from mesh :D
                Vector3 mirrorNormal = Vector3.UnitY;
                mirrorNormal = Vector3.TransformNormal(mirrorNormal, _mirror.transform.WorldRotation * _mirror.transform.AncestorsRotation);

                Vector3 mirrorPosition = _mirror.transform.WorldPosition;
                float d = -Vector3.Dot(mirrorNormal, mirrorPosition);
                Plane reflectionPlane = new Plane(mirrorNormal, d);
                reflectionPlane.Normalize();
                Vector3 camPos = cam.transform.Position;
                // if Normal ABC is normalized, then dist = |Ax + By + Cz + D|
                float camDistToPlane = Math.Abs(Vector3.Dot(reflectionPlane.Normal, camPos) + reflectionPlane.D);
                Vector3 reflectedCamPos = camPos - (2 * camDistToPlane * reflectionPlane.Normal);
                Vector3 reflectedCamDir = Vector3.Reflect(cam.Direction, reflectionPlane.Normal);
                Vector3 reflectedCamUp = Vector3.Reflect(Vector3.Up, reflectionPlane.Normal);
                Matrix reflectedView = Matrix.CreateLookAt(reflectedCamPos, reflectedCamPos + reflectedCamDir, reflectedCamUp);
                Matrix reflectedProjection = Matrix.CreatePerspectiveFieldOfView(cam.FoV, cam.Aspect, cam.Near + camDistToPlane, cam.Far);

                foreach (Effect effect in effects)
                {
                    effect.Parameters["view"].SetValue(reflectedView);
                    effect.Parameters["reflectedView"].SetValue(reflectedView);
                    effect.Parameters["projection"].SetValue(reflectedProjection);
                    effect.Parameters["cameraPosition"].SetValue(reflectedCamPos);
                    effect.Parameters["clipPlane"].SetValue(new Vector4(reflectionPlane.Normal, reflectionPlane.D));
                }
                DrawScene(gameTime, Renderer.Technique.CustomCamera, reflectedView * reflectedProjection);

                _graphics.SetRenderTarget(null);

                foreach (Effect effect in effects)
                {
                    effect.Parameters["clipPlane"].SetValue(new Vector4(0));
                }
            }

            DrawScene(gameTime);

            if (_mirror != null)
            {
                foreach (Effect effect in effects)
                {
                    effect.Parameters["reflectionMap"].SetValue(_reflectionTarget);
                    effect.Parameters["alphaValue"].SetValue(_mirror.renderer.AlphaValue);
                }

                //SetupEffectsCamera();
                _mirror.DrawSpecial(gameTime, Renderer.Technique.Reflection);
            }
        }

        /// <summary>
        /// Draws whole scene with main camera and given or default technique.
        /// </summary>
        /// <param name="gameTime">current time, nothing really uses in draw</param>
        /// <param name="technique">technique of shader</param>
        /// <param name="cameraMatrix">camera matrix: view * projection</param>
        private void DrawScene(GameTime gameTime, Renderer.Technique technique = Renderer.Technique.Default, Matrix cameraMatrix = default(Matrix))
        {
            if (cameraMatrix == default(Matrix)) 
            {
                SetupEffectsCamera();
                cameraMatrix = Camera.MainCamera.ViewMatrix*Camera.MainCamera.ProjectionMatrix;
            }

            List<GameObject> toRender = _cullingManager.GetVisibleGameObjects(cameraMatrix);
            //toRender.Remove(_mirror);


            if (technique == Renderer.Technique.Default)
            {
                foreach (GameObject gameObject in toRender)
                {
                    gameObject.Draw(gameTime);
                }
            }
            else
            {
                foreach (GameObject gameObject in toRender)
                {
                    gameObject.DrawSpecial(gameTime, technique);
                }
            }
            
            if (technique != Renderer.Technique.Default) return;
            foreach (GameObject gameObject in GameObjects)
            {
                if (gameObject.particleSystem != null) gameObject.particleSystem.Draw(gameTime);
            }

            _graphics.DepthStencilState = DepthStencilState.Default;
        }

        private void SetupEffectsCamera()
        {
            foreach (Effect effect in ResourceManager.Instance.ShaderEffects.Where(effect => effect.Name.Contains("BasicShader")))
            {
                effect.Parameters["view"].SetValue(Camera.MainCamera.ViewMatrix);
                effect.Parameters["projection"].SetValue(Camera.MainCamera.ProjectionMatrix);
                effect.Parameters["cameraPosition"].SetValue(Camera.MainCamera.transform.Position);
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
                Vector4 color = light.Color * light.Color.W;    // use alpha as a multiplier
                if (light.Type == LightType.Directional && dir < DIR_LIGHTS)
                {
                    if (!light.UseLight)
                    {
                        continue;
                    }
                    MyDirectionalLight dLight = light as MyDirectionalLight;
                    pos_dir[dir] = dLight.Direction;
                    colors [dir] = color;
                    att_int[dir] = dLight.Intensity;

                    _shadowFarPlanes[dir] = DIR_SHADOW_FAR; // close enough hard-coded but should be computed by unprojecting camera view and blah blah

                    if (light.HasShadow)
                    {
                        _hasShadows[dir] = true;
                        _shadowProjMatrices[dir] = Matrix.CreateOrthographic(DIR_SHADOW_SIZE, DIR_SHADOW_SIZE, 1f, DIR_SHADOW_FAR);
                        Ray cameraRay = new Ray(Camera.MainCamera.transform.Position, Camera.MainCamera.Direction);
                        float? distToGround = cameraRay.Intersects(_groundPlane);
                        if (distToGround.HasValue)
                        {
                            Vector3 cameraTarget = cameraRay.Position + cameraRay.Direction * distToGround.Value;
                            Vector3 lightImplicitPos = cameraTarget + Vector3.Normalize(dLight.Direction) * (DIR_SHADOW_FAR / 2f); // still better than a constant dependent-from-nothing gravity
                            _shadowViewMatrices[dir] = Matrix.CreateLookAt(lightImplicitPos, cameraTarget, Vector3.Up);

                            _graphics.SetRenderTarget(_shadowDirDepthTargets[dir]);
                            _graphics.Clear(Color.White);
                            
                            foreach (Effect effect in effects)
                            {
                                //effect.Parameters["world"].SetValue(Matrix.CreateTranslation(lightImplicitPos));
                                effect.Parameters["shadowFarPlane"].SetValue(_shadowFarPlanes[dir]);
                                effect.Parameters["view"].SetValue(_shadowViewMatrices[dir]);
                                effect.Parameters["projection"].SetValue(_shadowProjMatrices[dir]);
                            }

                            DrawScene(gameTime, Renderer.Technique.ShadowsDirectional, _shadowViewMatrices[dir] * _shadowProjMatrices[dir]);

                            _graphics.SetRenderTarget(null);
                            
                            foreach (Effect effect in effects)
                            {
                                effect.Parameters[string.Format("shadowMap{0}", dir)].SetValue(_shadowDirDepthTargets[dir]);
                            }

                        }
                        else
                        {
                            // camera not looking at the ground - screw this, no shadows
                            _hasShadows[dir] = false;
                        }
                    }
                    else
                    {
                        _hasShadows[dir] = false;
                    }

                    dir++;
                }
                else if (pt < ALL_LIGHTS)
                {
                    if (!light.UseLight)
                    {
                        continue;
                    }
                    PointLight pLight = light as PointLight;
                    pos_dir[pt] = pLight.Position;
                    colors [pt] = color;
                    att_int[pt] = pLight.Attenuation;
                    falloff[pt] = pLight.FallOff;

                    _shadowFarPlanes[pt] = pLight.Attenuation;

                    if (light.HasShadow && _shadowFarPlanes[pt] > 0)
                    {
                        _hasShadows[pt] = true;
                        Matrix lightProjMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1, 10f, _shadowFarPlanes[pt]);
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

                            DrawScene(gameTime, Renderer.Technique.ShadowsPoint, lightViewMatrices[face] * lightProjMatrix);
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
                effect.Parameters["shadowViewMatrices"].SetValue(_shadowViewMatrices);
                effect.Parameters["shadowProjMatrices"].SetValue(_shadowProjMatrices);
            }
        }

        private void ParameterizeEffectsWithGlow(GameTime gameTime, IEnumerable<Effect> effects)
        {
            _graphics.SetRenderTarget(_glowTarget);
            DrawScene(gameTime, Renderer.Technique.Glow);
            _gaussianBlur.Perform(_glowTarget, _graphics, _spriteBatch, _halfRenderTarget, _glowTarget);
            _graphics.SetRenderTarget(null);

            foreach (Effect effect in effects)
            {
                effect.Parameters["glowMap"].SetValue(_glowTarget);
            }

            _graphics.DepthStencilState = DepthStencilState.Default;

            //_spriteBatch.Begin();
            //_spriteBatch.Draw(_glowTarget, new Rectangle(0, 0, _glowTarget.Width, _glowTarget.Height), Color.White);
            //_spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            AISystem.ExecuteAI();
            _physicsSystem.Update(gameTime);
            foreach (GameObject gameObject in _cullingManager.GameObjectsForUpdate(Camera.MainCamera.ViewMatrix * Camera.WideProjection))
            {
                gameObject.Update(gameTime);
            }
            Camera.MainCamera.Update(gameTime);

            // needed when objects adds to GameObjects list
            if (QueueAfterUpdate != null)
            {
                QueueAfterUpdate();
                QueueAfterUpdate = null;
            }
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

            _cullingManager.Add(obj);
        }

        /// <summary>
        /// Adds temporary non-serializable game object to the scene.
        /// This is postponed after update of the scene is completed.
        /// </summary>
        /// <param name="obj">game object to add</param>
        public void AddTemporary(GameObject obj)
        {
            QueueAfterUpdate += delegate
            {
                obj.Temporary = true;
                obj.ID = (1 << 24);
                AddGameObject(obj);
                obj.Initialize(_editor);
            };
        }

        /// <summary>
        /// Removes temporary game objects.
        /// </summary>
        /// <param name="obj">game object to remove</param>
        /// <param name="additionalAction">action to make after remove</param>
        /// <returns>true if will be removed</returns>
        public bool RemoveTemporary(GameObject obj, Action additionalAction = null)
        {
            if (!obj.Temporary) return false;
            QueueAfterUpdate += delegate
            {
                RemoveGameObject(obj);
            };
            if(additionalAction != null) QueueAfterUpdate += additionalAction;
            return true;
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
            
            // Needed for editor
            _cullingManager.Add(obj);
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
            _cullingManager.Remove(id);
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
            using (MemoryStream memory = new MemoryStream())
            {
                _serializer.Serialize(memory, this);
                using (FileStream file = new FileStream(path, FileMode.Create))
                {
                    memory.WriteTo(file);
                }
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
                ///////////////////////////////////////////////

                ////////////////////////////////////////////////
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

            Initialize();

            // TODO better
            _mirror = FindGameObject("Mirror");

            //Hard coded setting Ace as target xD
            AISystem.SetPlayer(FindGameObject(8));

            _cullingManager = new CullingManager(GameObjects, 4);

            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.Initialize(_editor);
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
            Scene scene = ((SceneXmlReader) reader).Scene;
            reader.MoveToContent();
            reader.ReadStartElement();
            reader.MoveToContent();
            reader.ReadStartElement();
            reader.MoveToContent();
            while (reader.Name != "Scene")
            {
                if (reader.Name == "GameObject")
                {
                    GameObject obj = new GameObject(scene);
                    obj.ReadXml(reader);
                    GameObjects.Add(obj);
                }
                if (reader.Name == "Light")
                {
                    string type = reader.GetAttribute("Type");
                    Light l = null;
                    switch (type)
                    {
                        case "Directional":
                            l = new MyDirectionalLight(scene);
                            break;
                        case "Point":
                            l = new PointLight(scene);
                            break;
                    }

                    l.ReadXml(reader);
                    SceneLights.Add(l);
                }
                reader.Read();
            }

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("GameObjects");
            foreach (GameObject sceneGameObject in GameObjects.Where(o => !o.Temporary))
            {
                writer.WriteStartElement("GameObject");
                (sceneGameObject as IXmlSerializable).WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteStartElement("Lights");
            foreach (Light sceneLight in SceneLights.Where(o => !o.Temporary))
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


    /// <summary>
    /// Empty scene that does nothing.
    /// </summary>
    public class BaseScene
    {
        public virtual void Initialize() { }
        public virtual void Draw(GameTime gameTime) { }
        public virtual void Update(GameTime gameTime) { }
    }

}