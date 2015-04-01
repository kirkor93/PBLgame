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
        private List<GameObject> _sceneGameObjects;
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

        #endregion

        #region Methods

        public Scene()
        {
            _sceneGameObjects = new List<GameObject>();
            _serializer = new XmlSerializer(typeof(Scene));
        }
        
        public void AddGameObject(GameObject obj)
        {
            _sceneGameObjects.Add(obj);
        }

        public void RemoveGameObject(GameObject obj)
        {
            _sceneGameObjects.Remove(obj);
        }

        public void RemoveGameObject(string name)
        {
            _sceneGameObjects.RemoveAll(item => item.Name == name);
        }

        public void RemoveGameObject(int id)
        {
            _sceneGameObjects.RemoveAll(item => item.ID == id);
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
            
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new System.NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("GameObjects");
            foreach (GameObject sceneGameObject in _sceneGameObjects)
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