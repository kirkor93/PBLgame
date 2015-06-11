using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.Singleton;

namespace PBLgame.Engine.GUI
{
    public class ProgressImage : GUIObject
    {
        private int _maxProgress = 5;
        private Texture2D[] _progressTextures;

        public int MaxProgress
        {
            get { return _maxProgress; }
            set { _maxProgress = value; }
        }

        public Texture2D[] ProgressTextures
        {
            get { return _progressTextures; }
            set { _progressTextures = value; }
        }

        public void SetProgress(int value)
        {
            if (value > _maxProgress)
            {
                value = _maxProgress;
            }
            Texture = _progressTextures[value];
        }

        public ProgressImage() : base()
        {
        }

        public override void ReadXml(XmlReader reader)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            base.ReadXml(reader);
            MaxProgress = Convert.ToInt32(reader.GetAttribute("MaxProgress"), culture);
            ProgressTextures = new Texture2D[MaxProgress + 1];
            for (int i = 0; i < MaxProgress + 1; i++)
            {
                ProgressTextures[i] = ResourceManager.Instance.GetTexture(reader.GetAttribute("Texture_" + i.ToString("G", culture)));
            }
//            Texture = ProgressTextures[0];
            reader.Read();
        }

        public override void WriteXml(XmlWriter writer)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            base.WriteXml(writer);
            writer.WriteStartElement("ProgressImage");
            writer.WriteAttributeString("MaxProgress", MaxProgress.ToString("G", culture));
            for (int i = 0; i < ProgressTextures.Length; i++)
            {
                writer.WriteAttributeString("Texture_" + i.ToString("G", culture), ProgressTextures[i].Name);
            }
            writer.WriteEndElement();
        }
    }
}