using System;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using PBLgame.Engine.Components;

namespace PBLgame.Engine.GameObjects
{

    public abstract class Light : GameObject
    {
        #region Variables
        private Vector4 _color;
        private LightType _type;
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
        public LightType Type
        {
            get
            {
                return _type;
            }
        }
        #endregion

        #region  Methods

        protected Light(Light source, GameObject sourceParent) : base(source, sourceParent)
        {
            _color = source._color;
            _type  = source._type;
        }

        protected Light() { }


        #region XML serialization

        public override void WriteXml(XmlWriter writer)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            writer.WriteAttributeString("Type", _type.ToString());
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

            reader.ReadStartElement();
            if (reader.Name == "Color")
            {
                CultureInfo culture = CultureInfo.InvariantCulture;
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
