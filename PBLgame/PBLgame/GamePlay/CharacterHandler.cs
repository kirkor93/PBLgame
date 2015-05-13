using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using PBLgame.Engine;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;

namespace PBLgame.GamePlay
{
    public class CharacterHandler : Component
    {
        #region Variables
        private float _destAngle;
        #endregion

        #region Properties
        /// <summary>
        /// Unit velocity vector - max is when length == 1.
        /// </summary>
        public Vector2 UnitVelocity { get; set; }
        public float SpeedMultiplier { get; set; }

        /// <summary>
        /// Degrees per second
        /// </summary>
        public float RotationSpeed { get; set; }

        /// <summary>
        /// Angle added to the one calculated from velocity / look to achieve correct direction.
        /// TODO REMOVE THIS ASAP
        /// </summary>
        protected float AngleCorrection = 0f;
        #endregion

        #region Methods

        public CharacterHandler(GameObject gameObj) : base(gameObj)
        {
            UnitVelocity = Vector2.Zero;
            SpeedMultiplier = 100.0f;
            RotationSpeed = 360.0f;
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float) gameTime.ElapsedGameTime.TotalSeconds;
            float currentAngle = _gameObject.transform.Rotation.Y;
            float deltaAngle = _destAngle - _gameObject.transform.Rotation.Y;
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

            float v = trueVelocity.Length();
            if (Math.Abs(v) < 0.0001)
            {
                _gameObject.animator.Idle();
            }
            else
            {
//                if (Math.Abs(currentAngle - Extensions.CalculateDegrees(trueVelocity)) > 90f)
//                {
//                    v = -v;
//                }
                _gameObject.animator.Walk(v);
            }
            
        }

        public override void Initialize()
        {
            _gameObject.animator.Idle();
        }

        /// <summary>
        /// Sets looking direction angle of character smoothly.
        /// </summary>
        /// <param name="direction"></param>
        public void SetLookVector(Vector2 direction)
        {
            float angle = Extensions.CalculateDegrees(direction);
            angle += AngleCorrection;   // TODO solve better
            if (angle < 0) angle += 360.0f;
            _destAngle = angle;
        }


        #endregion
    }
}