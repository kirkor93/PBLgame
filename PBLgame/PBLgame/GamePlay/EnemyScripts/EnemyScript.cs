using System;
using Microsoft.Xna.Framework;
using PBLgame.Engine.AI;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Physics;

namespace PBLgame.GamePlay
{
    public abstract class EnemyScript : CharacterHandler
    {
        #region Variables
        #region Enemy Vars

        public AIComponent AIComponent;
        public float ChaseSpeed;
        protected float _attackDelay = 2500;
        protected bool _attackFlag = false;

        protected int _dmg;
        protected int _hp;
        protected int _maxHp;
        protected int _hpEscapeValue;

        protected float _affectDMGDelay;
        protected float _affectDMGTimer;

        protected bool _ionFlag;
        protected float _ionTimer;

        protected bool _pushed;
        protected Vector3 _pushValue;
        protected float _pushTimer;

        protected float _attackTimer;

        protected bool _enemySeen = false;

        protected GameObject _attackTriggerObject;
        protected GameObject _fieldOfView;

        protected string _name;

        #endregion
        #endregion

        public int HP
        {
            get { return _hp; }
            set { _hp = value; }
        }

        public int MaxHp
        {
            get { return _maxHp; }
            set { _maxHp = value; }
        }

        public int DMG
        {
            get { return _dmg; }
            set { _dmg = value; }
        }

        protected EnemyScript(GameObject owner, int maxHp) : base(owner)
        {
            _maxHp = _hp = maxHp;

            AIComponent = _gameObject.GetComponent<AIComponent>();
            if (AIComponent == null)
            {
                AIComponent = new AIComponent(owner);
                _gameObject.AddComponent(AIComponent);
            }

            gameObject.collision.OnTrigger += GetHitMethod;
        }

        protected void SetupScript(Vector3 attackTriggerPos, float attackTriggerRadius)
        {
            SetupScript(attackTriggerPos, attackTriggerRadius, new Vector3(55.0f, 10.0f, 0.0f), 70.0f);
        }

        protected void SetupScript(Vector3 attackTriggerPos, float attackTriggerRadius, Vector3 fovPos, float fovRadius)
        {
            _attackTriggerObject = new GameObject(gameObject.Scene)
            {
                Tag = "EnemyWeapon",
                transform = { Position = attackTriggerPos },
                parent = gameObject
            };

            _attackTriggerObject.collision = new Collision(_attackTriggerObject)
            {
                Static = false,
                Rigidbody = false
            };
            _attackTriggerObject.collision.MainCollider = new SphereCollider(_attackTriggerObject.collision, attackTriggerRadius, true);
            _attackTriggerObject.collision.Enabled = false;

            _fieldOfView = new GameObject(gameObject.Scene)
            {
                Tag = "FOV",
                transform = { Position = fovPos },
                parent = gameObject
            };

            _fieldOfView.collision = new Collision(_fieldOfView)
            {
                Rigidbody = false,
                Static = false,
                Enabled = true
            };
            _fieldOfView.collision.MainCollider = new SphereCollider(_fieldOfView.collision, fovRadius, true);
            _fieldOfView.collision.OnTrigger += GotPlayerMethod;
        }

        protected void GotPlayerMethod(Object o, ColArgs args)
        {
            if (args.EnemyCollider != null && args.EnemyCollider.Owner.gameObject.Tag == "Player")
            {
                _enemySeen = true;
            }
        }

        protected virtual void GetHitMethod(Object o, ColArgs args)
        {
            PlayerScript player = null;
            CuteBomberScript cute = null;
            if (args.EnemyCollider != null && (args.EnemyCollider.Owner.gameObject.Tag == "Weapon" || args.EnemyCollider.Owner.gameObject.Tag == "EnemyWeaponCB"))
            {
                if (args.EnemyCollider.Owner.gameObject.parent != null) player = args.EnemyCollider.Owner.gameObject.parent.GetComponent<PlayerScript>();
                if (player == null && args.EnemyCollider.Owner.gameObject.Name == "Banana") player = args.EnemyCollider.Owner.gameObject.GetComponent<BananaScript>().Player.GetComponent<PlayerScript>();
                if (player == null && args.EnemyCollider.Owner.gameObject.parent != null)
                    cute = args.EnemyCollider.Owner.gameObject.parent.GetComponent<CuteBomberScript>();
                if (player != null)
                {
                    if (player.AttackEnum != AttackType.Ion || !_ionFlag)
                    {
                        gameObject.audioSource.Play(player.GetHitSound(player.AttackEnum));
                        MakeOuch();
                    }
                    switch (player.AttackEnum)
                    {
                        case AttackType.Quick:
                            _hp -= (player.Stats.BasePhysicalDamage.Value + player.Stats.FastAttackDamageBonus.Value);
                            break;
                        case AttackType.Strong:
                            _hp -= (player.Stats.BasePhysicalDamage.Value + player.Stats.StrongAttackDamageBonus.Value);
                            break;
                        case AttackType.Push:
                            _pushValue = gameObject.transform.Position - AISystem.Player.transform.Position;
                            _pushValue.Normalize();
                            _pushValue *= 3.0f;
                            _pushValue.Y = 0.0f;
                            _pushed = true;
                            _pushTimer = -0.3f;
                            break;
                        case AttackType.Ion:
                            if (!_ionFlag)
                            {
                                _hp -= (player.Stats.ShootDamage.Value);
                                _ionFlag = true;
                                _ionTimer = 0.0f;
                                SetLookVector(AISystem.Player.transform.Position - gameObject.transform.Position);
                            }
                            break;
                    }
                    player.LastTargetedEnemyHp = new Stat(HP, MaxHp);
                    player.LastTargetedEnemyName = _name;
                }
                else if(cute != null)
                {
                    _hp -= (cute.DMG);
                    _pushValue = gameObject.transform.Position - cute.gameObject.transform.Position;
                    _pushValue.Normalize();
                    _pushValue *= 1.5f;
                    _pushValue.Y = 0.0f;
                    _pushed = true;
                    _pushTimer = -0.3f;
                    MakeOuch();
                }
            }
            if (HP <= 0 && !Dead)
            {
                MakeDead(AISystem.Player.GetComponent<PlayerScript>());
            }
        }

        private void MakeOuch()
        {
            if (gameObject.animator == null || !gameObject.animator.Ouch()) return;
            DisableAI();
            StandStill();
            gameObject.animator.OnAnimationFinish += EnableAI;
        }

        protected virtual void EnableAI()
        {
            AIComponent.Enabled = true;
            gameObject.renderer.EmissiveValue = 1f;
        }

        protected virtual void DisableAI()
        {
            AIComponent.Enabled = false;
            gameObject.renderer.EmissiveValue = 0.1f;
        }

        protected override void MakeDead(PlayerScript player)
        {
            AIComponent.Enabled = false;
            base.MakeDead(player);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(_ionFlag)
            {
                _ionTimer += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                if (_ionTimer > 0.3f) _ionFlag = false;
            }
        }

        protected override void PlayDeathSound()
        {
            gameObject.audioSource.Play("RobotDeath");
        }

        protected bool IsMyHP()
        {
            return _hp > _hpEscapeValue;
        }

        protected abstract void StandStill();

        public virtual string GetHitSound()
        {
            return null;
        }
    }
}