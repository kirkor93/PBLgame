using System;
using System.Collections.Generic;
using System.Globalization;
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
        #endregion
        #region Private
        private bool _static;
        private bool _rigidbody;
        private bool _inContact;
        private SphereCollider _mainCollider;
        private List<SphereCollider> _sphereColliders;
        private List<BoxCollider> _boxColliders;
        private bool _onTerrain;
        private float _mass;


        private int _cnt;
        #endregion
        #endregion

        #region Properties
        public bool OnTerrain
        {
            get { return _onTerrain; }
            private set { }
        }
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
        public float Mass
        {
            get
            {
                return _mass;
            }
            set
            {
                _mass = value;
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
        public bool InContact
        {
            get { return _inContact; }
            set { _inContact = value; }
        }
        #endregion

        #region Methods
        private void CopyData(Collision src)
        {
            OnCollision = src.OnCollision;
            OnTrigger = src.OnTrigger;

            _mass = src._mass;
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
            _onTerrain = false;
            _mass = 50.0f;
            _inContact = false;
            PhysicsSystem.AddCollisionObject(owner);
        }

        public Collision(Collision src, GameObject owner) : base(owner)
        {
            CopyData(src);
        }

        public override void Update(GameTime gameTime)
        {

            if(!Static)
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

            _inContact = false;
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
       
        public int ChceckCollisionDeeper(GameObject collisionGO)
        {
            _cnt = 0;
            if(!Static)
            {
                MainCollider.UpdatePosition();
                foreach (SphereCollider sphere in _sphereColliders)
                {
                    sphere.UpdatePosition();
                }
                foreach (BoxCollider box in _boxColliders)
                {
                    box.UpdatePosition();
                }
            }

            if (collisionGO.Tag != "Terrain")
            {
                //Console.WriteLine("NoTerrain");
                #region CollisionWithNoTerrain
                if (_sphereColliders.Count > 0)
                {
                    if (collisionGO.collision.SphereColliders.Count > 0)
                    {
                        foreach (SphereCollider myCol in SphereColliders)
                        {
                            foreach (SphereCollider enemyCol in collisionGO.collision.SphereColliders)
                            {
                                if (myCol.Contains(enemyCol) != ContainmentType.Disjoint)
                                {
                                    CollisionDetected(myCol, enemyCol);
                                    _inContact = true;
                                }
                            }
                        }
                    }
                    if (collisionGO.collision.BoxColliders.Count > 0)
                    {
                        foreach (SphereCollider myCol in SphereColliders)
                        {
                            foreach (BoxCollider enemyCol in collisionGO.collision.BoxColliders)
                            {
                                if (myCol.Contains(enemyCol) != ContainmentType.Disjoint)
                                {
                                    CollisionDetected(myCol, enemyCol);
                                    _inContact = true;
                                }
                            }
                        }
                    }
                    if (collisionGO.collision.SphereColliders.Count == 0 && collisionGO.collision.BoxColliders.Count == 0)
                    {
                        foreach (SphereCollider myCol in SphereColliders)
                        {
                            if (myCol.Contains(collisionGO.collision.MainCollider) != ContainmentType.Disjoint)
                            {
                                CollisionDetected(myCol, collisionGO.collision.MainCollider);
                                _inContact = true;
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
                                    _inContact = true;
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
                                    _inContact = true;
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
                                _inContact = true;
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
                                _inContact = true;
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
                                _inContact = true;
                            }
                        }
                    }
                    if (collisionGO.collision.BoxColliders.Count == 0 && collisionGO.collision.SphereColliders.Count == 0)
                    {
                        if (MainCollider.Contains(collisionGO.collision.MainCollider) != ContainmentType.Disjoint)
                        {
                            CollisionDetected(MainCollider, collisionGO.collision.MainCollider);
                            _inContact = true;
                        }
                    }
                }
                #endregion
            }
            else
            {
                //Console.WriteLine("Terrain");
                _onTerrain = false;
                #region CollidedWithTerrain
                if (_sphereColliders.Count > 0)
                {
                    if (collisionGO.collision.SphereColliders.Count > 0)
                    {
                        foreach (SphereCollider myCol in SphereColliders)
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
                    if (collisionGO.collision.SphereColliders.Count == 0 && collisionGO.collision.BoxColliders.Count == 0)
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
                    if (collisionGO.collision.BoxColliders.Count == 0 && collisionGO.collision.SphereColliders.Count == 0)
                    {
                        if (MainCollider.Contains(collisionGO.collision.MainCollider) != ContainmentType.Disjoint)
                        {
                            CollisionDetected(MainCollider, collisionGO.collision.MainCollider);
                        }
                    }
                }
                #endregion
            }
            return _cnt;
        }

        public void ResizeColliders()
        {
            foreach(BoxCollider box in _boxColliders)
            {
                box.ResizeCollider();
            }
            foreach(SphereCollider sphere in _sphereColliders)
            {
                sphere.ResizeCollider();
            }

            _mainCollider.ResizeCollider();
        }

        #region CollisionDetected
        private void CollisionDetected(SphereCollider myCol, SphereCollider enemyCol)
        {
            if (!myCol.Trigger && !enemyCol.Trigger)
            {
                if (enemyCol.Owner.gameObject.Tag == "Terrain")
                {
                    AffectCollision(myCol, enemyCol);
                }
                else
                {
                    if (OnCollision != null) OnCollision(this, new ColArgs(myCol,enemyCol));
                    ++_cnt;
                }
                //Console.WriteLine("Collison!");
            }
            else
            {
                //Console.WriteLine("Trigger!");
                if (OnTrigger != null) OnTrigger(this, new ColArgs(myCol, enemyCol));
            }
        }

        private void CollisionDetected(SphereCollider myCol, BoxCollider enemyCol)
        {
            if (!myCol.Trigger && !enemyCol.Trigger)
            {
                if (enemyCol.Owner.gameObject.Tag == "Terrain")
                {
                    AffectCollision(myCol, enemyCol);
                }
                else
                {
                    if (OnCollision != null) OnCollision(this, new ColArgs(myCol, enemyCol));
                    ++_cnt;
                }
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
                if (enemyCol.Owner.gameObject.Tag == "Terrain")
                {
                    AffectCollision(myCol, enemyCol);
                }
                else
                {
                    if (OnCollision != null) OnCollision(this, new ColArgs(myCol, enemyCol));
                    ++_cnt;
                }
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
                if (enemyCol.Owner.gameObject.Tag == "Terrain")
                {
                    AffectCollision(myCol, enemyCol);
                }
                else
                {
                    if (OnCollision != null) OnCollision(this, new ColArgs(myCol, enemyCol));
                    ++_cnt;
                }
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
            if(myCol.Owner.Rigidbody)
            {
                myCol.Owner.gameObject.transform.Translate(0.0f, 0.1f, 0.0f);
                _onTerrain = true;
            }
        }

        private void AffectCollision(SphereCollider myCol, BoxCollider enemyCol)
        {
            if (myCol.Owner.Rigidbody)
            {
                myCol.Owner.gameObject.transform.Translate(0.0f, 0.1f, 0.0f);
                _onTerrain = true;
            }
        }

        private void AffectCollision(BoxCollider myCol, SphereCollider enemyCol)
        {
            if (myCol.Owner.Rigidbody)
            {
                myCol.Owner.gameObject.transform.Translate(0.0f, 0.1f, 0.0f);
                _onTerrain = true;
            }
        }
        private void AffectCollision(BoxCollider myCol, BoxCollider enemyCol)
        {
            if (myCol.Owner.Rigidbody)
            {
                myCol.Owner.gameObject.transform.Translate(0.0f, 0.1f, 0.0f);
                _onTerrain = true;
            }
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
            CultureInfo culture = CultureInfo.InvariantCulture;

            Rigidbody = Convert.ToBoolean(reader.GetAttribute("IsRigidbody"), culture);
            Static = Convert.ToBoolean(reader.GetAttribute("IsStatic"), culture);
            reader.ReadStartElement();
            if (reader.Name == "SphereCollider")
            {
                MainCollider = new SphereCollider(this);
                MainCollider.ReadXml(reader);
                reader.Read();
            }
            if (reader.Name == "SphereColliders")
            {
                reader.Read();
                do
                {
                    SphereCollider collider = new SphereCollider(this);
                    collider.ReadXml(reader);
                    SphereColliders.Add(collider);
                    reader.Read();
                } while (reader.Name != "SphereColliders");
                reader.ReadEndElement();
            }
            if (reader.Name == "BoxColliders")
            {
                reader.Read();
                do
                {
                    BoxCollider collider = new BoxCollider(this);
                    collider.ReadXml(reader);
                    BoxColliders.Add(collider);
                    reader.Read();
                } while (reader.Name != "BoxColliders");
                reader.ReadEndElement();
            }
            reader.Read();
        }

        public override void WriteXml(XmlWriter writer)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            base.WriteXml(writer);
            writer.WriteAttributeString("IsRigidbody", Rigidbody.ToString(culture));
            writer.WriteAttributeString("IsStatic", Static.ToString(culture));
            MainCollider.WriteXml(writer);
            if (SphereColliders.Count > 0)
            {
                writer.WriteStartElement("SphereColliders");
                foreach (SphereCollider collider in SphereColliders)
                {
                    collider.WriteXml(writer);
                }
                writer.WriteEndElement();
            }
            if (BoxColliders.Count > 0)
            {
                writer.WriteStartElement("BoxColliders");
                foreach (BoxCollider collider in BoxColliders)
                {
                    collider.WriteXml(writer);
                }
                writer.WriteEndElement();
            }
        }

        #endregion
        #endregion
    }
}
