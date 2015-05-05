using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Singleton;

namespace PBLgame.GamePlay
{
    class PlayerScript : Engine.Components.Component
    {
        #region Variables
        #region Public
        public PlayerStatistics Stats { get; private set; }
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
            Stats = new PlayerStatistics(100, 100);
            InputManager.Instance.OnTurn   += CharacterRotation;
            InputManager.Instance.OnMove   += CharacterTranslate;
            InputManager.Instance.OnButton += CharacterAction;
        }

        public override void Draw(GameTime gameTime)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        private void CharacterRotation(Object obj, MoveArgs args)
        {
            float angle = Convert.ToSingle(Math.Atan2(Convert.ToDouble(-args.AxisValue.Y), Convert.ToDouble(-args.AxisValue.X))); 
            
            if(angle - _angle != 0.0f)
            {
                //_gameObject.transform.Rotation = Vector3.Lerp(_gameObject.transform.Rotation,new Vector3(MathHelper.ToDegrees(angle), 0.0f, 0.0f),0.5f);
                _gameObject.transform.Rotation = new Vector3(MathHelper.ToDegrees(angle), 0, 0);
                _angle = angle;
            }
        }
        private void CharacterTranslate(Object o, MoveArgs e)
        {
//            e.AxisValue *= 0.01f;
            _gameObject.transform.Translate(e.AxisValue.X, e.AxisValue.Y, 0.0f);

        }

        private void CharacterAction(Object o, ButtonArgs button)
        {
            if (button.IsDown)
            {
                switch (button.ButtonName)
                {
                    case Buttons.LeftShoulder:
                        {
                            const int cost = 20;
                            if (Stats.Energy.TryDecrease(cost))
                            {
                                Console.WriteLine("telekinetic push");
                            }
                            else
                            {
                                Console.WriteLine("Not enough mana");
                            }
                        }
                        break;

                    case Buttons.LeftTrigger:
                        {
                            const int cost = 25;
                            if (Stats.Energy.TryDecrease(cost))
                            {
                                Console.WriteLine("telekinetic shield");
                            }
                            else
                            {
                                Console.WriteLine("Not enough mana");
                            }
                        }
                        break;

                    case Buttons.RightShoulder:
                        {
                            Console.WriteLine("quick attack");
                        }
                        break;

                    case Buttons.RightTrigger:
                        {
                            Console.WriteLine("strong attack");
                        }
                        break;
                }
                Console.WriteLine(Stats.ToString());
            }
        }

        public override Component Copy(GameObject newOwner)
        {
            // developper doesn't get responsibility for unexpected behaviour
            return new PlayerScript(newOwner);
        }

        #endregion
    }
}
