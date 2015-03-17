using System;
using System.Collections.Generic;
using System.Linq;

using PBLgame.Engine.Components;

namespace PBLgame.Engine.GameObject
{
    class GameObject
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
            private Component _transform;
            private Component _renderer;
            private Component _collision;
            private Component _animator;
            private Component _particleSystem;          
            #endregion
        #endregion

        #region Properties
        public Component Transform
        {
            get
            {
                return _transform;
            }   
        }
        public Component Renderer
        {
            get
            {
                return _renderer;
            }
        }
        public Component Collision
        {
            get
            {
                return _collision;
            }
        }
        public Component Animator
        {
            get
            {
                return _animator;
            }
        }
        public Component ParticleSystem
        {
            get
            {
                return _particleSystem;
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
