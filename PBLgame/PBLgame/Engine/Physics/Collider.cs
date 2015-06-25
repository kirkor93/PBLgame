using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using PBLgame.Engine.Components;

namespace PBLgame.Engine.Physics
{
    /// <summary>
    /// Just for consistency.
    /// </summary>
    public abstract class Collider
    {
        protected Collision _owner;
        protected bool _trigger;

        public Collision Owner
        {
            get { return _owner; }
        }

        public bool Trigger
        {
            get
            {
                return _trigger;
            }
            set
            {
                _trigger = value;
            }
        }
    }
}