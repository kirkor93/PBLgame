using System;
using Microsoft.Xna.Framework;
using PBLgame.Engine.AI;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Physics;

namespace PBLgame.GamePlay
{
    public class CuteBomberScript : EnemyScript
    {
        #region Variables
        #region Enemy Vars
        public float AttackRange = 25.0f;
        private Vector3 _chaseStartPosition;
        private float _chaseTimer;

        private MeleeAction _currentAction = MeleeAction.Stay;

        #endregion
        #region DTNodes

        private DecisionNode _distanceNode = new DecisionNode();
        private DecisionNode _canAttackNode = new DecisionNode();
        private ActionNode _attackNode = new ActionNode();
        private ActionNode _chaseNode = new ActionNode();
        private ActionNode _standNode = new ActionNode();

        #endregion
        #endregion

        #region Methods
        public CuteBomberScript(GameObject owner) : base(owner, 30)
        {
            SetupScript(new Vector3(0.0f, 10.0f, 0.0f), AttackRange);
            ChaseSpeed = 0.0015f;
            _attackTimer = 1000.0f;
            _attackDelay = 2500;
            _hpEscapeValue = 15;

            _dmg = 10;
            #region DecisionTree & AiComponentInitialize

            _distanceNode.DecisionEvent += EnemyClose;
            _canAttackNode.DecisionEvent += CanAtack;
            _attackNode.ActionEvent += AttackPlayer;
            _chaseNode.ActionEvent += GoToPlayer;
            _standNode.ActionEvent += StandStill;

            _distanceNode.TrueChild = _canAttackNode;
            _distanceNode.FalseChild = _standNode;

            _canAttackNode.TrueChild = _attackNode;
            _canAttackNode.FalseChild = _chaseNode;

            AIComponent.MyDTree.DTreeStart = _distanceNode;
            #endregion
        }
        
        protected override void MakeDead(PlayerScript player)
        {
            if (player != null) player.Stats.Experience.Increase(100);
            _attackTriggerObject.Enabled = false;
            _fieldOfView.Enabled = false;
            gameObject.renderer.Enabled = false;
            gameObject.collision.Enabled = false;
            _attackTriggerObject.Enabled = false;
            _fieldOfView.Enabled = false;
            _hp = 0;
            
            //base.MakeDead(player); no animations here
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
                    if(_pushTimer > 0.0f)
                    {
                        _gameObject.transform.Position += _pushValue;
                        _pushValue.X *= (1.0f - _pushTimer);
                        _pushValue.Z *= (1.0f - _pushTimer);
                        if (_pushTimer > 1.0f) _pushed = false;
                    }
                }
                else
                {
                    switch (_currentAction)
                    {
                        case MeleeAction.Attack:
                            dir = AISystem.Player.transform.Position - gameObject.transform.Position;
                            SetLookVector(dir);
                            _attackTimer += gameTime.ElapsedGameTime.Milliseconds;
                            if (_attackTimer > _attackDelay)
                            {
                                _attackTriggerObject.collision.Enabled = true;
                                _attackTimer = 0.0f;
                                foreach (GameObject go in PhysicsSystem.CollisionObjects)
                                {
                                    if (_attackTriggerObject != go && go.collision.Enabled && _attackTriggerObject.collision.MainCollider.Contains(go.collision.MainCollider) != ContainmentType.Disjoint)
                                    {
                                        _attackTriggerObject.collision.ChceckCollisionDeeper(go);
                                    }
                                }
                                gameObject.GetComponent<ParticleSystem>().Triggered = true;
                                gameObject.renderer.Enabled = false;
                                gameObject.collision.Enabled = false;
                                _attackTriggerObject.Enabled = false;
                                _fieldOfView.Enabled = false;
                                _hp = 0;
                            }
                            break;
                        case MeleeAction.Chase:
                            _chaseTimer += gameTime.ElapsedGameTime.Milliseconds;
                            dir = AISystem.Player.transform.Position - gameObject.transform.Position;
                            SetLookVector(dir);
                            gameObject.transform.Position = Vector3.Lerp(_chaseStartPosition, AISystem.Player.transform.Position, _chaseTimer * ChaseSpeed);
                            break;
                        case MeleeAction.Stay:
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
            return new EnemyMeleeScript(newOwner);
        }

        private bool EnemyClose()
        {
            if (Vector3.Distance(gameObject.transform.Position, AISystem.Player.transform.Position) < 100.0f)
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
            if (Vector3.Distance(gameObject.transform.Position, AISystem.Player.transform.Position) < AttackRange - 5.0f) return true;
            else return false;
        }

        private void AttackPlayer()
        {
            _currentAction = MeleeAction.Attack;
        }

        private void GoToPlayer()
        {
            _currentAction = MeleeAction.Chase;
            _chaseTimer = 0.0f;
            _chaseStartPosition = gameObject.transform.Position;
        }

        private void StandStill()
        {
            _currentAction = MeleeAction.Stay;
        }
        #endregion

        enum MeleeAction
        {
            Chase = 0,
            Attack,
            Stay
        }
    }

}

