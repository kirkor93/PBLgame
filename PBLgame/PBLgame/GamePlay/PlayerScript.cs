using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using PBLgame.Engine.Components;
using PBLgame.Engine.Singleton;

namespace PBLgame.GamePlay
{
    class PlayerScript : Engine.Components.Component
    {
        #region Variables
        #region Private
        private float angle;
        #endregion
        #endregion
        #region Methods
        public PlayerScript(Engine.GameObjects.GameObject gameObj) : base(gameObj)
        {
           
        }

        public void Initialize()
        {
            InputManager.Instance.OnTurn += CharacterRotation;
            InputManager.Instance.OnMove += PlayerTranslate;
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {

        //Unity Code
        //if(Input.GetAxis("AxisX") != 0 || -Input.GetAxis("AxisY")!= 0)
        //{
        //    float angleTmp = Mathf.Atan2(Input.GetAxis("AxisX") , -Input.GetAxis("AxisY"));
        //    angleTmp = angleTmp * 180.0f / Math.PI;
        //    angleTmp = Mathf.Round((angleTmp * 10.0f) / 10.0f);
        //    if(angle!=angleTmp)
        //    {
        //        transform.rotation = Quaternion.AngleAxis(angleTmp, transform.up);
        //        angle = angleTmp;
        //    }
        //}
        }

        void CharacterRotation(Object obj, MoveArgs args)
        {
            float angleTMp = Convert.ToSingle(Math.Atan2(Convert.ToDouble(args.AxisValue.X), Convert.ToDouble(args.AxisValue.Y)));
            angleTMp = angleTMp * 180.0f / Convert.ToSingle(Math.PI);
            angleTMp = Convert.ToSingle(Math.Round((angleTMp * 10.0f) / 10.0f));
            if(angle != angleTMp)
            {
                _gameObject.transform.Rotate(angleTMp, 0.0f, 0.0f);
            }
            Console.WriteLine(_gameObject.transform.Rotation.ToString());

        }
        void PlayerTranslate(Object o, MoveArgs e)
        {
            e.AxisValue *= 0.01f;
            _gameObject.transform.Translate(e.AxisValue.X, e.AxisValue.Y, 0.0f);

            Console.WriteLine(_gameObject.transform.Position.ToString());
        }
        #endregion
    }
}
