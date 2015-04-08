using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using PBLgame.Engine.GameObjects;
using Microsoft.Xna.Framework;

namespace PBLgame.Engine.Scenes
{
    public class Scene : IXmlSerializable
    {
        #region Variables
        #region Private
        //It's gonna be scene graph later
        private List<GameObject> _gameObjects;
        private List<int> _takenIdNumbers;
        private static List<Light> _sceneLights;
        private Camera _mainCamera;

        private readonly XmlSerializer _serializer;

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

        public static List<Light> SceneLights
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
            _takenIdNumbers = new List<int> {0};

            CreateLights();

        }

        public void Draw()
        {
            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.Draw();
            }
        }

        public void Update()
        {
            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.Update();
            }
        }
        
        public void AddGameObject(GameObject obj)
        {
            GameObjects.Add(obj);
        }

        public void RemoveGameObject(GameObject obj)
        {
            _takenIdNumbers.Remove(obj.ID);
            GameObjects.Remove(obj);
        }

        public void RemoveGameObject(string name)
        {
            List<GameObject> gameObjects = GameObjects.FindAll(item => item.Name == name);
            foreach (GameObject gameObject in gameObjects)
            {
                _takenIdNumbers.Remove(gameObject.ID);
                GameObjects.Remove(gameObject);
            }
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
                Scene scene = (Scene) _serializer.Deserialize(file);
                GameObjects = scene._gameObjects;
            }
            //finding parents
            foreach (GameObject gameObject in GameObjects)
            {
                if (gameObject.parent != null)
                {
                    gameObject.parent = GameObjects.Find(parent => parent.ID == gameObject.parent.ID);
                    gameObject.parent.AddChild(gameObject);
                }

                //setting unique Id list
                _takenIdNumbers.Add(gameObject.ID);
            }
        }

        //TMP
        private void CreateLights()
        {
            PointLight l1 = new PointLight();
            l1.Type = LightType.point;
            l1.Attenuation = 10.0f;
            l1.Position = new Vector3(-5, -5, -5);
            l1.Color = new Vector4(1, 1, 1, 1);
            l1.FallOff = 2.0f;

            MyDirectionalLight l2 = new MyDirectionalLight();
            l2.Type = LightType.directional;
            l2.Intensity = 0.2f;
            l2.Direction = new Vector3(-1, -1, 0);
            l2.Color = new Vector4(1, 1, 1, 1);

            PointLight l3 = new PointLight();
            l3.Type = LightType.point;
            l3.Position = new Vector3(-2, 2, -2);
            l3.Color = new Vector4(1, 1, 1, 1);
            l3.Attenuation = 10.0f;
            l3.FallOff = 2f;

            AddLight(l1);
            AddLight(l2);
            //AddLight(l3);
        }

        #region XML Serialization

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
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.Name == "GameObject")
                {
                    GameObject obj = new GameObject();
                    (obj as IXmlSerializable).ReadXml(reader);
                    GameObjects.Add(obj);
                }
                reader.ReadEndElement();

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
        }
        #endregion
        #endregion


    }
}