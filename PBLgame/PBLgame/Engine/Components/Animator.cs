using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using PBLgame.Engine.GameObjects;

namespace PBLgame.Engine.Components
{
    public class Animator : Component
    {

        #region Methods
        public Animator(GameObject owner) : base(owner)
        {

        }

        public override void Update()
        {

        }

        public override void Draw()
        {

        }

        #region XML Serialization

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
        }

        #endregion
        #endregion
    }
}
