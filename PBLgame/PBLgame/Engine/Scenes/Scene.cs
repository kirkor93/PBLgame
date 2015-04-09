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
        private List<Light> _sceneLights;
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
            _takenIdNumbers = new List<int> {0};
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
            while (_takenIdNumbers.Exists(item => item == obj.ID))
            {
                obj.ID += 1;
            }
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
                Scene scene = (Scene) _serializer.Deserialize(new SceneXmlReader(file, this));
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