using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using PBLgame.Engine.GameObjects;

namespace PBLgame.Engine.Components
{
    public abstract class Component : IXmlSerializable
    {
        #region Variables
        protected bool _enabled;
        protected GameObject _gameObject;
        #endregion
        #region Properties
        public bool Enabled
        {
            get 
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }

        public GameObject GameObject
        {
            get
            {
                return _gameObject;
            }
        }
        #endregion
        #region Methods

        protected Component()
        {
            Enabled = true;
        }

        protected Component(GameObject owner) : this()
        {
            _gameObject = owner;
        }

        public abstract void Update();
        public abstract void Draw();

        #region XML serialization
        public XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            throw new System.NotImplementedException();
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            
        }
        #endregion
        #endregion
    }
}
