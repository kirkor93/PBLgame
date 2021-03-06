﻿using System;
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
    public delegate void  CollisionHandler(Object sender, ColArgs e);
    public delegate void TriggerHandler(Object sender, ColArgs e);

    public class ColArgs : EventArgs
    {
        public Collider EnemyCollider
        {
            get;
            set;
        }

        public Collider MyCollider
        {
            get;
            set;
        }

        public ColArgs(Collider myCol, Collider enemyCol)
        {
            MyCollider = myCol;
            EnemyCollider = enemyCol;
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
        private SphereCollider _mainCollider;
        private List<SphereCollider> _sphereColliders;
        private List<BoxCollider> _boxColliders;
        private bool _onTerrain;
        private float _mass = 50.0f;


        private int _cnt;
        private int _terrainCalls;
        #endregion
        #endregion

        #region Properties
        public int TerrainCalls
        {
            get { return _terrainCalls; }
            set { _terrainCalls = value; }
        }
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
        public override bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                if(_enabled) UpdatePositions();
            }
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
        }

        // ReSharper disable once IntroduceOptionalParameters.Global
        // for a retarded serializer
        public Collision(GameObject owner) : this(owner, false, 0f)
        {
        }

        public Collision(GameObject owner, bool rigid, float mass): base(owner)
        {
            _static = true;
            _rigidbody = rigid;
            _mainCollider = null;
            _sphereColliders = new List<SphereCollider>();
            _boxColliders = new List<BoxCollider>();
            _onTerrain = false;
            _mass = mass;
            PhysicsSystem.AddCollisionObject(owner);
        }

        public Collision(Collision src, GameObject owner) : this(owner, src._rigidbody, src._mass)
        {
            CopyData(src);
            _mainCollider = new SphereCollider(src._mainCollider, this);
            foreach (SphereCollider srcCollider in src._sphereColliders)
            {
                _sphereColliders.Add(new SphereCollider(srcCollider, this));
            }
            foreach (BoxCollider srcCollider in src._boxColliders)
            {
                _boxColliders.Add(new BoxCollider(srcCollider, this));
            }
        }

        public override void Initialize(bool editor)
        {
            base.Initialize(editor);
            MainCollider.UpdatePosition();
            foreach (SphereCollider sc in SphereColliders)
            {
                sc.UpdatePosition();
            }
            foreach (BoxCollider bc in BoxColliders)
            {
                bc.UpdatePosition();
            }
        }

        public override Component Copy(GameObject newOwner)
        {
            return new Collision(this, newOwner);
        }

        public override void Update(GameTime gameTime)
        {
            if(Enabled)
            {
                if (_terrainCalls == 0) _onTerrain = false;
                if (!Static)
                {
                    MainCollider.UpdatePosition();
                    foreach (SphereCollider sc in SphereColliders)
                    {
                        sc.UpdatePosition();
                    }
                    foreach (BoxCollider bc in BoxColliders)
                    {
                        bc.UpdatePosition();
                    }
                }
            }
        }
        
        public override void Draw(GameTime gameTime)
        {
            if (Enabled)
            {
                foreach (BoxCollider box in _boxColliders)
                {
                    box.Draw();
                }
                foreach (SphereCollider sphere in _sphereColliders)
                {
                    sphere.Draw();
                }
                MainCollider.Draw();
            }
        }

        public void UpdatePositions()
        {
            if(_enabled && !Static)
            {
                if (MainCollider == null) return;
                MainCollider.UpdatePosition();
                if(SphereColliders != null)
                {
                    foreach(SphereCollider sc in SphereColliders)
                    {
                        sc.UpdatePosition();
                    }
                }
                if(BoxColliders != null)
                {
                    foreach (BoxCollider bc in BoxColliders)
                    {
                        bc.UpdatePosition();
                    }
                }
            }
        }

        public void UpdateDisablePositions()
        {
            if (!Static)
            {
                if (MainCollider == null) return;
                MainCollider.UpdatePosition();
                if (SphereColliders != null)
                {
                    foreach (SphereCollider sc in SphereColliders)
                    {
                        sc.UpdatePosition();
                    }
                }
                if (BoxColliders != null)
                {
                    foreach (BoxCollider bc in BoxColliders)
                    {
                        bc.UpdatePosition();
                    }
                }
            }
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
       
        public int ChceckCollisionDeeper(GameObject collisionGO)
        {
            if (!_enabled) return 0;
            _cnt = 0;
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
                                    break;
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
                                    break;
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
                                break;
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
                                    break;
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
                                    break;
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
                                break;
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
                                break;
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
                                break;
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

        #region CollisionDetected
        private void CollisionDetected(SphereCollider myCol, SphereCollider enemyCol)
        {
            if (!_enabled) return;
            if (!myCol.Trigger && !enemyCol.Trigger)
            {
                if (enemyCol.Owner.gameObject.Tag == "Terrain")
                {
                    AffectCollision(myCol, enemyCol);
                }
                else
                {
                    if (OnCollision != null) OnCollision(this, new ColArgs(myCol,enemyCol));
                    if (enemyCol.Owner.OnCollision != null) enemyCol.Owner.OnCollision(this, new ColArgs(enemyCol, myCol));
                    ++_cnt;
                }
                //Console.WriteLine("Collison!");
            }
            else
            {
                //Console.WriteLine("Trigger!");
                if (OnTrigger != null) OnTrigger(this, new ColArgs(myCol, enemyCol)); 
                if (enemyCol.Owner.OnTrigger != null) enemyCol.Owner.OnTrigger(this, new ColArgs(enemyCol, myCol));
            }
        }

        private void CollisionDetected(SphereCollider myCol, BoxCollider enemyCol)
        {
            if (!_enabled) return;
            if (!myCol.Trigger && !enemyCol.Trigger)
            {
                if (enemyCol.Owner.gameObject.Tag == "Terrain")
                {
                    AffectCollision(myCol, enemyCol);
                }
                else
                {
                    if (OnCollision != null) OnCollision(this, new ColArgs(myCol, enemyCol));
                    if (enemyCol.Owner.OnCollision != null) enemyCol.Owner.OnCollision(this, new ColArgs(enemyCol, myCol));
                    ++_cnt;
                }
                //Console.WriteLine("Collison!");
            }
            else
            {
                //Console.WriteLine("Trigger!");
                if (OnTrigger != null) OnTrigger(this, new ColArgs(myCol, enemyCol));
                if (enemyCol.Owner.OnTrigger != null) enemyCol.Owner.OnTrigger(this, new ColArgs(enemyCol, myCol));
            }
        }

        private void CollisionDetected(BoxCollider myCol, SphereCollider enemyCol)
        {
            if (!_enabled) return;
            if (!myCol.Trigger && !enemyCol.Trigger)
            {
                if (enemyCol.Owner.gameObject.Tag == "Terrain")
                {
                    AffectCollision(myCol, enemyCol);
                }
                else
                {
                    if (OnCollision != null) OnCollision(this, new ColArgs(myCol, enemyCol));
                    if (enemyCol.Owner.OnCollision != null) enemyCol.Owner.OnCollision(this, new ColArgs(enemyCol, myCol));
                    ++_cnt;
                }
                //Console.WriteLine("Collison!");
            }
            else
            {
                //Console.WriteLine("Trigger!");
                if (OnTrigger != null) OnTrigger(this, new ColArgs(myCol, enemyCol));
                if (enemyCol.Owner.OnTrigger != null) enemyCol.Owner.OnTrigger(this, new ColArgs(enemyCol, myCol));
            }
        }

        private void CollisionDetected(BoxCollider myCol, BoxCollider enemyCol)
        {
            if (!_enabled) return;
            if (!myCol.Trigger && !enemyCol.Trigger)
            {
                if (enemyCol.Owner.gameObject.Tag == "Terrain")
                {
                    AffectCollision(myCol, enemyCol);
                }
                else
                {
                    if (OnCollision != null) OnCollision(this, new ColArgs(myCol, enemyCol));
                    if (enemyCol.Owner.OnCollision != null) enemyCol.Owner.OnCollision(this, new ColArgs(enemyCol, myCol));
                    ++_cnt;
                }
                //Console.WriteLine("Collison!");
            }
            else
            {
                //Console.WriteLine("Trigger!");
                if (OnTrigger != null) OnTrigger(this, new ColArgs(myCol, enemyCol));
                if (enemyCol.Owner.OnTrigger != null) enemyCol.Owner.OnTrigger(this, new ColArgs(enemyCol, myCol));
            }
        }
        #endregion

        #region AffectCollision
        private void AffectCollision(SphereCollider myCol, SphereCollider enemyCol)
        {
            if (!Enabled) return;
            if(myCol.Owner.Rigidbody)
            {
                //myCol.Owner.gameObject.transform.Translate(0.0f, 0.01f * _mass, 0.0f);
                ++_terrainCalls;
            }
        }

        private void AffectCollision(SphereCollider myCol, BoxCollider enemyCol)
        {
            if (!Enabled) return;
            if (myCol.Owner.Rigidbody)
            {
                //myCol.Owner.gameObject.transform.Translate(0.0f, 0.01f * _mass, 0.0f);
                ++_terrainCalls;
            }
        }

        private void AffectCollision(BoxCollider myCol, SphereCollider enemyCol)
        {
            if (!Enabled) return;
            if (myCol.Owner.Rigidbody)
            {
                //myCol.Owner.gameObject.transform.Translate(0.0f, 0.01f * _mass, 0.0f);
                ++_terrainCalls;
            }
        }
        private void AffectCollision(BoxCollider myCol, BoxCollider enemyCol)
        {
            if (!Enabled) return;
            if (myCol.Owner.Rigidbody)
            {
                //myCol.Owner.gameObject.transform.Translate(0.0f, 0.01f * _mass, 0.0f);
                ++_terrainCalls;
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

        public BoxCollider GenerateBoxCollider()
        {
            if (gameObject.renderer == null || gameObject.renderer.MyMesh == null || gameObject.renderer.MyMesh.Model == null) return null;
            Matrix localWorld = gameObject.transform.World;
            localWorld.Translation = Vector3.Zero;
            BoundingBox aabb = gameObject.renderer.GenerateAABB(localWorld);
            return new BoxCollider(this, aabb.GetCenter(), aabb.GetSize(), false);
        }

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
