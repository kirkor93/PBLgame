using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using PBLgame.Engine.GameObjects;

namespace PBLgame.Engine.Scene
{
    public class Scene : IXmlSerializable
    {
        #region Variables
        #region Private
        //It's gonna be scene graph later
        private List<GameObject> _gameObjects;
        private Camera _mainCamera;

        private XmlSerializer _serializer;

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
        }
        
        public void AddGameObject(GameObject obj)
        {
            _gameObjects.Add(obj);
        }

        public void RemoveGameObject(GameObject obj)
        {
            _gameObjects.Remove(obj);
        }

        public void RemoveGameObject(string name)
        {
            _gameObjects.RemoveAll(item => item.Name == name);
        }

        public void RemoveGameObject(int id)
        {
            _gameObjects.RemoveAll(item => item.ID == id);
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
                _gameObjects = scene._gameObjects;
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
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement();
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


    }
}