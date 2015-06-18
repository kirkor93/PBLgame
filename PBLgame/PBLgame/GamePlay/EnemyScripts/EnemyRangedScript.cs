using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using PBLgame.Engine.AI;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Physics;

namespace PBLgame.GamePlay
{
    public class EnemyRangedScript : CharacterHandler
    {
        #region Variables
        #region Enemy Vars
        public AIComponent AIComponent;
        public float ChaseSpeed = 0.005f;
        public float AttackRange = 45.0f;
        public float AttackAffectDelay;
        private float _attackDelay = 2500;

        private int _hp;
        private int _hpEscapeValue = 15;
        private Vector3 _startingPosition;
        private Vector3 _chaseStartPosition;

        private float _affectDMGDelay = 600.0f;
        private float _affectDMGTimer = 0.0f;
        private bool _attackFlag = false;

        private bool _pushed;
        private Vector3 _pushValue;
        private float _pushTimer;

        private float _attackTimer = 2000;

        private float _chaseTimer;

        private bool _enemySeen = false;

        private RangeAction _currentAction = RangeAction.Stay;

        private GameObject _attackTriggerObject;
        private GameObject _fieldOfView;

        #endregion
        #region DTNodes

        private DecisionNode _distanceNode = new DecisionNode();
        private DecisionNode _distance2Node = new DecisionNode();
        private DecisionNode _hpNode = new DecisionNode();
        private DecisionNode _canAttackNode = new DecisionNode();
        private ActionNode _attackNode = new ActionNode();
        private ActionNode _chaseNode = new ActionNode();
        private ActionNode _standNode = new ActionNode();
        private ActionNode _escapeNode = new ActionNode();
        private ActionNode _escapeNattackNode = new ActionNode();

        #endregion
        #endregion
        #region Methods
        public EnemyRangedScript(GameObject owner)
            : base(owner)
        {
            _hp = 100;
            MaxHp = _hp;

            _attackTriggerObject = new GameObject();
            _attackTriggerObject.Tag = "EnemyWeapon";
            _attackTriggerObject.transform.Position = new Vector3(0.0f, 10.0f, AttackRange);
            _attackTriggerObject.parent = this.gameObject;

            _attackTriggerObject.collision = new Collision(_attackTriggerObject);
            _attackTriggerObject.collision.Rigidbody = false;
            _attackTriggerObject.collision.Static = false;
            _attackTriggerObject.collision.MainCollider = new SphereCollider(_attackTriggerObject.collision, 5.0f, true);
            _attackTriggerObject.collision.Enabled = false;

            gameObject.collision.OnTrigger += GetHitMethod;

            _fieldOfView = new GameObject();
            _fieldOfView.Tag = "FOV";
            _fieldOfView.transform.Position = new Vector3(0.0f, 10.0f, 55.0f);
            _fieldOfView.parent = this.gameObject;

            _fieldOfView.collision = new Collision(_fieldOfView);
            _fieldOfView.collision.Rigidbody = false;
            _fieldOfView.collision.Static = false;
            _fieldOfView.collision.MainCollider = new SphereCollider(_fieldOfView.collision, 70.0f, true);
            _fieldOfView.collision.Enabled = true;

            _fieldOfView.collision.OnTrigger += GotPlayerMethod;

            _startingPosition = gameObject.transform.Position;
            _chaseTimer = 0.0f;

            #region DecisionTree & AiComponentInitialize

            _distanceNode.DecisionEvent += EnemyClose;
            _hpNode.DecisionEvent += IsMyHP;
            _canAttackNode.DecisionEvent += CanAtack;
            _distance2Node.DecisionEvent += EscapeOrChase;
            _attackNode.ActionEvent += AttackPlayer;
            _chaseNode.ActionEvent += GoToPlayer;
            _standNode.ActionEvent += StandStill;
            _escapeNode.ActionEvent += Escape;
            _escapeNattackNode.ActionEvent += EscapeNAttack;

            AIComponent = _gameObject.GetComponent<AIComponent>();
            if (AIComponent == null)
            {
                AIComponent = new AIComponent(owner);
                _gameObject.AddComponent<AIComponent>(AIComponent);
            }


            _distanceNode.TrueChild = _hpNode;
            _distanceNode.FalseChild = _standNode;

            _hpNode.TrueChild = _canAttackNode;
            _hpNode.FalseChild = _escapeNode;

            _canAttackNode.TrueChild = _attackNode;
            _canAttackNode.FalseChild = _distance2Node;

            _distance2Node.TrueChild = _escapeNattackNode;
            _distance2Node.FalseChild = _chaseNode;


            AIComponent.MyDTree.DTreeStart = _distanceNode;
            #endregion
        }

