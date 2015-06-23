﻿using System;
using Microsoft.Xna.Framework;
using PBLgame.Engine.AI;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Physics;

namespace PBLgame.GamePlay
{
    public class NJChuckScript : EnemyScript
    {
        #region Variables
        #region Enemy Vars

        private string _attackType = string.Empty;

        private MeleeAction _currentAction = MeleeAction.Stay;
        
        #endregion
        #region DTNodes

        private DecisionNode _distanceNode = new DecisionNode();
        private DecisionNode _hpNode = new DecisionNode();
        private DecisionNode _canAttackNode = new DecisionNode();
        private ActionNode _attackNode = new ActionNode();
        private ActionNode _chaseNode = new ActionNode();
        private ActionNode _standNode = new ActionNode();
        private ActionNode _escapeNode = new ActionNode();

        #endregion
        #endregion

        #region Methods
        public NJChuckScript(GameObject owner) : base(owner, 400)
        {
            SetupScript(new Vector3(15.0f, 10.0f, 0.0f), 5.0f);
            ChaseSpeed = 0.003f;
            _attackTimer = 2000;
            _attackDelay = 2500;
            _affectDMGDelay = 500.0f;
            _hpEscapeValue = 15;

            _dmg = 20;
            
            #region DecisionTree & AiComponentInitialize
            _distanceNode.DecisionEvent += EnemyClose;
            _hpNode.DecisionEvent += IsMyHP;
            _canAttackNode.DecisionEvent += CanAtack;
            _attackNode.ActionEvent += AttackPlayer;
            _chaseNode.ActionEvent += GoToPlayer;
            _standNode.ActionEvent += StandStill;
            _escapeNode.ActionEvent += Escape;

            _distanceNode.TrueChild = _hpNode;
            _distanceNode.FalseChild = _standNode;

            _hpNode.TrueChild = _canAttackNode;
            _hpNode.FalseChild = _escapeNode;

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
            base.MakeDead(player);
        }

        public override void Update(GameTime gameTime)
        {
            Random rand = new Random();
            if (_hp > 0)
            {
                base.Update(gameTime);
                _attackTriggerObject.Update(gameTime);
                _fieldOfView.Update(gameTime);
                Vector3 dir;
                if (_pushed)
                {
                    _pushTimer += (gameTime.ElapsedGameTime.Milliseconds / 1000f);
                    if (_pushTimer > 0.0f)
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
                            UnitVelocity = Vector2.Zero;
                            dir = AISystem.Player.transform.Position - gameObject.transform.Position;
                            SetLookVector(dir);
                            _attackTimer += gameTime.ElapsedGameTime.Milliseconds;
                            if (_attackTimer > _attackDelay)
                            {
                                _attackTimer = 0.0f;
                                _attackType = (rand.NextDouble()) > 0.3 ? "Basic" : "Strong";
                                gameObject.animator.Attack(_attackType);
                                gameObject.animator.OnTrigger += delegate
                                {
                                    // TODO these two look quite the same
                                    if (_attackType == "Basic")
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
                                    }
                                    else if (_attackType == "Strong")
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
                                    }
                                };
                            }
                            break;
                        case MeleeAction.Chase:
                            dir = AISystem.Player.transform.Position - gameObject.transform.Position;
                            SetLookVector(dir);
                            UnitVelocity = new Vector2(dir.X, dir.Z) * ChaseSpeed;
                            break;
                        case MeleeAction.Escape:
                            dir = gameObject.transform.Position - AISystem.Player.transform.Position;
                            SetLookVector(dir);
                            UnitVelocity = new Vector2(dir.X, -dir.Z) * ChaseSpeed;
                            break;
                        case MeleeAction.Stay:
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
            if (Vector3.Distance(gameObject.transform.Position, AISystem.Player.transform.Position) < 20.0f) return true;
            else return false;
        }

        private void AttackPlayer()
        {
            _currentAction = MeleeAction.Attack;
        }

        private void GoToPlayer()
        {
            _currentAction = MeleeAction.Chase;
        }

        private void StandStill()
        {
            _currentAction = MeleeAction.Stay;
        }

        private void Escape()
        {
            _currentAction = MeleeAction.Escape;
        }
        #endregion

        enum MeleeAction
        {
            Chase = 0,
            Attack,
            Escape,
            Stay
        }
    }
}

