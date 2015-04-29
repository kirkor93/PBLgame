using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PBLgame.Engine.Components;

namespace PBLgame.Engine.Physics
{
    public class SphereCollider
    {
        #region Variables
        private Collision _owner;
        private Vector3 _localPosition;
        private Vector3 _totalPosition;
        private float _radius;
        private bool _trigger;
        private BoundingSphere _sphere = new BoundingSphere();

        private Vector3 _previousPosition;
        #endregion

        #region Properties
        public Collision Owner
        {
            get
            {
                return _owner;
            }
            private set { }
        }

        public Vector3 LocalPosition
        {
            get
            {
                return _localPosition;
            }
            set
            {
                _localPosition = value;
                _totalPosition = _owner.gameObject.transform.Position + _localPosition;
                if (_owner.gameObject.parent != null) _totalPosition += _owner.gameObject.transform.AncestorsPosition;
                _sphere.Center = _totalPosition;
            }
        }
        public float Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = value;
                _sphere.Radius = _radius;
            }
        }
        public Vector3 TotalPosition
        {
            get
            {
                return _totalPosition;
            }
            private set { }
        }
        public Vector3 PreviousPosition
        {
            get
            {
                return _previousPosition;
            }
            private set{ }
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
        public BoundingSphere Sphere
        {
            get
            {
                return _sphere;
            }
            set
            {
                _sphere = value;
            }
        }
        #endregion

        #region Methods
        public SphereCollider(Collision owner)
        {
            _owner = owner;
            _localPosition = Vector3.Zero;
            _totalPosition = owner.gameObject.transform.Position + _localPosition;
            if (owner.gameObject.parent != null) _totalPosition += owner.gameObject.transform.AncestorsPosition;
            _previousPosition = Vector3.Zero;
            _radius = 0.0f;
            _trigger = false;
            _sphere = new BoundingSphere(_totalPosition,_radius);
        }

        public SphereCollider(Collision owner, Vector3 position, float radius, bool trigger)
        {
            _previousPosition = Vector3.Zero;
            _radius = radius;
            _localPosition = position;
            _totalPosition = _localPosition + owner.gameObject.transform.Position;
            if (owner.gameObject.parent != null) _totalPosition += owner.gameObject.transform.AncestorsPosition;
            _trigger = trigger;
            _sphere = new BoundingSphere(TotalPosition, Radius);
        }

        public SphereCollider(Collision owner, float radius, bool trigger)
        {
            _previousPosition = Vector3.Zero;
            _radius = radius;
            _localPosition = Vector3.Zero;
            _totalPosition = _localPosition + owner.gameObject.transform.Position;
            if (owner.gameObject.parent != null) _totalPosition += owner.gameObject.transform.AncestorsPosition;
            _trigger = trigger;
            _sphere = new BoundingSphere(TotalPosition, Radius);
        }

        public bool Intersect(SphereCollider sphere)
        {
            return _sphere.Intersects(sphere.Sphere);
        }

        public ContainmentType Contains(SphereCollider sphere)
        {
            return _sphere.Contains(sphere.Sphere);
        }

        public void UpdatePosition()
        {
            _previousPosition = _totalPosition;
            _totalPosition = _owner.gameObject.transform.Position + _localPosition;
            if (_owner.gameObject.parent != null) _totalPosition += _owner.gameObject.transform.AncestorsPosition;
            _sphere.Center = _totalPosition;
        }

        public void Update()
        {
        //    _previousPosition = _totalPosition;
        //    _totalPosition = _owner.gameObject.transform.Position + _localPosition;
        //    if (_owner.gameObject.parent != null) _totalPosition += _owner.gameObject.transform.AncestorsPosition;
        //    _sphere.Center = _totalPosition;
        }

        
        public void Draw()
        {

        }
        #endregion 
    }
}
