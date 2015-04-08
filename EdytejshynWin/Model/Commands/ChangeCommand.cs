using System;

namespace Edytejshyn.Model.Commands
{
    public class ChangeCommand<T> : ICommand
    {
        private readonly string _name;
        private readonly Action<T> _setValue;
        private readonly T _oldValue;
        private readonly T _newValue;

        public ChangeCommand(string name, Action<T> setValue, T oldValue, T newValue)
        {
            _name = name;
            _setValue = setValue;
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public bool AffectsData
        {
            get { return true; }
        }

        public string Description
        {
            get { return string.Format("Change {0}", _name); }
        }

        public string Message
        {
            get { return string.Format("{0} from: {1} to: {2}", Description, _oldValue, _newValue); }
        }

        public void Do()
        {
            _setValue(_newValue);
        }

        public void Undo()
        {
            _setValue(_oldValue);
        }
    }
}