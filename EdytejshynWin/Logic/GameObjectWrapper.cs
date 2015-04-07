using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Edytejshyn.Logic.Commands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;

namespace Edytejshyn.Logic
{
    public class GameObjectWrapper
    {
        #region Variables
        private GameObject _gameObject;
        private readonly MainForm _mainForm;
        private TransformWrapper _transform;
        #endregion

        #region Properties
        [Browsable(false)]
        public event PropertyChangedEventHandler ChangedEvent;
     
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

        [Category("General")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public TransformWrapper Transform
        {
            get { return _transform; }
        }
        #endregion

        #region Methods
        public GameObjectWrapper(GameObject gameObject, MainForm mainForm)
        {
            _gameObject = gameObject;
            _mainForm = mainForm;
            if (_gameObject.transform != null)
                _transform = new TransformWrapper(this, _gameObject.transform);

        }

        public void FireChangedEvent<T>(Action<T> setValue, T oldValue, T newValue, [CallerMemberName] string property = null)
        {
            if (oldValue.Equals(newValue)) return;
            setValue += delegate
            {
                if (ChangedEvent != null)
                    ChangedEvent(this, new PropertyChangedEventArgs(property));
            };
            _mainForm.Logic.History.NewAction(new ChangeCommand<T>(string.Format("{0} of {1}", property, Name), setValue, oldValue, newValue));
        }

        public GameObject[] GetChildren()
        {
            return _gameObject.GetChildren();
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
                _parent.FireChangedEvent(x => _transform.Position = x, _transform.Position, value);
            }
        }

        [TypeConverter(typeof(Vector3ConverterEx))]
        public Vector3 Rotation
        {
            get { return _transform.Rotation; }
            set
            {
                _parent.FireChangedEvent(x => _transform.Rotation = x, _transform.Rotation, value);
            }
        }

        [TypeConverter(typeof(Vector3ConverterEx))]
        public Vector3 Scale
        {
            get { return _transform.Scale; }
            set
            {
                _parent.FireChangedEvent(x => _transform.Scale = x, _transform.Scale, value);
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