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
    public class PlayerScript : CharacterHandler
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
        public AttackType AttackEnum { get; private set; }
        #endregion
        #region Private
        /// <summary>
        /// Get character turn (look direction) from walk direciton if right stick is floating (not pushed).
        /// </summary>
        private bool _syncAngles = true;

        private GameObject _attackTriggerObject;
        private bool _locked;
        private PostponeBuffer _postponeBuffer = new PostponeBuffer();

        #endregion
        #endregion

        #region Properties

        public Stat LastTargetedEnemyHp { get; set; }
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
            SpeedMultiplier = 60f;

            _attackTriggerObject = new GameObject();
            _attackTriggerObject.Tag = "Weapon";
            _attackTriggerObject.transform.Position = new Vector3(15.0f, 10.0f, 0.0f);
            _attackTriggerObject.parent = this.gameObject;

            _attackTriggerObject.collision = new Collision(_attackTriggerObject);
            _attackTriggerObject.collision.Static = false;
            _attackTriggerObject.collision.MainCollider = new SphereCollider(_attackTriggerObject.collision, 5.0f, true);
            _attackTriggerObject.collision.Enabled = false;

            if (gameObject.collision != null)
            {
                gameObject.collision.OnTrigger += GetHit;
                gameObject.collision.OnTrigger += OnGateTrigger;
            }
        }

        private void OnGateTrigger(object sender, ColArgs colArgs)
        {
            if (colArgs.EnemyBox != null && colArgs.EnemyBox.Owner.gameObject.Name.Contains("Gate_"))
            {
                Translator t = colArgs.EnemyBox.Owner.gameObject.GetComponent<Translator>();
                if (t != null)
                {
                    t.IsTriggered = true;
                }
            }
        }

        public void GetHit(Object o, ColArgs args)
        {
            //if (args.EnemyBox != null) Console.WriteLine(args.EnemyBox.Owner.gameObject.Tag);
            //if (args.EnemySphere != null) Console.WriteLine(args.EnemySphere.Owner.gameObject.Tag);

            if (args.EnemyBox != null && args.EnemyBox.Owner.gameObject.Tag == "EnemyWeapon")
            {
                if(args.EnemyBox.Owner.gameObject.parent.GetComponent<EnemyMeleeScript>() != null) Stats.Health.Decrease(5);
                else if (args.EnemyBox.Owner.gameObject.parent.GetComponent<EnemyRangedScript>() != null) Stats.Health.Decrease(3);
                else if (args.EnemyBox.Owner.gameObject.parent.GetComponent<NJChuckScript>() != null) Stats.Health.Decrease(15);
                else if (args.EnemyBox.Owner.gameObject.parent.GetComponent<CuteBomberScript>() != null) Stats.Health.Decrease(7);
            }
            else if (args.EnemySphere != null && args.EnemySphere.Owner.gameObject.Tag == "EnemyWeapon")
            {
                if (args.EnemySphere.Owner.gameObject.parent.GetComponent<EnemyMeleeScript>() != null) Stats.Health.Decrease(5);
                else if (args.EnemySphere.Owner.gameObject.parent.GetComponent<EnemyRangedScript>() != null) Stats.Health.Decrease(3);
                else if (args.EnemySphere.Owner.gameObject.parent.GetComponent<NJChuckScript>() != null) Stats.Health.Decrease(15);
                else if (args.EnemySphere.Owner.gameObject.parent.GetComponent<CuteBomberScript>() != null) Stats.Health.Decrease(7);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _attackTriggerObject.Draw(gameTime);
        }

        public override void Initialize(bool editor)
        {
            base.Initialize(editor);
            Stats = new PlayerStatistics(100, 100, 100);
            if (editor)
            {
            }
            else
            {
                InputManager.Instance.OnTurn += CharacterRotation;
                InputManager.Instance.OnMove += CharacterTranslate;
                InputManager.Instance.OnButton += CharacterAction;

                HUD.Instance.AssignPlayerScript(this);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _attackTriggerObject.Update(gameTime);
            if (gameObject.particleSystem != null && gameObject.particleSystem.Triggered)
            {
                Vector3 newDirectionFrom = new Vector3(LookVector.X - 0.3f, 0.0f, LookVector.Z - 0.3f);
                Vector3 newDirectionTo = new Vector3(LookVector.X + 0.3f, 0.0f, LookVector.Z + 0.3f);
                gameObject.GetComponent<ParticleSystem>().DirectionFrom = newDirectionFrom;
                gameObject.GetComponent<ParticleSystem>().DirectionTo = newDirectionTo;
            }
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
                                Attack(AttackType.Push);
                                if (gameObject.particleSystem != null)
                                {
                                    gameObject.animator.OnTrigger += delegate
                                    {
                                        Vector3 newDirectionFrom = new Vector3(LookVector.X - 0.3f, 0.0f, LookVector.Z - 0.3f);
                                        Vector3 newDirectionTo = new Vector3(LookVector.X + 0.3f, 0.0f, LookVector.Z + 0.3f);
                                        ParticleSystem sys = gameObject.GetComponent<ParticleSystem>();
                                        sys.DirectionFrom = newDirectionFrom;
                                        sys.DirectionTo = newDirectionTo;
                                        sys.Enabled = true;
                                        sys.Triggered = true;
                                        InputManager.Instance.RumplePad(200f, 1f, 0.5f);
                                    };
                                }
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
                                Attack(AttackType.Shield);
                                gameObject.animator.OnTrigger += delegate
                                {
                                    InputManager.Instance.RumplePad(300f, 0.3f, 0.7f);
                                };
                            }
                            else
                            {
                                Console.WriteLine("Not enough mana");
                            }
                        }
                        break;

                    case Buttons.RightShoulder:
                        {
                            Attack(AttackType.Quick);
                        }
                        break;

                    case Buttons.RightTrigger:
                        {
                            Attack(AttackType.Strong);
                            gameObject.animator.OnTrigger += delegate
                            {
                                InputManager.Instance.RumplePad(150f, 1f, 1f);
                            };
                        }
                        break;

                    case Buttons.RightShoulder | Buttons.LeftShoulder:
                        {
                            Attack(AttackType.Ion);
                            gameObject.animator.OnTrigger += delegate
                            {
                                InputManager.Instance.RumplePad(150f, 0.2f, 0.8f);
                            };
                        }
                        break;
                }
                //Console.WriteLine(Stats.ToString());
            }
        }

        private void Attack(AttackType attackType)
        {
            AttackEnum = attackType;
            Locked = true;
            _postponeBuffer.SetTranslation(new MoveArgs(new Vector2(UnitVelocity.X, -UnitVelocity.Y)));
            UnitVelocity = Vector2.Zero;

            gameObject.animator.Attack(attackType.ToString());
            gameObject.animator.OnAnimationFinish += () => Locked = false;
            gameObject.animator.OnTrigger += delegate
            {
                _attackTriggerObject.collision.Enabled = true;
                foreach (GameObject go in PhysicsSystem.CollisionObjects)
                {
                    if (_attackTriggerObject != go && go != gameObject.parent && go.collision.Enabled && _attackTriggerObject.collision.MainCollider.Contains(go.collision.MainCollider) != ContainmentType.Disjoint)
                    {
                        _attackTriggerObject.collision.ChceckCollisionDeeper(go);
                    }
                }
                _attackTriggerObject.collision.Enabled = false;
            };

        }

        public override Component Copy(GameObject newOwner)
        {
            // developper doesn't get responsibility for unexpected behaviour
            return new PlayerScript(newOwner);
        }
        #endregion
    }

    public enum AttackType
    {
        Quick,
        Strong,
        Shield,
        Push,
        Ion
    }

}
