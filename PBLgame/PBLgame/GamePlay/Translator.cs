using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;

namespace PBLgame.GamePlay
{
    class Translator : Component
    {
        private bool _isTriggered;
        private Vector3 _direction;

        public bool IsTriggered
        {
            get { return _isTriggered; }
            set
            {
                _isTriggered = value;
                if (gameObject.collision != null && value)
                {
                    gameObject.collision.Enabled = false;
                }
            }
        }

        public Vector3 Direction
        {
            get { return _direction;  }
            set
            {
                _direction = value;
                _direction.Normalize();
            }
        }

        public float Speed { get; set; }

        public TimeSpan TranslationTime = new TimeSpan(0, 0, 1);

        private TimeSpan _startTime;

        public Translator(Translator source, GameObject newOwner) : base(newOwner)
        {
            Direction = Vector3.Forward;
            Speed = 0.4f;
            TranslationTime = new TimeSpan(0, 0, 1);

        }

        public Translator(GameObject owner) : base(owner)
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (!IsTriggered)
            {
                _startTime = gameTime.TotalGameTime;
                return;
            }
            if (TranslationTime + _startTime < gameTime.TotalGameTime)
            {
                IsTriggered = false; 
            }
           
            gameObject.transform.Translate(Direction * Speed);
            base.Update(gameTime);
        }

        public override Component Copy(GameObject newOwner)
        {
            return new Translator(this, newOwner);
        }

        public override void ReadXml(XmlReader reader)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            base.ReadXml(reader);
            Speed = Convert.ToSingle(reader.GetAttribute("Speed"), culture);
            TranslationTime = new TimeSpan(0, 0, Convert.ToInt32(reader.GetAttribute("Time"), culture));
            reader.ReadStartElement();
            if (reader.Name == "Direction")
            {
                Vector3 tmp = Vector3.Zero;
                tmp.X = Convert.ToSingle(reader.GetAttribute("x"), culture);
                tmp.Y = Convert.ToSingle(reader.GetAttribute("y"), culture);
                tmp.Z = Convert.ToSingle(reader.GetAttribute("z"), culture);
                Direction = tmp;
                reader.Read();
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            base.WriteXml(writer);
            writer.WriteAttributeString("Speed", Speed.ToString("G", culture));
            writer.WriteAttributeString("Time", TranslationTime.Seconds.ToString("G", culture));
            writer.WriteStartElement("Direction");
            writer.WriteAttributeString("x", Direction.X.ToString("G", culture));
            writer.WriteAttributeString("y", Direction.Y.ToString("G", culture));
            writer.WriteAttributeString("z", Direction.Z.ToString("G", culture));
            writer.WriteEndElement();
        }
    }
}