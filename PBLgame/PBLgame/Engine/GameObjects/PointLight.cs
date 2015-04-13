﻿using System;
using System.Globalization;
using System.Xml;
using Microsoft.Xna.Framework;

using PBLgame.Engine.Components;

namespace PBLgame.Engine.GameObjects
{
    public class PointLight : Light
    {
        #region Variables
        private float _attenuation;
        private float _fallOff;
        #endregion

        #region Properties
        public Vector3 Position
        {
            get
            {
                if(parent != null)
                {
                    return parent.transform.Position + transform.Position;
                }
                else
                {
                    return transform.Position;
                }
            }
            set
            {
                if(parent != null)
                {
                    transform.Position = parent.transform.Position + value;
                }
                else
                {
                    transform.Position = value;
                }
            }
        }

        public float Attenuation
        {
            get
            {
                return _attenuation;
            }
            set
            {
                _attenuation = value;
            }
        }

        public float FallOff
        {
            get
            {
                return _fallOff;
            }
            set
            {
                _fallOff = value;
            }
        }
        #endregion 

        #region Methods

        public override void Update()
        {
            base.Update();
            //transform.Translate(0.1f, 0.0f, 0.0f);
        }

        #region XML Serialization

        public override void WriteXml(XmlWriter writer)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            base.WriteXml(writer);
            writer.WriteStartElement("PointLight");
            writer.WriteAttributeString("Attenuation", Convert.ToString(Attenuation, culture));
            writer.WriteAttributeString("FallOff", Convert.ToString(FallOff, culture));
            writer.WriteEndElement();
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            CultureInfo culture = CultureInfo.InvariantCulture;
            Attenuation = Convert.ToSingle(reader.GetAttribute("Attenuation"), culture);
            FallOff = Convert.ToSingle(reader.GetAttribute("FallOff"), culture);
            reader.Read();
        }

        #endregion

        #endregion
    }
}
