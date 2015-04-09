using System;
using System.Globalization;
using System.Xml;
using PBLgame.Engine.Components;

using Microsoft.Xna.Framework;

namespace PBLgame.Engine.GameObjects
{
    public class MyDirectionalLight : Light
    {
        #region Variables
        private float _intensity;
        #endregion

        #region Properites
        public float Intensity
        {
            get
            {
                return _intensity;
            }
            set
            {
                _intensity = value;
            }
        }
        

        public Vector3 Direction
        {
            get
            {
                return transform.Position;
            }
            set
            {
                transform.Position = value;
            }
        }
        #endregion

        #region Methods

        #region XML serialization

        public override void WriteXml(XmlWriter writer)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            base.WriteXml(writer);
            writer.WriteStartElement("DirectionalLight");
            writer.WriteAttributeString("Intensity", Convert.ToString(Intensity, culture));
            writer.WriteEndElement();
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            CultureInfo culture = CultureInfo.InvariantCulture;
            Intensity = Convert.ToSingle(reader.GetAttribute("Intensity"), culture);
            reader.Read();

        }

        #endregion
        #endregion
    }
}
