﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        public ParticleSystem StrongParticle { get; set; }
        public ParticleSystem ShieldParticle { get; set; }
        public BananaScript BananaAttack { get; set; }
        #endregion
        #region Private
        /// <summary>
        /// Get character turn (look direction) from walk direciton if right stick is floating (not pushed).
        /// </summary>
        private bool _syncAngles = true;

        private GameObject _attackTriggerObject;
        private Vector3 _baseAttackSpherePostion;
        private float _baseAttackSphereRadius;
        private bool _shieldActive;
        private bool _locked;
        private float _shieldManaTimer;
        private PostponeBuffer _postponeBuffer = new PostponeBuffer();
        private bool _goDown;
        private const string _swipeSound = "SwordSwipe";
        private const string _pushSound = "Push";
        private const string _hitSound = "SwordHit";

        #endregion
        #endregion

        #region Properties

        public Stat LastTargetedEnemyHp { get; set; }
        public string LastTargetedEnemyName { get; set; }
        public bool Locked
        {
            get { return _locked;  }
            set
            {
                if (_locked == value) return;
                _locked = value;
                if (value)
                {
                    _postponeBuffer = new PostponeBuffer();
                }
                else
                {
                    // handle postponed actions in locked-state buffer
                    UnleashBuffer(_postponeBuffer);
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

            _attackTriggerObject = new GameObject(gameObject.Scene);
            _attackTriggerObject.Tag = "Weapon";
            _attackTriggerObject.transform.Position = new Vector3(15.0f, 10.0f, 0.0f);
            _attackTriggerObject.parent = this.gameObject;

            _attackTriggerObject.collision = new Collision(_attackTriggerObject);
            _attackTriggerObject.collision.Static = false;
            _attackTriggerObject.collision.MainCollider = new SphereCollider(_attackTriggerObject.collision, 5.0f, true);
            _attackTriggerObject.collision.Enabled = false;

            _shieldActive = false;
            _baseAttackSpherePostion = new Vector3(15.0f,10.0f,0.0f);
            _baseAttackSphereRadius = 5.0f;

            if (gameObject.collision != null)
            {
                gameObject.collision.OnTrigger += GetHit;
                gameObject.collision.OnTrigger += OnGateTrigger;
            }
            
        }

        private void OnGateTrigger(object sender, ColArgs colArgs)
        {
            BoxCollider boxCollider = colArgs.EnemyCollider as BoxCollider;
            if (boxCollider != null && boxCollider.Owner.gameObject.Name.Contains("Gate_"))
            {
                Translator t = boxCollider.Owner.gameObject.GetComponent<Translator>();
                if (t != null)
                {
                    t.IsTriggered = true;
                }
                AudioSource audio = boxCollider.Owner.gameObject.audioSource;
                if (audio != null)
                {
                    audio.Play(0);
                }
            }
        }

        public void GetHit(Object o, ColArgs args)
        {
            if (args.EnemyCollider != null && (args.EnemyCollider.Owner.gameObject.Tag == "EnemyWeapon" || args.EnemyCollider.Owner.gameObject.Tag == "EnemyWeaponCB"))
            {
                GameObject obj = args.EnemyCollider.Owner.gameObject.parent;
                EnemyScript enemy = obj.GetComponent<EnemyScript>();
                if (enemy != null)
                {
                    if (enemy is NJChuckScript)
                    {
                        gameObject.animator.Ouch();
                        Locked = true; 
                        gameObject.animator.OnAnimationFinish += delegate { Locked = false; };
                    }
                    gameObject.audioSource.Play(enemy.GetHitSound());
                    if(_shieldActive)
                    {
                        Stats.Health.Decrease(enemy.DMG * (100 - Stats.ShieldAbsorption.Value)/100);
                    }
                    else
                    {
                        Stats.Health.Decrease(enemy.DMG);
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //_attackTriggerObject.collision.MainCollider.Draw();
        }

        public override void Initialize(bool editor)
        {
            base.Initialize(editor);
            Stats = new PlayerStatistics(300, 200, 110);
            Stats.Experience.OnLevelUp += delegate
            {
                gameObject.audioSource.Play("LevelUp");
            };

            if (editor)
            {
            }
            else
            {
                InputManager.Instance.OnTurn += CharacterRotation;
                InputManager.Instance.OnMove += CharacterTranslate;
                InputManager.Instance.OnButton += CharacterAction;
                InputManager.Instance.OnCheat += PlayerSucks;

                HUD.Instance.AssignPlayerScript(this);
            }
            //BananaAttack.Player = gameObject;
        }

        private void PlayerSucks()
        {
            Stats.EnergyPotions.Increase(20);
            Stats.HealthPotions.Increase(20);
            Stats.TalentPoints.Increase(2);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (gameObject.particleSystem != null && gameObject.particleSystem.Triggered)
            {
                Vector3 newDirectionFrom = new Vector3(LookVector.X - 0.3f, 0.0f, LookVector.Z - 0.3f);
                Vector3 newDirectionTo = new Vector3(LookVector.X + 0.3f, 0.0f, LookVector.Z + 0.3f);
                gameObject.GetComponent<ParticleSystem>().DirectionFrom = newDirectionFrom;
                gameObject.GetComponent<ParticleSystem>().DirectionTo = newDirectionTo;
            }

            if (_shieldActive)
            {
                ShieldParticle.gameObject.parent.transform.Rotation += new Vector3(0.0f, 400.0f * gameTime.ElapsedGameTime.Milliseconds / 1000.0f, 0.0f);
                ShieldParticle.gameObject.transform.Translate(0.0f, (_goDown? -3.0f : 3.0f)  * gameTime.ElapsedGameTime.Milliseconds / 1000.0f,0.0f);
                if (ShieldParticle.gameObject.transform.Position.Y >= 13.0f) _goDown = true;
                if (ShieldParticle.gameObject.transform.Position.Y <= 0.0f) _goDown = false;
                if (_shieldManaTimer > 1.0f)
                {
                    if(!Stats.Energy.TryDecrease(Stats.ShieldManaCost.Value))
                    {
                        _shieldActive = false;
                        ShieldParticle.Triggered = false;
                    }
                    _shieldManaTimer = _shieldManaTimer % 1.0f;
                }
                _shieldManaTimer += gameTime.ElapsedGameTime.Milliseconds/1000.0f;
            }
            else
            {
                if(StrongParticle != null)
                    StrongParticle.gameObject.transform.Rotation = Vector3.Zero;
            }
        }

        protected override void PlayDeathSound()
        {
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
                            if (Stats.Energy.TryDecrease(Stats.PushManaCost.Value))
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
                                gameObject.audioSource.Play("NoMana");
                            }
                        }
                        break;

                    case Buttons.LeftTrigger:
                        {            
                            Attack(AttackType.Shield);
                            gameObject.animator.OnTrigger += delegate
                            {
                                if (ShieldParticle != null)
                                {
                                    if (Stats.Energy.Value > Stats.ShieldManaCost.Value)
                                    {
                                        _shieldActive = !_shieldActive;
                                        ShieldParticle.Triggered = _shieldActive;
                                        _shieldManaTimer = 0.0f;
                                    }
                                }
                                InputManager.Instance.RumplePad(300f, 0.3f, 0.7f);
                            };
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
                            if (Stats.Energy.TryDecrease(Stats.ShootManaCost.Value))
                            {
                                Attack(AttackType.Ion);
                                gameObject.animator.OnTrigger += delegate
                                {
                                    if (BananaAttack != null)
                                    {
                                        BananaAttack.Direction = LookVector;
                                        BananaAttack.StartPosition = gameObject.transform.Position + (LookVector*3.0f);
                                        BananaAttack.Activated = true;
                                    }
                                    InputManager.Instance.RumplePad(150f, 0.2f, 0.8f);
                                };
                            }
                            else
                            {
                                gameObject.audioSource.Play("NoMana");
                            }
                        }
                        break;
                    case Buttons.B:
                        UsePotion(PotionType.Health);
                        break;
                    case Buttons.X:
                        UsePotion(PotionType.Energy);
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

            switch (attackType)
            {
                case AttackType.Quick:
                    gameObject.audioSource.Play(_swipeSound);
                    break;

                case AttackType.Strong:
                    gameObject.audioSource.Play("SwordStrong");
                    break;


            }
            gameObject.animator.Attack(attackType.ToString());
            gameObject.animator.OnAnimationFinish += () => Locked = false;
            gameObject.animator.OnTrigger += delegate
            {
                _attackTriggerObject.transform.Position = _baseAttackSpherePostion;
                _attackTriggerObject.collision.MainCollider.Radius = _baseAttackSphereRadius;
                switch (AttackEnum)
                {
                    case AttackType.Strong:
                        _attackTriggerObject.transform.Position = new Vector3(11.0f, 10.0f, 0.0f);
                        _attackTriggerObject.collision.MainCollider.Radius = 14.5f;
                        _attackTriggerObject.collision.UpdateDisablePositions();
                        StrongParticle.Enabled = true;
                        StrongParticle.Triggered = true;
                        break;
                    
                    case AttackType.Push:
                        _attackTriggerObject.collision.MainCollider.Radius = 12.0f;
                        _attackTriggerObject.collision.UpdateDisablePositions();
                        gameObject.audioSource.Play(_pushSound);
                        break;

                    case AttackType.Ion:
                        gameObject.audioSource.Play("BananaShot");
                        break;
                }
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

        private void UsePotion(PotionType potionType)
        {
            switch (potionType)
            {
                case PotionType.Health:
                    if (Stats.HealthPotions.TryDecrease(1) && Stats.Health.Value < Stats.Health.MaxValue)
                    {
                        Stats.Health.Increase((int)(Stats.Health.MaxValue * 0.2f));
                        gameObject.audioSource.Play("PotionUse");
                    }
                    break;
                case PotionType.Energy:
                    if (Stats.EnergyPotions.TryDecrease(1) && Stats.Energy.Value < Stats.Energy.MaxValue)
                    {
                        Stats.Energy.Increase((int)(Stats.Energy.MaxValue * 0.2f));
                        gameObject.audioSource.Play("PotionUse");
                    }
                    break;
            }
        }

        public override Component Copy(GameObject newOwner)
        {
            // developper doesn't get responsibility for unexpected behaviour
            return new PlayerScript(newOwner);
        }
        #endregion

        public string GetHitSound(AttackType type)
        {
            switch (type)
            {
                case AttackType.Quick: return _hitSound;
                case AttackType.Ion: return "IonShock";
            }
            return null;
        }
    }

    public enum AttackType
    {
        Quick,
        Strong,
        Shield,
        Push,
        Ion
    }

    public enum PotionType
    {
        Health,
        Energy
    }

}
