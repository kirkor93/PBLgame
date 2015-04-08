﻿using System;
using System.Diagnostics;
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

        public GameObject gameObject
        {
            get
            {
                return _gameObject;
            }
            private set
            {
                _gameObject = value;
            }
        }
        #endregion
        #region Methods

        private Component()
        {
            Enabled = true;
        }

        protected Component(GameObject owner)
        {
            Enabled = true;
            gameObject = owner;
        }

        public virtual void Update()
        {
            
        }

        public virtual void Draw()
        {
            
        }

        #region XML serialization
        public virtual XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            Enabled = Convert.ToBoolean(reader.GetAttribute("Enabled"));
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Enabled", Enabled.ToString());
        }
        #endregion
        #endregion
    }
}
