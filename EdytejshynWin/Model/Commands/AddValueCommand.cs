using System;

namespace Edytejshyn.Model.Commands
{
    public class AddValueCommand<T> : ICommand
    {
        private readonly string _name;
        private readonly Action<T> _addAction;
        private readonly Action<T> _removeAction;
        private readonly T _value;

        public AddValueCommand(string name, Action<T> addAction, Action<T> removeAction, T value)
        {
            _name = name;
            _addAction = addAction;
            _removeAction = removeAction;
            _value = value;
        }

        public bool AffectsData
        {
            get { return true; }
        }

        public string Description
        {
            get { return string.Format("Add {0}", _name); }
        }

        public string Message
        {
            get { return string.Format("{0} value: {1}", Description, _value); }
        }

        public void Do()
        {
            _addAction(_value);
        }

        public void Undo()
        {
            _removeAction(_value);
        }
    }
}