        public int HP
        {
            get { return _hp; }
            set { _hp = value; }
        }

        public int MaxHp { get; set; }

        public void GotPlayerMethod(Object o, ColArgs args)
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

        public void GetHitMethod(Object o, ColArgs args)
        {
            PlayerScript player = null;
            if (args.EnemyBox != null && args.EnemyBox.Owner.gameObject.Tag == "Weapon")
            {
                player = args.EnemyBox.Owner.gameObject.parent.GetComponent<PlayerScript>();
                if (player != null)
                {
                    Console.WriteLine(player.AttackEnum.ToString());
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
                            _pushValue /= (biggest * 3.0f) ;
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
                    Console.WriteLine(player.AttackEnum.ToString());
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
                            _pushValue /= (biggest * 3.0f) ;
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

        protected override void MakeDead(PlayerScript player)
        {
            if (player != null) player.Stats.Experience.Increase(100);
            _attackTriggerObject.Enabled = false;
            _fieldOfView.Enabled = false;
            base.MakeDead(player);
        }

        public override void Update(GameTime gameTime)
        {
            if (_hp > 0)
            {
                base.Update(gameTime);
                _attackTriggerObject.Update(gameTime);
                _fieldOfView.Update(gameTime);
                Vector3 dir;
                if (_pushed)
                {
                    _pushTimer += (gameTime.ElapsedGameTime.Milliseconds / 1000f);
                    _gameObject.transform.Position += _pushValue;
                    if (_pushTimer > 1.0f) _pushed = false;
                }
                else
                {
                    switch (_currentAction)
                    {
                        // TODO fix border points b/w attack & idle - now is flickering
                        case RangeAction.EscapeNAttack:
                            dir = gameObject.transform.Position - AISystem.Player.transform.Position;
                            Random rand = new Random();
                            int x = rand.Next(0, 100);
                            int y = rand.Next(0, 100);
                            if (_hp < _hpEscapeValue) SetLookVector(new Vector2(dir.Z, dir.X));
                            else SetLookVector(new Vector2(-dir.Z, -dir.X));
                            dir.X *= x / 100.0f;
                            dir.Z *= y / 100.0f;
                            UnitVelocity = new Vector2(dir.X, dir.Z) * 0.02f;
                            _attackTimer += gameTime.ElapsedGameTime.Milliseconds;
                            if (_attackTimer > _attackDelay)
                            {
                                _attackTimer = 0.0f;
                                //gameObject.animator.Attack();
                                _attackFlag = true;
                                _affectDMGTimer = 0.0f;
                            }
                            if(_attackFlag)
                            {
                                _affectDMGTimer += gameTime.ElapsedGameTime.Milliseconds;
                                if(_affectDMGTimer > _affectDMGDelay)
                                {
                                    _attackTriggerObject.collision.Enabled = true;
                                    foreach (GameObject go in PhysicsSystem.CollisionObjects)
                                    {
                                        if (_attackTriggerObject != go && go.collision.Enabled && _attackTriggerObject.collision.MainCollider.Contains(go.collision.MainCollider) != ContainmentType.Disjoint)
                                        {
                                            _attackTriggerObject.collision.ChceckCollisionDeeper(go);
                                        }
                                    }
                                    _attackTriggerObject.collision.Enabled = false;
                                    _attackFlag = false;
                                }
                            }
                            break;
                        case RangeAction.Attack:
                            dir = AISystem.Player.transform.Position - gameObject.transform.Position;
                            UnitVelocity = Vector2.Zero;
                            SetLookVector(new Vector2(dir.Z, dir.X));
                            _attackTimer += gameTime.ElapsedGameTime.Milliseconds;
                            if (_attackTimer > _attackDelay)
                            {
                                _attackTimer = 0.0f;
                                gameObject.animator.Attack();
                                _attackFlag = true;
                                _affectDMGTimer = 0.0f;
                            }
                            if(_attackFlag)
                            {
                                _affectDMGTimer += gameTime.ElapsedGameTime.Milliseconds;
                                if(_affectDMGTimer > _affectDMGDelay)
                                {
                                    _attackTriggerObject.collision.Enabled = true;
                                    foreach (GameObject go in PhysicsSystem.CollisionObjects)
                                    {
                                        if (_attackTriggerObject != go && go.collision.Enabled && _attackTriggerObject.collision.MainCollider.Contains(go.collision.MainCollider) != ContainmentType.Disjoint)
                                        {
                                            _attackTriggerObject.collision.ChceckCollisionDeeper(go);
                                        }
                                    }
                                    _attackTriggerObject.collision.Enabled = false;
                                    _attackFlag = false;
                                }
                            }
                            break;
                        case RangeAction.Chase:
                            _chaseTimer += gameTime.ElapsedGameTime.Milliseconds;
                            dir = AISystem.Player.transform.Position - gameObject.transform.Position;
                            SetLookVector(new Vector2(dir.Z, dir.X));
                            //Vector3 chaseDir = Vector3.Lerp(_chaseStartPosition, AISystem.Player.transform.Position, _chaseTimer * ChaseSpeed);
                            UnitVelocity = new Vector2(dir.X, dir.Z) * ChaseSpeed;
                            break;
                        case RangeAction.Escape:
                            dir = gameObject.transform.Position - AISystem.Player.transform.Position;
                            Random rand2 = new Random();
                            int x2 = rand2.Next(0, 100);
                            int y2 = rand2.Next(0, 100);
                            if (_hp < _hpEscapeValue) SetLookVector(new Vector2(dir.Z, dir.X));
                            else SetLookVector(new Vector2(-dir.Z, -dir.X));
                            dir.X *= x2 / 100.0f;
                            dir.Z *= y2 / 100.0f;
                            //gameObject.transform.Position += (new Vector3(dir.X, 0.0f, dir.Z) * 0.02f);
                            UnitVelocity = new Vector2(dir.X, dir.Z) * ChaseSpeed;
                            break;
                        case RangeAction.Stay:
                            UnitVelocity = Vector2.Zero;
                            break;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _attackTriggerObject.Draw(gameTime);
            _fieldOfView.Draw(gameTime);
        }

        public override Component Copy(GameObject newOwner)
        {
            return new EnemyRangedScript(newOwner);
        }

        private bool EnemyClose()
        {
            if (Vector3.Distance(gameObject.transform.Position, AISystem.Player.transform.Position) < 130.0f)
            {
                foreach (GameObject go in PhysicsSystem.CollisionObjects)
                {
                    if (_fieldOfView != go && go.collision.Enabled && _fieldOfView.collision.MainCollider.Contains(go.collision.MainCollider) != ContainmentType.Disjoint)
                    {
                        _fieldOfView.collision.ChceckCollisionDeeper(go);
                    }
                }
                return _enemySeen;
            }
            else return false;
        }

        private bool CanAtack()
        {
            float dist = Math.Abs(Vector3.Distance(gameObject.transform.Position, AISystem.Player.transform.Position));
            if (  dist > AttackRange -5 && dist < AttackRange + 5) return true;
            else return false;
        }

        private bool EscapeOrChase()
        {
            float dist = Math.Abs(Vector3.Distance(gameObject.transform.Position, AISystem.Player.transform.Position));
            if (dist + 5 > AttackRange) return false;
            return true;
        }
        
        private bool IsMyHP()
        {
            if (_hp > _hpEscapeValue) return true;
            else return false;
        }

        private void EscapeNAttack()
        {
            _currentAction = RangeAction.EscapeNAttack;
        }

        private void AttackPlayer()
        {
            _currentAction = RangeAction.Attack;
        }

        private void GoToPlayer()
        {
            _currentAction = RangeAction.Chase;
            _chaseTimer = 0.0f;
            _chaseStartPosition = gameObject.transform.Position;
        }

        private void StandStill()
        {
            _currentAction = RangeAction.Stay;
        }

        private void Escape()
        {
            _currentAction = RangeAction.Escape;
        }
        #endregion

        enum RangeAction
        {
            Chase = 0,
            Attack,
            Escape,
            Stay,
            EscapeNAttack
        }
    }

}

