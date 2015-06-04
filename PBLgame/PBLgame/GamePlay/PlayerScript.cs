using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PBLgame.Engine;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.GUI;
using PBLgame.Engine.Singleton;
using PBLgame.Engine.Physics;

namespace PBLgame.GamePlay
{
    class PlayerScript : CharacterHandler
    {
        private class PostponeBuffer
        {
            public MoveArgs Rotation { get; private set; }
            public bool HasRotation { get; private set; }
            
            public MoveArgs Translation { get; private set; }
            public bool HasTranslation { get; private set; }

            private List<ButtonArgs> _buttons = new List<ButtonArgs>();
            public List<ButtonArgs> Buttons { get { return _buttons; } }

            public void SetRotation(MoveArgs args)
            {
                HasRotation = true;
                Rotation = args;
            }

            public void SetTranslation(MoveArgs args)
            {
                HasTranslation = true;
                Translation = args;
            }

            public void AddButton(ButtonArgs args)
            {
                _buttons.Add(args);
            }
        }

        #region Variables
        #region Public
        public PlayerStatistics Stats { get; private set; }
        #endregion
        #region Private
        /// <summary>
        /// Get character turn (look direction) from walk direciton if right stick is floating (not pushed).
        /// </summary>
        private bool _syncAngles = true;

        private Bar _healthBar;
        private Bar _manaBar;
        private Bar _experienceBar;

        private GameObject _attackTriggerObject;
        private bool _locked;
        private PostponeBuffer _postponeBuffer = new PostponeBuffer();

        #endregion
        #endregion

        #region Properties

        public bool Locked
        {
            get { return _locked;  }
            set
            {
                _locked = value;
                if (value == false)
                {
                    // handle postponed actions in locked-state buffer
                    UnleashBuffer(_postponeBuffer);
                }
                else
                {
                    _postponeBuffer = new PostponeBuffer();
                }
            }
        }

        private void UnleashBuffer(PostponeBuffer buffer)
        {
            if(buffer.HasRotation) CharacterRotation(this, buffer.Rotation);
            if(buffer.HasTranslation) CharacterTranslate(this, buffer.Translation);
            foreach (ButtonArgs button in buffer.Buttons)
            {
                CharacterAction(this, button);
            }
        }

        #endregion

        #region Methods

        public PlayerScript(GameObject gameObj) : base(gameObj)
        {
            SpeedMultiplier = 70f;

            _attackTriggerObject = new GameObject();
            _attackTriggerObject.Tag = "Weapon";
            _attackTriggerObject.transform.Position = new Vector3(15.0f, 10.0f, 0.0f);
            _attackTriggerObject.parent = this.gameObject;

            _attackTriggerObject.collision = new Collision(_attackTriggerObject);
            _attackTriggerObject.collision.Static = false;
            _attackTriggerObject.collision.MainCollider = new SphereCollider(_attackTriggerObject.collision, 5.0f, true);
        }

        public override void Draw(GameTime gameTime)
        {
            _attackTriggerObject.Draw(gameTime);
        }

        public override void Initialize(bool editor)
        {
            base.Initialize(editor);
            if (editor)
            {
            }
            else
            {
                Stats = new PlayerStatistics(100, 100, 100);
                InputManager.Instance.OnTurn += CharacterRotation;
                InputManager.Instance.OnMove += CharacterTranslate;
                InputManager.Instance.OnButton += CharacterAction;

                //getting controls from gui
                _healthBar = HUD.Instance.GetGuiObject("Health_bar") as Bar;
                _manaBar = HUD.Instance.GetGuiObject("Mana_bar") as Bar;
                _experienceBar = HUD.Instance.GetGuiObject("Experience_bar") as Bar;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _attackTriggerObject.Update(gameTime);
            if(_healthBar != null)
            {
                _healthBar.FillAmount = ConvertRange(Convert.ToSingle(Stats.Health.Value), 0.0f, Stats.Health.MaxValue, 0.0f, 1.0f);
            }
            if (_manaBar != null)
            {
                _manaBar.FillAmount = ConvertRange(Convert.ToSingle(Stats.Energy.Value), 0.0f, Stats.Energy.MaxValue, 0.0f, 1.0f);
            }
            if (_experienceBar != null)
            {
                _experienceBar.FillAmount = ConvertRange(Convert.ToSingle(Stats.Experience.Value), 0.0f, Stats.Experience.MaxValue, 0.0f, 1.0f);
            }
        }

        private float ConvertRange(float value, float oldMin, float oldMax, float newMin, float newMax)
        {
            return ((value - oldMin) / (oldMax - oldMin)) * (newMax - newMin) + newMin;
        }

        private void CharacterRotation(Object obj, MoveArgs args)
        {
            if (Locked)
            {
                // postpone actions
                _postponeBuffer.SetRotation(args);
                return;
            }

            if (args.AxisValue.LengthSquared() < 0.01)
            {
                _syncAngles = true;
            }
            else
            {
                _syncAngles = false;
                SetLookVector(args.AxisValue);
            }
        }


        private void CharacterTranslate(Object o, MoveArgs args)
        {
            if (Locked)
            {
                // postpone actions
                _postponeBuffer.SetTranslation(args);
                return;
            }
            UnitVelocity = new Vector2(args.AxisValue.X, -args.AxisValue.Y);
            if (_syncAngles && args.AxisValue.LengthSquared() > 1e-5f)
            {
                SetLookVector(args.AxisValue);
            }
        }


        private void CharacterAction(Object o, ButtonArgs button)
        {
            if (Locked)
            {
                // e.g. drinking mana or coffee
                //_postponeBuffer.AddButton()
                return;
            }

            if (button.IsDown)
            {
                switch (button.ButtonName)
                {
                    case Buttons.LeftShoulder:
                        {
                            const int cost = 1;
                            if (Stats.Energy.TryDecrease(cost))
                            {
                                Console.WriteLine("telekinetic push");
                                InputManager.Instance.RumplePad(200, 1, 0.5f);
                            }
                            else
                            {
                                Console.WriteLine("Not enough mana");
                            }
                        }
                        break;

                    case Buttons.LeftTrigger:
                        {
                            const int cost = 3;
                            if (Stats.Energy.TryDecrease(cost))
                            {
                                Console.WriteLine("telekinetic shield");
                                InputManager.Instance.RumplePad(300, 0.3f, 0.7f);
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
                            Locked = true;
                            _postponeBuffer.SetTranslation(new MoveArgs(new Vector2(UnitVelocity.X, -UnitVelocity.Y)));
                            UnitVelocity = Vector2.Zero;

                            gameObject.animator.Attack();
                            gameObject.animator.OnAnimationFinish += () => Locked = false;
                            
                        }
                        break;

                    case Buttons.RightTrigger:
                        {
                            Console.WriteLine("strong attack");
                            InputManager.Instance.RumplePad(200, 1, 1);
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
