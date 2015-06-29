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
    public class IntroScene : GUIObject
    {
        #region Variables
        private TimeSpan _length;
        private Texture2D _fontBg;
        private Vector2 _moveDirection;
        private GameTime _currentGameTime;
        private TimeSpan _lastChangeTime;

        #endregion

        #region Properties

        public bool FirstScene { get; set; }

        public TimeSpan Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public Vector2 MoveDirection
        {
            get { return _moveDirection; }
            set { _moveDirection = value; }
        }

        #endregion
        #region Methods

        public IntroScene()
        {
            _fontBg = ResourceManager.Instance.GetTexture(@"Textures\GUI\Font_background");
        }

        public void Update(GameTime gameTime)
        {
            if (!Enabled)
            {
                return;
            }

            _currentGameTime = gameTime;
            if (_lastChangeTime + new TimeSpan(0, 0, 0, 0, 50) < gameTime.TotalGameTime
                && Position.X + MoveDirection.X < 0
                && Position.Y + MoveDirection.Y < 0)
            {
                _lastChangeTime = gameTime.TotalGameTime;
                Position += MoveDirection;
            }
        }

        public override void Draw(SpriteBatch batch)
        {
            if (!Enabled)
            {
                return;
            }
            batch.Draw(Texture, _boundries, Color.White);
            if (Text != null)
            {
                Vector2 stringSize = Text.Font.MeasureString(Text.Text);

                Vector2 textPosition = new Vector2(batch.GraphicsDevice.Viewport.Bounds.Center.X, batch.GraphicsDevice.Viewport.Bounds.Bottom);
                textPosition.X -= (stringSize.X / 2.0f);
                textPosition.Y -= (stringSize.Y + batch.GraphicsDevice.Viewport.Bounds.Height * 0.05f);
                Rectangle rect = new Rectangle((int)textPosition.X - 10, (int)textPosition.Y - 10, (int)stringSize.X + 20,
                    (int) stringSize.Y + 20);
                batch.Draw(_fontBg, rect, Color.White);
                batch.DrawString(Text.Font, Text.Text, textPosition, Color.White);
            }
        }

        #region XmlSerialization

        public override void ReadXml(XmlReader reader)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            base.ReadXml(reader);
            Length = new TimeSpan(0, 0, Convert.ToInt32(reader.GetAttribute("Length"), culture));
            FirstScene = Convert.ToBoolean(reader.GetAttribute("FirstScene"), culture);
            reader.Read();
            if (reader.Name == "MoveDirection")
            {
                Vector2 tmp = Vector2.Zero;
                tmp.X = Convert.ToInt32(reader.GetAttribute("x"), culture);
                tmp.Y = Convert.ToInt32(reader.GetAttribute("y"), culture);
                MoveDirection = tmp;
                reader.Read();
            }
            reader.Read();
        }

        public override void WriteXml(XmlWriter writer)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            base.WriteXml(writer);
            writer.WriteStartElement("IntroScene");
            writer.WriteAttributeString("Length", Length.Seconds.ToString("G", culture));
            writer.WriteAttributeString("FirstScene", FirstScene.ToString(culture));
            writer.WriteStartElement("MoveDirection");
            writer.WriteAttributeString("x", MoveDirection.X.ToString("G", culture));
            writer.WriteAttributeString("y", MoveDirection.Y.ToString("G", culture));
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        #endregion
        #endregion
    }
}