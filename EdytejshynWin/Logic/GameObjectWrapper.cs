using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Edytejshyn.Logic.Commands;
using PBLgame.Engine.GameObjects;

namespace Edytejshyn.Logic
{
    public class GameObjectWrapper
    {
        private GameObject _gameObject;
        private readonly MainForm _mainForm;

        public GameObjectWrapper(GameObject gameObject, MainForm mainForm)
        {
            _gameObject = gameObject;
            _mainForm = mainForm;
        }

        [Browsable(false)]
        public event PropertyChangedEventHandler ChangedEvent;

        private void FireChangedEvent<T>(Action<T> setValue, T oldValue, T newValue, [CallerMemberName] string property = null)
        {
            if (oldValue.Equals(newValue)) return;
            setValue += delegate
            {
                if (ChangedEvent != null)
                    ChangedEvent(this, new PropertyChangedEventArgs(property));
            };
            _mainForm.Logic.History.NewAction(new ChangeCommand<T>(string.Format("{0} of {1}", property, Name), setValue, oldValue, newValue));
        }

     
        [Description("Name of the game object")]
        [Category("General")]
        public string Name
        {
            get { return _gameObject.Name; }
            set
            {
                FireChangedEvent(x => _gameObject.Name = x, _gameObject.Name, value);
            }
        }


        [Description("Tag for the game object")]
        [Category("General")]
        public string Tag
        {
            get { return _gameObject.Tag; }
            set
            {
                FireChangedEvent(x => _gameObject.Tag = x, _gameObject.Tag, value);
            }
        }

        public GameObject[] GetChildren()
        {
            return _gameObject.GetChildren();
        }
    }
}