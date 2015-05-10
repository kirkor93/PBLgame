using System;
using Microsoft.Xna.Framework;
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
        public Vector2 Velocity { get; set; }
        public float SpeedMultiplier { get; set; }

        /// <summary>
        /// Degrees per second
        /// </summary>
        public float RotationSpeed { get; set; }
        #endregion

        #region Methods

        public CharacterHandler(GameObject gameObj) : base(gameObj)
        {
            Velocity = Vector2.Zero;
            SpeedMultiplier = 50.0f;
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
                
                _gameObject.transform.Rotation = new Vector3(0.0f, currentAngle + dAngle, 0.0f);
            }
            Vector2 movement = Velocity * SpeedMultiplier * seconds;
            _gameObject.transform.Translate(movement.X, 0.0f, movement.Y);

            // TODO handle animator
        }

        /// <summary>
        /// Sets looking direction angle of character smoothly.
        /// </summary>
        /// <param name="direction"></param>
        public void SetLookVector(Vector2 direction)
        {
            float angle = MathHelper.ToDegrees( Extensions.CalculateAngle(direction.X, direction.Y) );
            if (angle < 0) angle += 360.0f;
            _destAngle = angle;
        }



        #endregion
    }
}