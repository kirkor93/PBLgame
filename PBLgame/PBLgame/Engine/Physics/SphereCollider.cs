using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PBLgame.Engine.Physics
{
    public class SphereCollider
    {
        #region Variables
        private Vector3 _position;
        private float _radius;
        private bool _trigger;
        private BoundingSphere _sphere = new BoundingSphere();
        #endregion

        #region Properties
        public Vector3 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
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
        public SphereCollider()
        {
            _position = Vector3.Zero;
            _radius = 0.0f;
            _trigger = false;
            _sphere = new BoundingSphere(_position,_radius);
        }

        public void InitializeSphere(Vector3 pos)
        {
            _sphere = new BoundingSphere(pos + _position, _radius);
        }

        public void RealPosition(Vector3 pos)
        {
            _sphere.Center = pos + _position;
        }

        public void Update()
        {

        }

        
        public void Draw()
        {

        }
        #endregion 
    }
}
