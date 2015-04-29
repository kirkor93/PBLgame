using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;

using PBLgame.Engine.Physics;

namespace PBLgame.Engine.Components
{
#region CollisionEventHelpers
    public delegate void  CollisionHandler(Object sender, EventArgs e);
    public delegate void TriggerHandler(Object sender, EventArgs e);

    public class ColArgs : EventArgs
    {
            public SphereCollider EnemySphere
            {
                get;
                set;
            }

            public ColArgs(SphereCollider col)
            {
                EnemySphere = col;
            }
    }
#endregion

    public class Collision : Component
    {
        #region Variables
        #region Public
        public event CollisionHandler OnCollision;
        public event TriggerHandler OnTrigger;
        //public event CollisionHandler OnCollisionEnter;
        //public event CollisionHandler OnCollisionStay;
        //public event CollisionHandler OnCollisionExit;
        //public event TriggerHandler OnTriggerEnter;
        //public event TriggerHandler OnTriggerStay;
        //public event TriggerHandler OnTriggerExit;
        #endregion
        #region Private
        private bool _static;
        private bool _rigidbody;
        private SphereCollider _mainCollider;
        private List<SphereCollider> _sphereColliders;
        //private bool _previousFrameCollision;
        //private List<SphereCollider> _previousFrameCollided;
        #endregion

        #endregion

        #region Properties
        public bool Static
        {
            get
            {
                return _static;
            }
            set
            {
                _static = value;
            }
        }
        public bool Rigidbody
        {
            get
            {
                return _rigidbody;
            }
            set
            {
                _rigidbody = value;
            }
        }
        public SphereCollider MainCollider
        {
            get
            {
                return _mainCollider;
            }
            set
            {
                _mainCollider = value;
            }
        }
        public List<SphereCollider> SphereColliders
        {
            get
            {
                return _sphereColliders;
            }
            private set { }
        }
        #endregion

        #region Methods
        private void CopyData(Collision src)
        {
            OnCollision = src.OnCollision;
            OnTrigger = src.OnTrigger;

            _static = src._static;
            _rigidbody = src._rigidbody;
            _mainCollider = src._mainCollider;
            _sphereColliders = src._sphereColliders;
        }
        public Collision(GameObject owner) : base(owner)
        {
            _static = true;
            _rigidbody = false;
            _mainCollider = null;
            _sphereColliders = new List<SphereCollider>();
        }

        public Collision(Collision src, GameObject owner) : base(owner)
        {
            CopyData(src);
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            
        }

        public void ResetPreviousState()
        {

        }

        public void ChceckCollisionDeeper(GameObject collisionGO)
        {
            if(_sphereColliders.Count > 0)
            {
                if(collisionGO.collision.SphereColliders.Count > 0)
                {
                    foreach(SphereCollider myCol in SphereColliders)
                    {
                        foreach(SphereCollider enemyCol in collisionGO.collision.SphereColliders)
                        {
                            if(myCol.Contains(enemyCol) != ContainmentType.Disjoint)
                            {
                                CollisionDetected(myCol,enemyCol);
                            }
                        }
                    }
                }
                else
                {
                    foreach (SphereCollider myCol in SphereColliders)
                    {
                        if (myCol.Contains(collisionGO.collision.MainCollider) != ContainmentType.Disjoint)
                        {
                            CollisionDetected(myCol, collisionGO.collision.MainCollider);
                        }                        
                    }
                }
            }
            else
            {
                if (collisionGO.collision.SphereColliders.Count > 0)
                {
                    foreach (SphereCollider enemyCol in collisionGO.collision.SphereColliders)
                    {
                        if (MainCollider.Contains(enemyCol) != ContainmentType.Disjoint)
                        {
                            CollisionDetected(MainCollider, enemyCol);
                        }
                    }
                }
                else
                {
                    if(MainCollider.Contains(collisionGO.collision.MainCollider) != ContainmentType.Disjoint)
                    {
                        CollisionDetected(MainCollider, collisionGO.collision.MainCollider);
                    }
                }
            }
        }


        private void CollisionDetected(SphereCollider myCol, SphereCollider enemyCol)
        {
            if (!myCol.Trigger && !enemyCol.Trigger)
            {
                if (OnCollision != null) OnCollision(this, new ColArgs(enemyCol));
                AffectCollision(myCol, enemyCol);
            }
            else
            {
                if (OnTrigger != null) OnTrigger(this, new ColArgs(enemyCol));
            }
        }

        private void AffectCollision(SphereCollider myCol, SphereCollider enemyCol)
        {
            Vector3 moveVector = new Vector3(myCol.TotalPosition.X - myCol.PreviousPosition.X, myCol.TotalPosition.Y - myCol.PreviousPosition.Y, myCol.TotalPosition.Z - myCol.PreviousPosition.Z);
            moveVector.Normalize();

            Vector3 direction = myCol.TotalPosition - enemyCol.TotalPosition;
            float intersectionValue = myCol.Radius + enemyCol.Radius - direction.Length();
            direction.Normalize();
            direction *= intersectionValue;
            direction *= moveVector;
            if (!myCol.Owner.Static) myCol.Owner.gameObject.transform.Translate(direction);
        }

        private void AffectTrigger(SphereCollider myCol, SphereCollider enemyCol)
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
