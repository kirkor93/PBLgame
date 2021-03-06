﻿using System;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using PBLgame.Engine.Components;
using PBLgame.Engine.Scenes;

namespace PBLgame.Engine.GameObjects
{

    public abstract class Light : GameObject
    {
        #region Variables
        private Vector4 _color;
        private LightType _type;
        private bool _used = true;
        #endregion

        #region Properites
        public Vector4 Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }

        public bool HasShadow { get; set; }

        public LightType Type
        {
            get
            {
                return _type;
            }
        }

        /// <summary>
        /// Used by simple light manager to decide when to enable this light.
        /// </summary>
        public bool UseLight
        {
            get { return Enabled & _used; }
            set { _used = value; }
        }

        #endregion

        #region  Methods

        protected Light(Light source, GameObject sourceParent) : base(source, sourceParent)
        {
            _color = source._color;
            _type  = source._type;
            HasShadow = source.HasShadow;
        }

        protected Light(Scene scene) : base(scene)
        {
            HasShadow = true;
        }


        #region XML serialization

        public override void WriteXml(XmlWriter writer)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            writer.WriteAttributeString("Type", _type.ToString());
            writer.WriteAttributeString("HasShadow", HasShadow.ToString());
            writer.WriteStartElement("Color");
            writer.WriteAttributeString("r", Convert.ToString(Color.X, culture));
            writer.WriteAttributeString("g", Convert.ToString(Color.Y, culture));
            writer.WriteAttributeString("b", Convert.ToString(Color.Z, culture));
            writer.WriteAttributeString("a", Convert.ToString(Color.W, culture));
            writer.WriteEndElement();

            //GameObject part
            writer.WriteStartElement("GameObject");
            base.WriteXml(writer);
            writer.WriteEndElement();
        }

        public override void ReadXml(XmlReader reader)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            reader.MoveToContent();
            string t = reader.GetAttribute("Type");
            switch (t)
            {
                case "Directional":
                    _type = LightType.Directional;
                    break;
                case "Point":
                    _type = LightType.Point;
                    break;
            }
            HasShadow = Convert.ToBoolean(reader.GetAttribute("HasShadow") ?? "True");
            reader.ReadStartElement();
            if (reader.Name == "Color")
            {
                Vector4 c = Vector4.One;
                c.X = Convert.ToSingle(reader.GetAttribute("r"), culture);
                c.Y = Convert.ToSingle(reader.GetAttribute("g"), culture);
                c.Z = Convert.ToSingle(reader.GetAttribute("b"), culture);
                c.W = Convert.ToSingle(reader.GetAttribute("a"), culture);
                Color = c;
            }
            reader.Read();
            base.ReadXml(reader);
            reader.Read();
        }

        #endregion
        #endregion
    }

#region LightEnumType
    public enum LightType
    {
        Directional = 0,
        Point
    }
#endregion
}
