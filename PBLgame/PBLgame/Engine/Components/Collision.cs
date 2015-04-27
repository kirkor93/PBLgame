using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;

namespace PBLgame.Engine.Components
{
    public class Collision : Component
    {

        #region Variables
        private bool _rigidbody;
        private BoundingSphere _mainCollider;
        private List<BoundingBox> _boundingBoxes;
        private List<BoundingSphere> _boundingSpheres;

        
        #endregion

        #region Properties
        #endregion

        #region Methods
        public Collision(GameObject owner) : base(owner)
        {
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime)
        {
            
        }

        public void UpdateBoxCollider(Matrix world)
        {  }
         
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
