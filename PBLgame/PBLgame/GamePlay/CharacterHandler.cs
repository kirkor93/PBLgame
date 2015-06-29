using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using PBLgame.Engine;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Physics;
using PBLgame.Engine.Singleton;

namespace PBLgame.GamePlay
{
    public abstract class CharacterHandler : Component
    {
        #region Variables
        private float _destAngle;
        private Vector3 _lookVector;
        protected Avatar _avatar;

        #endregion

        #region Properties
        /// <summary>
        /// Unit velocity vector - max is when length == 1.
        /// </summary>
        public Vector2 UnitVelocity { get; set; }
        public float SpeedMultiplier { get; set; }
        public Vector3 LookVector 
        {
            get { return _lookVector; }
            private set{} 
        }
        /// <summary>
        /// Degrees per second
        /// </summary>
        public float RotationSpeed { get; set; }

        protected bool Dead;
        #endregion

        #region Methods

        protected CharacterHandler(GameObject gameObj) : base(gameObj)
        {
            UnitVelocity = Vector2.Zero;
            SpeedMultiplier = 100.0f;
            RotationSpeed = 360.0f;
            _destAngle = _gameObject.transform.Rotation.Y;
            _lookVector = new Vector3();
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float) gameTime.ElapsedGameTime.TotalSeconds;
            float currentAngle = _gameObject.transform.Rotation.Y;
            float deltaAngle = _destAngle - currentAngle;
            if (Math.Abs(deltaAngle) > 0.01)
            {
                if (currentAngle < 0.0f) currentAngle += 360.0f;
                if (deltaAngle > 180.0f) deltaAngle -= 360.0f;
                else if (deltaAngle < -180.0f) deltaAngle += 360.0f;

                float dAngle = RotationSpeed * seconds;
                if (deltaAngle < 0) dAngle = -dAngle;
                if (Math.Abs(dAngle) > Math.Abs(deltaAngle)) dAngle = deltaAngle;

                currentAngle += dAngle;
                _gameObject.transform.Rotation = new Vector3(0.0f, currentAngle, 0.0f);
            }
            Vector2 trueVelocity = UnitVelocity * SpeedMultiplier;
            Vector2 movement = trueVelocity * seconds;

            _gameObject.transform.Translate(movement.X, 0.0f, movement.Y);
            
            _avatar.Update(new Vector2(trueVelocity.X, -trueVelocity.Y), currentAngle);
        }

        public override void Initialize(bool editor)
        {
            if (_gameObject.audioSource == null) _gameObject.audioSource = new AudioSource(_gameObject);
            _avatar = Avatar.CreateAvatar(gameObject.animator);
        }

        /// <summary>
        /// Sets looking direction angle of character smoothly.
        /// </summary>
        /// <param name="direction">2D direction vector</param>
        public void SetLookVector(Vector2 direction)
        {
            _lookVector.X = direction.X;
            _lookVector.Z = -direction.Y;
            _lookVector *= 10000.0f;
            _lookVector.Normalize();
            float angle = Extensions.CalculateDegrees(direction);
            while (angle < 0) angle += 360.0f;
            _destAngle = angle;
        }

        /// <summary>
        /// Sets looking angle smoothly using 2D components from a 3D vector.
        /// Axes are converted from [+X][+Y][+Z] to [+X][-Z] to preserve ground plane orientation.
        /// </summary>
        /// <param name="direction">3D vector</param>
        public void SetLookVector(Vector3 direction)
        {
            _lookVector = direction;
            SetLookVector(new Vector2(direction.X, -direction.Z));
        }


        public void SetLookVectorInstantly(Vector3 direction)
        {
            _lookVector = direction;
            SetLookVectorInstantly(new Vector2(direction.X, -direction.Z));
        }

        public void SetLookVectorInstantly(Vector2 direction)
        {
            _lookVector.X = direction.X;
            _lookVector.Z = -direction.Y;
            _lookVector *= 10000.0f;
            _lookVector.Normalize();
            float angle = Extensions.CalculateDegrees(direction);
            while (angle < 0) angle += 360.0f;
            _destAngle = angle;
            _gameObject.transform.Rotation = new Vector3(gameObject.transform.Rotation.X, angle, gameObject.transform.Rotation.Z);
        }

        protected virtual void MakeDead(PlayerScript player)
        {
            if (Dead) return;
            Dead = true;
            gameObject.renderer.EmissiveValue = 0f;
            AttachSlot slot = gameObject.GetComponent<AttachSlot>();
            if (slot != null)
            {
                slot.Attachment.renderer.EmissiveValue = 0f;
            }
            gameObject.audioSource.Play("RobotDeath");
            gameObject.animator.Death();
            gameObject.collision.Enabled = false;
            gameObject.animator.OnAnimationFinish += delegate
            {
                ItemDropper.DropPotion(gameObject, player.gameObject);
            };
        }

        #endregion

    }
}