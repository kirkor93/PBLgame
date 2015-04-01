using System;

using Microsoft.Xna.Framework;

using PBLgame.Engine.Components;
using PBLgame.Engine.Singleton;

namespace PBLgame.GamePlay
{
    class PlayerScript : Engine.Components.Component
    {
        #region Variables
        #region Public
        #endregion
        #region Private
        private float _angle;
        #endregion
        #endregion

        #region Properties
        #endregion

        #region Methods
        public PlayerScript(Engine.GameObjects.GameObject gameObj) : base(gameObj)
        {
           
        }

        public void Initialize()
        {
            InputManager.Instance.OnTurn += CharacterRotation;
            InputManager.Instance.OnMove += CharacterTranslate;
        }

        public override void Draw()
        {
            
        }

        public override void Update()
        {
            
        }

        private void CharacterRotation(Object obj, MoveArgs args)
        {
            float angle = Convert.ToSingle(Math.Atan2(Convert.ToDouble(-args.AxisValue.Y), Convert.ToDouble(-args.AxisValue.X))); 
            
            if(angle - _angle != 0.0f)
            {
                _gameObject.transform.Rotation = Vector3.Lerp(_gameObject.transform.Rotation,new Vector3(MathHelper.ToDegrees(angle), 0.0f, 0.0f),0.5f);
                _angle = angle;
            }
        }
        private void CharacterTranslate(Object o, MoveArgs e)
        {
            e.AxisValue *= 0.01f;
            _gameObject.transform.Translate(e.AxisValue.X, e.AxisValue.Y, 0.0f);

        }
        #endregion
    }
}
