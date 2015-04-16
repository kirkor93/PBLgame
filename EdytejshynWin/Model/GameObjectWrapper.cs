using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Forms.VisualStyles;
using Edytejshyn.GUI;
using Edytejshyn.Logic;
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
        protected GameObject _gameObject;
        protected TransformWrapper _transform;
        protected RendererWrapper _renderer;
        protected List<GameObjectWrapper> _children = new List<GameObjectWrapper>();

        public event PropertyChangedEventHandler ChangedEvent;
        public delegate void SetterHandler(ICommand command);
        private event SetterHandler SetterEvent;
        #endregion

        #region Properties
     
        [Description("Name of the game object")]
        [Category("0.General")]
        public string Name
        {
            get { return _gameObject.Name; }
            set { FireSetter(x => _gameObject.Name = x, _gameObject.Name, value); }
        }


        [Description("Tag for the game object")]
        [Category("0.General")]
        public string Tag
        {
            get { return _gameObject.Tag; }
            set { FireSetter(x => _gameObject.Tag = x, _gameObject.Tag, value); }
        }

        [Category("Components")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public TransformWrapper Transform
        {
            get { return _transform; }
        }

        [Category("Components")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public RendererWrapper Renderer
        {
            get { return _renderer; }
            private set
            {
                _gameObject.renderer = (value == null) ? null : value.WrappedRenderer;
                _renderer = value;
            }
        }


        // ----- NON-BROWSEABLE ----- //

        /// <summary>
        /// Inside nut in wrapper (wrapped original GameObject)
        /// </summary>
        [Browsable(false)]
        public GameObject Nut
        {
            get { return _gameObject; }
        }

        /// <summary>
        /// Ugly and dreadful way of fast constant [O(1)] access to corresponding scene TreeViewNode.
        /// </summary>
        [Browsable(false)]
        public SceneTreeNode TreeViewNode { get; set; }

        /// <summary>
        /// Recursive enumerator through all descendants (children, grandchildren, etc.).
        /// </summary>
        [Browsable(false)]
        public IEnumerable<GameObjectWrapper> Descendants
        {
            get
            {
                foreach (GameObjectWrapper child in _children)
                {
                    yield return child;
                    foreach (GameObjectWrapper grand in child.Descendants)
                    {
                        yield return grand;
                    }
                }
            }
        }

        #endregion

        #region Methods
        public GameObjectWrapper(GameObject gameObject)
        {
            _gameObject = gameObject;
            if (_gameObject.transform != null) _transform = new TransformWrapper(this, _gameObject.transform);
            if (_gameObject.renderer  != null) _renderer  = new RendererWrapper(this, _gameObject.renderer);

            foreach (GameObject child in _gameObject.GetChildren())
            {
                _children.Add(GameObjectWrappingFactory.Wrap(child));
            }

        }

        public void FireSetter<T>(Action<T> setValue, T oldValue, T newValue, [CallerMemberName] string property = null)
        {
            if ((oldValue != null && oldValue.Equals(newValue)) || (oldValue == null && newValue == null)) return;
            setValue += delegate
            {
                if (ChangedEvent != null)
                    ChangedEvent(this, new PropertyChangedEventArgs(property));
            };
            if (SetterEvent != null)
                SetterEvent(new ChangeValueCommand<T>(string.Format("{0} of {1}", property, Name), setValue, oldValue, newValue));
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

        public void NewRenderer()
        {
            // TODO unstaticate
            EditorLogic logic = Program.UglyStaticLogic;
            Renderer renderer = new Renderer(_gameObject, logic.CurrentScene)
            {
                MyMesh   = logic.ResourceManager.Meshes[0],
                Material = logic.ResourceManager.Materials[0],
                MyEffect = logic.ResourceManager.ShaderEffects[0]
            };
            RendererWrapper rendererWrapper = new RendererWrapper(this, renderer);

            FireSetter(x => Renderer = x, _renderer, rendererWrapper, "Renderer");
        }

        public void RemoveRenderer()
        {
            FireSetter(x => Renderer = x, _renderer, null, "Renderer");   
        }



        public void Draw(IDrawerStrategy strategy, GameTime gameTime)
        {
            if (_renderer != null) _renderer.Draw(strategy, gameTime);
            foreach (GameObjectWrapper child in _children)
            {
                child.Draw(strategy, gameTime);
            }
        }

        public GameObjectWrapper[] GetWrappedChildren()
        {
            return _children.ToArray();
        }
        #endregion

    }


    public abstract class LightWrapper : GameObjectWrapper
    {
        [TypeConverter(typeof(Vector4Converter))]
        [Category("1.Light")]
        public Vector4 Color
        {
            get { return ((Light) _gameObject).Color; }
            set { FireSetter(x => ((Light)_gameObject).Color = x, ((Light)_gameObject).Color, value); }
        }

        protected LightWrapper(Light light) : base(light) { }
    }

    public class PointLightWrapper : LightWrapper
    {

        #region Properties

        [Category("2.Point Light")]
        public float Attenuation
        {
            get { return ((PointLight) _gameObject).Attenuation; }
            set { FireSetter(x => ((PointLight) _gameObject).Attenuation = x, ((PointLight) _gameObject).Attenuation, value); }
        }

        [Category("2.Point Light")]
        public float FallOff
        {
            get { return ((PointLight) _gameObject).FallOff; }
            set { FireSetter(x => ((PointLight) _gameObject).FallOff = x, ((PointLight) _gameObject).FallOff, value); }
        }
        #endregion 

        public PointLightWrapper(PointLight light) : base(light) { }
    }

    public class DirectionalLightWrapper : LightWrapper
    {

        #region Properties

        [Category("2.Directional Light")]
        public float Intensity
        {
            get { return ((MyDirectionalLight)_gameObject).Intensity; }
            set { FireSetter(x => ((MyDirectionalLight)_gameObject).Intensity = x, ((MyDirectionalLight)_gameObject).Intensity, value); }
        }
        #endregion

        public DirectionalLightWrapper(MyDirectionalLight light) : base(light) { }
    }

    /// <summary>
    /// Factory creating wrappers with highest possible level of inheritance.
    /// </summary>
    public static class GameObjectWrappingFactory
    {
        public static GameObjectWrapper Wrap(GameObject obj)
        {
            GameObjectWrapper wrapper;

            if (obj is PointLight) wrapper = new PointLightWrapper((PointLight)obj);
            else if (obj is MyDirectionalLight) wrapper = new DirectionalLightWrapper((MyDirectionalLight)obj);
            else wrapper = new GameObjectWrapper(obj);

            return wrapper;
        }
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
            set { _parent.FireSetter(x => _transform.Position = x, _transform.Position, value); }
        }

        [TypeConverter(typeof(Vector3ConverterEx))]
        public Vector3 Rotation
        {
            get { return _transform.Rotation; }
            set { _parent.FireSetter(x => _transform.Rotation = x, _transform.Rotation, value); }
        }

        [TypeConverter(typeof(Vector3ConverterEx))]
        public Vector3 Scale
        {
            get { return _transform.Scale; }
            set { _parent.FireSetter(x => _transform.Scale = x, _transform.Scale, value); }
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