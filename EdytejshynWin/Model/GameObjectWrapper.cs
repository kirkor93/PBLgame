using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Edytejshyn.Model.Commands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;

namespace Edytejshyn.Model
{
    [DefaultProperty("Name")]
    public class GameObjectWrapper
    {
        #region Variables
        private GameObject _gameObject;
        private TransformWrapper _transform;
        private List<GameObjectWrapper> _children = new List<GameObjectWrapper>();

        public event PropertyChangedEventHandler ChangedEvent;
        public delegate void SetterHandler(ICommand command);
        private event SetterHandler SetterEvent;
        #endregion

        #region Properties
     
        [Description("Name of the game object")]
        [Category("General")]
        public string Name
        {
            get { return _gameObject.Name; }
            set
            {
                FireSetter(x => _gameObject.Name = x, _gameObject.Name, value);
            }
        }


        [Description("Tag for the game object")]
        [Category("General")]
        public string Tag
        {
            get { return _gameObject.Tag; }
            set
            {
                FireSetter(x => _gameObject.Tag = x, _gameObject.Tag, value);
            }
        }

        [Category("Components")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public TransformWrapper Transform
        {
            get { return _transform; }
        }

        [Category("Components")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public TransformWrapper Renderer
        {
            get { return null; }
        }
        #endregion

        #region Methods
        public GameObjectWrapper(GameObject gameObject)
        {
            _gameObject = gameObject;
            if (_gameObject.transform != null)
                _transform = new TransformWrapper(this, _gameObject.transform);
            foreach (GameObject child in _gameObject.GetChildren())
            {
                _children.Add(new GameObjectWrapper(child));
            }

        }

        public void FireSetter<T>(Action<T> setValue, T oldValue, T newValue, [CallerMemberName] string property = null)
        {
            if (oldValue.Equals(newValue)) return;
            setValue += delegate
            {
                if (ChangedEvent != null)
                    ChangedEvent(this, new PropertyChangedEventArgs(property));
            };
            if (SetterEvent != null)
                SetterEvent(new ChangeCommand<T>(string.Format("{0} of {1}", property, Name), setValue, oldValue, newValue));
        }

        public void AttachSetterHandler(SetterHandler handler)
        {
            SetterEvent += handler;
            foreach (GameObjectWrapper child in _children)
            {
                child.AttachSetterHandler(handler);
            }
        }

        public void DetachSetterHandler(SetterHandler handler)
        {
            SetterEvent -= handler;
            foreach (GameObjectWrapper child in _children)
            {
                child.DetachSetterHandler(handler);
            }
        }


        public GameObject[] GetChildren()
        {
            return _gameObject.GetChildren();
        }

        public GameObjectWrapper[] GetWrappedChildren()
        {
            return _children.ToArray();
        }
        #endregion

    }

    public class TransformWrapper
    {
        #region Variables
        private GameObjectWrapper _parent;
        private Transform _transform;
        #endregion

        #region Properties
        [TypeConverter(typeof(Vector3ConverterEx))]
        public Vector3 Position
        {
            get { return _transform.Position; }
            set
            {
                _parent.FireSetter(x => _transform.Position = x, _transform.Position, value);
            }
        }

        [TypeConverter(typeof(Vector3ConverterEx))]
        public Vector3 Rotation
        {
            get { return _transform.Rotation; }
            set
            {
                _parent.FireSetter(x => _transform.Rotation = x, _transform.Rotation, value);
            }
        }

        [TypeConverter(typeof(Vector3ConverterEx))]
        public Vector3 Scale
        {
            get { return _transform.Scale; }
            set
            {
                _parent.FireSetter(x => _transform.Scale = x, _transform.Scale, value);
            }
        }
        #endregion

        #region Methods
        public TransformWrapper(GameObjectWrapper parent, Transform transform)
        {
            _parent = parent;
            _transform = transform;
        }

        public override string ToString()
        {
            return "";
        }
        #endregion
    }

    /// <summary>
    /// Extends standard converter so that can convert single float into Vector3 with equal coordinates.
    /// </summary>
    public class Vector3ConverterEx : Vector3Converter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = value as string;
            if (str != null)
            {
                try
                {
                    float coord = float.Parse(str, culture);
                    return new Vector3(coord);
                }
                catch (FormatException)
                {
                    try
                    {
                        float coord = float.Parse(str, CultureInfo.InvariantCulture);
                        return new Vector3(coord);
                    }
                    catch (FormatException)
                    {
                        // catch'em all
                    }
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

}