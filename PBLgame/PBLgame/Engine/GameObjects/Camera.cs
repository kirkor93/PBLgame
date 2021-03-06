﻿using System;
using System.Dynamic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace PBLgame.Engine.GameObjects
{
    public class Camera : GameObject
    {
        #region Variables
        #region Private
        private Matrix _viewMatrix;
        private Matrix _projectionMatrix;
        private float _near;
        private float _far;
        private float _foV;
        private float _aspect;
        private Vector3 _direction;
        private const float _maxPitch = 1.5f;

        private AudioListener _listener;

        //static reference to first camera created for other classes
        private static Camera _mainCamera = null;
        

        #endregion
        #endregion

        #region Properites
        public AudioListener audioListener
        {
            get
            {
                _listener.Position = base.transform.Position;
                return _listener;
            }
            set
            {
                _listener = value;
            }
        }
        public Matrix ViewMatrix
        {
            get
            {
                _viewMatrix = Matrix.CreateLookAt(base.transform.Position, _direction + base.transform.Position, Vector3.Up);
                return _viewMatrix;
            }
        }
        public Matrix ProjectionMatrix
        {
            get
            {
                return _projectionMatrix;
            }
        }

        /// <summary>
        /// Sets or gets direction vector. Will be normalized.
        /// </summary>
        public Vector3 Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
                _direction.Normalize();
            }
        }

        public static Camera MainCamera 
        {
            get
            {
                return _mainCamera;
            }
            private set
            {
                _mainCamera = value;
            }
        }


        public static Matrix WideProjection { get; private set; }

        public float Near
        {
            get { return _near; }
        }

        public float Far
        {
            get { return _far; }
        }

        public float FoV
        {
            get { return _foV; }
        }

        public float Aspect
        {
            get { return _aspect; }
        }

        #endregion

        #region Methods
        public Camera(Vector3 pos, Vector3 target, Vector3 up,
            float FoV, float screenWidth, float screenHeight, float near, float far) : base(null)
        {
            base.transform.Position = pos;
            Direction = target - pos;

            _foV = FoV;
            _near = near;
            _far = far;
            _aspect = screenWidth / screenHeight;
            _viewMatrix = Matrix.CreateLookAt(base.transform.Position, _direction + pos, up);

            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                _foV,
                _aspect,
                _near, _far);

            //first camera created is main camera for game
            if (MainCamera == null)
            {
                MainCamera = this;
                WideProjection = Matrix.CreatePerspectiveFieldOfView(1.05f, _aspect, _near, _far);  // ~60 deg
            }

            _listener = new AudioListener();

        }

        public void SetTarget(Vector3 target)
        {
            Direction = target - transform.Position;
        }

        public void SetAspect(float screenWidth, float screenHeight)
        {
            _aspect = screenWidth / screenHeight;
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                _foV,
                _aspect,
                _near, _far
            );
        }

        public override void Update(GameTime gameTime = null)
        {

            //_viewMatrix = Matrix.CreateLookAt(base.transform.Position, _direction + base.transform.Position, Vector3.Up);
            if (this.parent != null) this.transform.Position = parent.transform.Position + new Vector3(0, 100f, 80f);
            
            _viewMatrix = ViewMatrix;
        }

        /// <summary>
        /// Rotates camera with desired yaw and clamped pitch for not flipping.
        /// Angles in radians.
        /// </summary>
        /// <param name="yaw">yaw angle rotation</param>
        /// <param name="pitch">pitch angle rotation</param>
        public void RotateYawPitch(float yaw, float pitch)
        {
            float currentYaw = (float) Math.Atan2(_direction.Z, _direction.X);

            float currentPitch = (float) Math.Asin(_direction.Y);
            float desiredPitch = currentPitch + pitch;
            
            if (desiredPitch > _maxPitch || desiredPitch < -_maxPitch)
            {
                desiredPitch = currentPitch;
            }
            double newYaw   = currentYaw - yaw;
            double newPitch = desiredPitch;

            Direction = new Vector3(
                (float) (Math.Cos(newPitch) * Math.Cos(newYaw)), 
                (float)  Math.Sin(newPitch), 
                (float) (Math.Cos(newPitch) * Math.Sin(newYaw))
            );

            // not working fully as desired:
            //Direction = Vector3.Transform(Direction, Matrix.CreateFromYawPitchRoll(yaw, pitch, 0f));
        }

        #endregion
    }
}
