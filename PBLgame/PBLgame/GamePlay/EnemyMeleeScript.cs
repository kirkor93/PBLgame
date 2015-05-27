using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using PBLgame.Engine.AI;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;

namespace PBLgame.GamePlay
{
    public class EnemyMeleeScript : Component
    {
        #region Variables
        #region Enemy Vars
        public AIComponent AIComponent;

        private int _hp = 100;

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
        public EnemyMeleeScript(GameObject owner) : base(owner)
        {
            _distanceNode.DecisionEvent += EnemyClose;
            _hpNode.DecisionEvent += IsMyHP;
            _canAttackNode.DecisionEvent += CanAtack;
            _attackNode.ActionEvent += AttackPlayer;
            _chaseNode.ActionEvent += GoToPlayer;
            _standNode.ActionEvent += StandStill;
            _escapeNode.ActionEvent += Escape;
            AIComponent = new AIComponent(owner);
            _gameObject.AddComponent<AIComponent>(AIComponent);


            _distanceNode.TrueChild = _hpNode;
            _distanceNode.FalseChild = _standNode;

            _hpNode.TrueChild = _canAttackNode;
            _hpNode.FalseChild = _escapeNode;

            _canAttackNode.TrueChild = _attackNode;
            _canAttackNode.FalseChild = _chaseNode;

            AIComponent.MyDTree.DTreeStart = _distanceNode;
         }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //switch(_currentAction)
            //{
            //    case MeleeAction.Attack:
            //        Console.WriteLine("Attack");
            //        break;
            //    case MeleeAction.Chase:
            //        Console.WriteLine("Chase");
            //        break;
            //    case MeleeAction.Escape:
            //        Console.WriteLine("Escape");
            //        break;
            //    case MeleeAction.Stay:
            //        Console.WriteLine("Stay");
            //        break;
            //}
        }

        private bool EnemyClose()
        {
            if (Vector3.Distance(gameObject.transform.Position, AISystem.Player.transform.Position) < 100.0f) return true;
            else return false;
        }

        private bool CanAtack()
        {
            if (Vector3.Distance(gameObject.transform.Position, AISystem.Player.transform.Position) < 20.0f) return true;
            else return false;
        }

        private bool IsMyHP()
        {
            if (_hp > 10.0f) return true;
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
