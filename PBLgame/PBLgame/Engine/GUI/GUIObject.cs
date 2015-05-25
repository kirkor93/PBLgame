using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.Singleton;

namespace PBLgame.Engine.GUI
{
    public class GUIObject : IXmlSerializable
    {
        #region Variables

        private Texture2D _texture;
        private Vector2 _position;
        private Vector2 _scale;
        protected Rectangle _boundries;
        #endregion

        public string Name { get; set; }
        public int Id { get; set; }

        public Texture2D Texture
        {
            get { return _texture; }
            set
            {
                _texture = value;
                UpdateBoundries();
            }
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                UpdateBoundries();
            }
        }

        public Vector2 Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                UpdateBoundries();
            }
        }

        public GUIObject()
        {
            Texture = ResourceManager.Instance.GetTexture(@"Textures\fire");
            Position = Vector2.Zero;
            Scale = Vector2.One;
            Name = "GuiObject";
        }

        public GUIObject(string name, Texture2D texture, Vector2 position, Vector2 scale)
        {
            Name = name;
            Texture = texture;
            Position = position;
            Scale = scale;
        }

        protected virtual void UpdateBoundries()
        {
            _boundries = new Rectangle((int)_position.X, (int)_position.Y, (int)(_texture.Width * _scale.X), (int)(_texture.Height * _scale.Y));
        }

        public virtual void Draw(SpriteBatch batch)
        {
            batch.Draw(Texture, _boundries, Color.White);
        }

        #region XmlSerialization
        public virtual XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            Name = reader.GetAttribute("Name");
            Id = Convert.ToInt32(reader.GetAttribute("Id"), culture);
            Texture = ResourceManager.Instance.GetTexture(reader.GetAttribute("Texture"));
            reader.ReadStartElement();
            if (reader.Name == "Position")
            {
                Vector2 tmp = Vector2.Zero;
                tmp.X = Convert.ToSingle(reader.GetAttribute("x"), culture);
                tmp.Y = Convert.ToSingle(reader.GetAttribute("y"), culture);
                Position = tmp;
            }
            reader.ReadStartElement();
            if (reader.Name == "Scale")
            {
                Vector2 tmp = Vector2.One;
                tmp.X = Convert.ToSingle(reader.GetAttribute("x"), culture);
                tmp.Y = Convert.ToSingle(reader.GetAttribute("y"), culture);
                Scale = tmp;
            }
            reader.Read();
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            writer.WriteAttributeString("Id", Id.ToString("G", culture));
            writer.WriteAttributeString("Name", Name);
            writer.WriteAttributeString("Texture", Texture.Name);
            writer.WriteStartElement("Position");
            writer.WriteAttributeString("x", Position.X.ToString("G", culture));
            writer.WriteAttributeString("y", Position.Y.ToString("G", culture));
            writer.WriteEndElement();
            writer.WriteStartElement("Scale");
            writer.WriteAttributeString("x", Scale.X.ToString("G", culture));
            writer.WriteAttributeString("y", Scale.Y.ToString("G", culture));
            writer.WriteEndElement();
        }
        #endregion
    }
}