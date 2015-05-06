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

        public SphereCollider MySphere
        {
            get;
            set;
        }

        public BoxCollider EnemyBox
        {
            get;
            set;
        }

        public BoxCollider MyBox
        {
            get;
            set;
        }

        public ColArgs(SphereCollider myCol,SphereCollider enemyCol)
        {
            MySphere = myCol;
            EnemySphere = enemyCol;
        }

        public ColArgs(SphereCollider myCol, BoxCollider enemyCol)
        {
            MySphere = myCol;
            EnemyBox = enemyCol;
        }

        public ColArgs(BoxCollider myCol, SphereCollider enemyCol)
        {
            MyBox = myCol;
            EnemySphere = enemyCol;
        }

        public ColArgs(BoxCollider myCol, BoxCollider enemyCol)
        {
            MyBox = myCol;
            EnemyBox = enemyCol;
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
        private List<BoxCollider> _boxColliders;
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
        public List<BoxCollider> BoxColliders
        {
            get
            {
                return _boxColliders;
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
            _boxColliders = new List<BoxCollider>();
        }

        public Collision(Collision src, GameObject owner) : base(owner)
        {
            CopyData(src);
        }

        public override void Update(GameTime gameTime)
        {
            MainCollider.UpdatePosition();
            foreach(SphereCollider sc in SphereColliders)
            {
                sc.UpdatePosition();
            }
            foreach (BoxCollider bc in BoxColliders)
            {
                bc.UpdatePosition();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach(BoxCollider box in _boxColliders)
            {
                box.Draw();
            }
            foreach(SphereCollider sphere in _sphereColliders)
            {
                sphere.Draw();
            }
            MainCollider.Draw();
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
                if(collisionGO.collision.BoxColliders.Count > 0)
                {
                    foreach (SphereCollider myCol in SphereColliders)
                    {
                        foreach (BoxCollider enemyCol in collisionGO.collision.BoxColliders)
                        {
                            if (myCol.Contains(enemyCol) != ContainmentType.Disjoint)
                            {
                                CollisionDetected(myCol, enemyCol);
                            }
                        }
                    }
                }
                if(collisionGO.collision.SphereColliders.Count == 0 && collisionGO.collision.BoxColliders.Count == 0)
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
            if (_boxColliders.Count > 0)
            {
                if (collisionGO.collision.SphereColliders.Count > 0)
                {
                    foreach (BoxCollider myCol in BoxColliders)
                    {
                        foreach (SphereCollider enemyCol in collisionGO.collision.SphereColliders)
                        {
                            if (myCol.Contains(enemyCol) != ContainmentType.Disjoint)
                            {
                                CollisionDetected(myCol, enemyCol);
                            }
                        }
                    }
                }
                if (collisionGO.collision.BoxColliders.Count > 0)
                {
                    foreach (BoxCollider myCol in BoxColliders)
                    {
                        foreach (BoxCollider enemyCol in collisionGO.collision.BoxColliders)
                        {
                            if (myCol.Contains(enemyCol) != ContainmentType.Disjoint)
                            {
                                CollisionDetected(myCol, enemyCol);
                            }
                        }
                    }
                }
                if (collisionGO.collision.SphereColliders.Count == 0 && collisionGO.collision.BoxColliders.Count == 0)
                {
                    foreach (BoxCollider myCol in BoxColliders)
                    {
                        if (myCol.Contains(collisionGO.collision.MainCollider) != ContainmentType.Disjoint)
                        {
                            CollisionDetected(myCol, collisionGO.collision.MainCollider);
                        }
                    }
                }
            }
            
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
                if (collisionGO.collision.BoxColliders.Count > 0)
                {
                    foreach (BoxCollider enemyCol in collisionGO.collision.BoxColliders)
                    {
                        if (MainCollider.Contains(enemyCol) != ContainmentType.Disjoint)
                        {
                            CollisionDetected(MainCollider, enemyCol);
                        }
                    }
                }
                if(collisionGO.collision.BoxColliders.Count == 0 && collisionGO.collision.SphereColliders.Count == 0)
                {
                    if(MainCollider.Contains(collisionGO.collision.MainCollider) != ContainmentType.Disjoint)
                    {
                        CollisionDetected(MainCollider, collisionGO.collision.MainCollider);
                    }
                }
            }
        }

        #region CollisionDetected
        private void CollisionDetected(SphereCollider myCol, SphereCollider enemyCol)
        {
            if (!myCol.Trigger && !enemyCol.Trigger)
            {
                if (OnCollision != null) OnCollision(this, new ColArgs(myCol,enemyCol));
                AffectCollision(myCol, enemyCol);
                //Console.WriteLine("Collison!");
            }
            else
            {
                //Console.WriteLine("Trigger!");
                if (OnTrigger != null) OnTrigger(this, new ColArgs(myCol,enemyCol));
            }
        }

        private void CollisionDetected(SphereCollider myCol, BoxCollider enemyCol)
        {
            if (!myCol.Trigger && !enemyCol.Trigger)
            {
                if (OnCollision != null) OnCollision(this, new ColArgs(myCol, enemyCol));
                AffectCollision(myCol, enemyCol);
                //Console.WriteLine("Collison!");
            }
            else
            {
                //Console.WriteLine("Trigger!");
                if (OnTrigger != null) OnTrigger(this, new ColArgs(myCol, enemyCol));
            }
        }

        private void CollisionDetected(BoxCollider myCol, SphereCollider enemyCol)
        {
            if (!myCol.Trigger && !enemyCol.Trigger)
            {
                if (OnCollision != null) OnCollision(this, new ColArgs(myCol, enemyCol));
                AffectCollision(myCol, enemyCol);
                //Console.WriteLine("Collison!");
            }
            else
            {
                //Console.WriteLine("Trigger!");
                if (OnTrigger != null) OnTrigger(this, new ColArgs(myCol, enemyCol));
            }
        }

        private void CollisionDetected(BoxCollider myCol, BoxCollider enemyCol)
        {
            if (!myCol.Trigger && !enemyCol.Trigger)
            {
                if (OnCollision != null) OnCollision(this, new ColArgs(myCol, enemyCol));
                AffectCollision(myCol, enemyCol);
                //Console.WriteLine("Collison!");
            }
            else
            {
                //Console.WriteLine("Trigger!");
                if (OnTrigger != null) OnTrigger(this, new ColArgs(myCol, enemyCol));
            }
        }
        #endregion

        #region AffectCollision
        private void AffectCollision(SphereCollider myCol, SphereCollider enemyCol)
        {
            Vector3 moveVector = myCol.TotalPosition - myCol.PreviousPosition;
            moveVector.Normalize();

            Vector3 direction = myCol.TotalPosition - enemyCol.TotalPosition;
            float intersectionValue = myCol.Radius + enemyCol.Radius - direction.Length();
            direction.Normalize();
            direction *= intersectionValue;
            direction *= moveVector;
            //direction.Y = 0;
            if (!myCol.Owner.Static && intersectionValue > 0 && direction.Length() > 0) myCol.Owner.gameObject.transform.Translate(direction);
        }

        private void AffectCollision(SphereCollider myCol, BoxCollider enemyCol)
        {
            
        }

        private void AffectCollision(BoxCollider myCol, SphereCollider enemyCol)
        {

        }
        private void AffectCollision(BoxCollider myCol, BoxCollider enemyCol)
        {

        }
        #endregion

        #region AffectTrigger
        private void AffectTrigger(SphereCollider myCol, SphereCollider enemyCol)
        {

        }
        private void AffectTrigger(SphereCollider myCol, BoxCollider enemyCol)
        {

        }

        private void AffectTrigger(BoxCollider myCol, SphereCollider enemyCol)
        {

        }
        private void AffectTrigger(BoxCollider myCol, BoxCollider enemyCol)
        {

        }
        #endregion

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
