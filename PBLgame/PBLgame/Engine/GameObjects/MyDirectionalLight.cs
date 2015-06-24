using System;
using System.Globalization;
using System.Xml;
using PBLgame.Engine.Components;

using Microsoft.Xna.Framework;
using PBLgame.Engine.Scenes;

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

        public MyDirectionalLight(Scene scene) : base(scene) { }

        protected MyDirectionalLight(MyDirectionalLight source, GameObject sourceParent) : base(source, sourceParent)
        {
            _intensity = source._intensity;
        }

        public override GameObject Copy(GameObject sourceParent)
        {
            return new MyDirectionalLight(this, sourceParent);
        }

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
