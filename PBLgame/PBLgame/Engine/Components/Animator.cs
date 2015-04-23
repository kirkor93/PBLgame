using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using PBLgame.Engine.GameObjects;

namespace PBLgame.Engine.Components
{
    public class Animator : Component
    {

        #region Methods
        public Animator(GameObject owner) : base(owner)
        {
            // don't forget to implement copy constructor below
        }

        public Animator(Animator src, GameObject owner) : base(owner)
        {
            // TODO copy all
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime)
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
