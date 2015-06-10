using System;
using System.Collections.Generic;

namespace Edytejshyn.Model.Commands
{
    public class AddToListCommand<T> : ICommand
    {
        private readonly string _name;
        private readonly IList<T> _list;
        private readonly T _value;
        private readonly Action _actionAfter;

        public AddToListCommand(string name, IList<T> list, T value, Action actionAfter = null)
        {
            _name = name;
            _list = list;
            _value = value;
            _actionAfter = actionAfter;
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
            _list.Add(_value);
            if (_actionAfter != null) _actionAfter();
        }

        public void Undo()
        {
            _list.Remove(_value);
            if (_actionAfter != null) _actionAfter();
        }
    }

    public class RemoveFromListCommand<T> : ICommand
    {
        private readonly string _name;
        private readonly IList<T> _list;
        private readonly T _value;
        private readonly Action _actionAfter;
        private readonly int _index;

        public RemoveFromListCommand(string name, IList<T> list, T value, Action actionAfter = null)
        {
            _name = name;
            _list = list;
            _value = value;
            _actionAfter = actionAfter;
            _index = _list.IndexOf(_value);
        }

        public bool AffectsData
        {
            get { return true; }
        }

        public string Description
        {
            get { return string.Format("Remove {0}", _name); }
        }

        public string Message
        {
            get { return string.Format("{0} value: {1}", Description, _value); }
        }

        public void Do()
        {
            if(_index >= 0) _list.RemoveAt(_index);
            if(_actionAfter != null) _actionAfter();
        }

        public void Undo()
        {
           if(_index >= 0) _list.Insert(_index, _value);
           if (_actionAfter != null) _actionAfter();
        }
    }
}