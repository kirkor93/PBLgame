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
        public AIComponent AIComponent;

        private int _hp = 100;

        private bool _chase;
        private bool _attack;
        private bool _escape;
        private bool stay;

        private DecisionNode _distanceNode = new DecisionNode();
        private DecisionNode _hpNode = new DecisionNode();
        private DecisionNode _canAttackNode = new DecisionNode();
        private ActionNode _attackNode = new ActionNode();
        private ActionNode _chaseNode = new ActionNode();
        private ActionNode _standNode = new ActionNode();
        private ActionNode _escapeNode = new ActionNode();
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

        public void Update()
        {

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
            Console.WriteLine("Attack");
        }

        private void GoToPlayer()
        {
            Console.WriteLine("Chase");
        }

        private void StandStill()
        {
            Console.WriteLine("Stand");
        }

        private void Escape()
        {
            Console.WriteLine("Escape");
        }
#endregion


        //class IsEnemyNear : DecisionNode
        //{

        //}

        //class IsMyHPHigh : DecisionNode
        //{

        //}

        //class IsInAttackRange : DecisionNode
        //{

        //}

        //class Attack : ActionNode
        //{

        //}

        //class Chase : ActionNode
        //{

        //}

        //class Stand : ActionNode
        //{

        //}

        //class RunAway : ActionNode
        //{

        //}
    }

}
