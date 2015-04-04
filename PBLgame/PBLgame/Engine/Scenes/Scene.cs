using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using PBLgame.Engine.GameObjects;

namespace PBLgame.Engine.Scenes
{
    public class Scene : IXmlSerializable
    {
        #region Variables
        #region Private
        //It's gonna be scene graph later
        private List<GameObject> _gameObjects;
        private List<int> _takenIdNumbers; 
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

        #endregion

        #region Methods

        public Scene()
        {
            _gameObjects = new List<GameObject>();
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