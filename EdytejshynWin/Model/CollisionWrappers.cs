using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.Components;
using PBLgame.Engine.Physics;

namespace Edytejshyn.Model
{
    public class CollisionWrapper
    {
        #region Variables

        public readonly GameObjectWrapper Parent;

        #endregion

        #region Properties
        [Browsable(false)]
        public Collision WrappedCollision { get; private set; }

        public bool Static
        {
            get { return WrappedCollision.Static; }
            set { Parent.FireSetter(x => WrappedCollision.Static = x, WrappedCollision.Static, value); }
        }

        public float Mass
        {
            get { return WrappedCollision.Mass; }
            set { Parent.FireSetter(x => WrappedCollision.Mass = x, WrappedCollision.Mass, value); }
        }

        public bool Rigidbody
        {
            get { return WrappedCollision.Rigidbody; }
            set { Parent.FireSetter(x => WrappedCollision.Rigidbody = x, WrappedCollision.Rigidbody, value); }
        }
        
        #endregion

        #region Methods
        public CollisionWrapper(GameObjectWrapper parent, Collision collision)
        {
            Parent = parent;
            WrappedCollision = collision;
        }

        public void Draw(IDrawerStrategy drawerStrategy, GameTime gameTime)
        {
            WrappedCollision.Draw(gameTime);
        }

        public override string ToString()
        {
            return "Here I am";
        }

        #endregion

        public void AddBox()
        {
            BoxCollider boxCollider = new BoxCollider(WrappedCollision, new Vector3(20, 20, 20), false);

            Parent.FireAdder(x =>
                {
                    WrappedCollision.BoxColliders.Add(x);
                    Program.UglyStaticMain.ReSelectSceneNode();
                },
                x =>
                {
                    WrappedCollision.BoxColliders.Remove(x);
                    Program.UglyStaticMain.ReSelectSceneNode();
                }, 
                boxCollider, "Box collider");
        }

        public void AddSphere()
        {
            //EditorLogic logic = Program.UglyStaticLogic;
            //BoxCollider boxCollider = new BoxCollider(WrappedCollision, new Vector3(20, 20, 20), false);
            //BoxColliderWrapper wrapper = new BoxColliderWrapper(this, boxCollider);

            //Parent.FireAdder(x => WrappedCollision.BoxColliders.Add(x), x => WrappedCollision.BoxColliders.Remove(x), boxCollider, "Box collider");
        }

        public void Remove(ColliderWrapper colliderWrapper)
        {
            throw new NotImplementedException();
        }
    }



    public abstract class ColliderWrapper
    {
        protected readonly CollisionWrapper _collision;

        protected ColliderWrapper(CollisionWrapper collision)
        {
            _collision = collision;
        }
    }

    public class SphereColliderWrapper : ColliderWrapper
    {
        private readonly SphereCollider _collider;


        [TypeConverter(typeof(Vector3ConverterEx))]
        public Vector3 LocalPosition
        {
            get { return _collider.LocalPosition; }
            set { _collision.Parent.FireSetter(x => _collider.LocalPosition = x, _collider.LocalPosition, value); }
        }

        public float Radius
        {
            get { return _collider.Radius; }
            set { _collision.Parent.FireSetter(x => _collider.Radius = x, _collider.Radius, value); }
        }

        public Vector3 TotalPosition
        {
            get { return _collider.TotalPosition; }
        }

        public bool Trigger
        {
            get { return _collider.Trigger; }
            set { _collision.Parent.FireSetter(x => _collider.Trigger = x, _collider.Trigger, value); }
        }

        public BoundingSphere Sphere
        {
            get { return _collider.Sphere; }
            set { _collision.Parent.FireSetter(x => _collider.Sphere = x, _collider.Sphere, value); }
        }
        

        public SphereColliderWrapper(CollisionWrapper collision, SphereCollider collider) : base(collision)
        {
            _collider = collider;
        }

        public override string ToString()
        {
            return _collider.ToString();
        }
    }


    public class BoxColliderWrapper : ColliderWrapper
    {
        private readonly BoxCollider _collider;

        public BoxColliderWrapper(CollisionWrapper collision, BoxCollider collider) : base(collision)
        {
            _collider = collider;
        }

        [TypeConverter(typeof(Vector3ConverterEx))]
        public Vector3 LocalPosition
        {
            get { return _collider.LocalPosition; }
            set { _collision.Parent.FireSetter(x => _collider.LocalPosition = x, _collider.LocalPosition, value); }
        }

        public Vector3 TotalPosition
        {
            get { return _collider.TotalPosition; }
        }

        public bool Trigger
        {
            get { return _collider.Trigger; }
            set { _collision.Parent.FireSetter(x => _collider.Trigger = x, _collider.Trigger, value); }
        }

        public BoundingBox Box
        {
            get { return _collider.Box; }
            set { _collision.Parent.FireSetter(x => _collider.Box = x, _collider.Box, value); }
        }

        [TypeConverter(typeof(Vector3ConverterEx))]
        public Vector3 EdgesSize
        {
            get { return _collider.EdgesSize; }
            set { _collision.Parent.FireSetter(x => _collider.EdgesSize = x, _collider.EdgesSize, value); }
        }

        public override string ToString()
        {
            return _collider.ToString();
        }
    }

}