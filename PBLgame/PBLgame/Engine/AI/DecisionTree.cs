using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PBLgame.Engine.AI
{
    public delegate void ActionDelegate();
    public delegate bool DecisionDelegate();

    public class DecisionTree
    {
        private DTNode _dTreeStart;

        public DecisionTree()
        {
        }

        public DecisionTree(DTNode start)
        {
            _dTreeStart = start;
        }

        public DTNode DTreeStart
        {
            get { return _dTreeStart; }
            set { _dTreeStart = value; }
        }

        public void ExecuteTree()
        {
            if (DTreeStart != null)
            {
                _dTreeStart.ExecuteNode();
            }
        }
    }

    public abstract class DTNode
    {
        public virtual void ExecuteNode()
        {
            Console.WriteLine("Executed abstract ExectueNodeMethod");
        }
    }

    public class ActionNode : DTNode
    {
        public event ActionDelegate ActionEvent;

        public override void ExecuteNode()
        {
            if (ActionEvent != null) ActionEvent();
        }
    }

    public class DecisionNode : DTNode
    {
        public event DecisionDelegate DecisionEvent;

        private DTNode _trueChild;
        private DTNode _falseChild;

        public DTNode TrueChild
        {
            get { return _trueChild; }
            set { _trueChild = value; }
        }

        public DTNode FalseChild
        {
            get { return _falseChild; }
            set { _falseChild = value; }
        }

        public DecisionNode()
        {
            TrueChild = null;
            FalseChild = null;
        }

        public DecisionNode(DecisionNode truee, DecisionNode falsee)
        {
            TrueChild = truee;
            FalseChild = falsee;
        }

        public override void ExecuteNode()
        {
            if (TrueChild == null && FalseChild == null)
            {
                Console.WriteLine("Decision Node Bug");
                return;
            }
            bool flag = false;
            if (DecisionEvent != null) flag = DecisionEvent();
            if (flag) TrueChild.ExecuteNode();
            else FalseChild.ExecuteNode();
        }
    }
}
