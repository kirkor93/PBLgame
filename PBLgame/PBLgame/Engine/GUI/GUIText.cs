using System;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.Singleton;

namespace PBLgame.Engine.GUI
{
    public class GUIText : IXmlSerializable
    {
        private Vector2 _localPosition;
        private SpriteFont _font;
        private string _text;
        private string _fontName = string.Empty;

        public SpriteFont Font
        {
            get { return _font; }
            set { _font = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public Vector2 LocalPosition
        {
            get { return _localPosition; }
            set { _localPosition = value; }
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            _fontName = reader.GetAttribute("Font");
            Font = ResourceManager.Instance.GetFont(_fontName);
            Text = reader.GetAttribute("Text");
            reader.ReadStartElement();
            if (reader.Name == "LocalPosition")
            {
                Vector2 tmp = Vector2.Zero;
                tmp.X = Convert.ToSingle(reader.GetAttribute("x"), culture);
                tmp.Y = Convert.ToSingle(reader.GetAttribute("y"), culture);
                reader.Read();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            writer.WriteStartElement("GUIText");
            writer.WriteAttributeString("Font", _fontName);
            writer.WriteAttributeString("Text", Text);
            writer.WriteStartElement("LocalPosition");
            writer.WriteAttributeString("x", LocalPosition.X.ToString("G", culture));
            writer.WriteAttributeString("y", LocalPosition.Y.ToString("G", culture));
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
}