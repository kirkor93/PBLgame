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

        protected int _hp;
        protected int _maxHp;
        protected int _hpEscapeValue;

        protected float _affectDMGDelay;
        protected float _affectDMGTimer;

        protected bool _pushed;
        protected Vector3 _pushValue;
        protected float _pushTimer;

        protected float _attackTimer;

        protected bool _enemySeen = false;

        protected GameObject _attackTriggerObject;
        protected GameObject _fieldOfView;

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
            _attackTriggerObject = new GameObject
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

            _fieldOfView = new GameObject
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
            if (args.EnemyBox != null && args.EnemyBox.Owner.gameObject.Tag == "Player")
            {
                _enemySeen = true;
            }
            else if (args.EnemySphere != null && args.EnemySphere.Owner.gameObject.Tag == "Player")
            {
                _enemySeen = true;
            }
        }

        protected void GetHitMethod(Object o, ColArgs args)
        {
            PlayerScript player = null;
            if (args.EnemyBox != null && args.EnemyBox.Owner.gameObject.Tag == "Weapon")
            {
                player = args.EnemyBox.Owner.gameObject.parent.GetComponent<PlayerScript>();
                if (player != null)
                {
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
                            float biggest = Math.Abs(_pushValue.X);
                            if (_pushValue.Z > biggest) biggest = _pushValue.Z;
                            _pushValue /= (biggest * 3.0f);
                            _pushValue.Y = gameObject.transform.Position.Y;
                            _pushed = true;
                            _pushTimer = 0.0f;
                            break;
                        case AttackType.Ion:
                            break;
                    }
                    player.LastTargetedEnemyHp = new Stat(HP, MaxHp);
                }
            }
            else if (args.EnemySphere != null && args.EnemySphere.Owner.gameObject.Tag == "Weapon")
            {
                player = args.EnemySphere.Owner.gameObject.parent.GetComponent<PlayerScript>();
                if (player != null)
                {
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
                            float biggest = Math.Abs(_pushValue.X);
                            if (_pushValue.Z > biggest) biggest = _pushValue.Z;
                            _pushValue /= (biggest * 3.0f);
                            _pushValue.Y = gameObject.transform.Position.Y;
                            _pushed = true;
                            _pushTimer = 0.0f;
                            break;
                        case AttackType.Ion:
                            break;
                    }
                    player.LastTargetedEnemyHp = new Stat(HP, MaxHp);
                }
            }
            if (HP <= 0)
            {
                MakeDead(player);
            }
        }


        protected bool IsMyHP()
        {
            return _hp > _hpEscapeValue;
        }
    }
}