using System;
using System.Collections.Generic;
using System.Linq;

using PBLgame.Engine.Components;

namespace PBLgame.Engine.GameObjects
{
    public class GameObject
    {
        #region Variables
            #region Public
            public String Name;
            public String Tag;
            public int ID;
            #endregion
            #region Private
            //Components list attached to game object
            List<Component> _components = new List<Components.Component>();
            //Most common _components             
            private Transform _transform;
            private Renderer _renderer;
            private Collision _collision;
            private Animator _animator;
            private ParticleSystem _particleSystem;          
            #endregion
        #endregion

        #region Properties
        public Transform transform
        {
            get
            {
                return _transform;
            }
            set
            {
                _transform = value;
            }
        }
        public Renderer renderer
        {
            get
            {
                return _renderer;
            }
            set
            {
                _renderer = value;
            }
        }
        public Collision collision
        {
            get
            {
                return _collision;
            }
            set
            {
                _collision = value;
            }
        }
        public Animator animator
        {
            get
            {
                return _animator;
            }
            set
            {
                _animator = value;
            }
        }
        public ParticleSystem particleSystem
        {
            get
            {
                return _particleSystem;
            }
            set
            {
                _particleSystem = value;
            }
        }
        #endregion  

        #region Methods

        public GameObject()
        {
            _transform = new Components.Transform();
            _components.Add(_transform);
        }


        public T GetComponent<T>() where T : Component
        {
            IEnumerable<Component> list = 
                from component in _components
                where component.GetType() == typeof(T)
                select component;

            if (list.Any())
            {
                return list.First() as T;
            }

            return null;
        }
        #endregion
    }
}
