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
            List<Engine.Components.Component> _components = new List<Components.Component>();
            //Most common _components             
            private Engine.Components.Component _transform;
            private Engine.Components.Component _renderer;
            private Engine.Components.Component _collision;
            private Engine.Components.Component _animator;
            private Engine.Components.Component _particleSystem;          
            #endregion
        #endregion

        #region Properties
        public Engine.Components.Component Transform
        {
            get
            {
                return this._transform;
            }   
        }
        public Engine.Components.Component Renderer
        {
            get
            {
                return this._renderer;
            }
        }
        public Engine.Components.Component Collision
        {
            get
            {
                return this._collision;
            }
        }
        public Engine.Components.Component Animator
        {
            get
            {
                return this._animator;
            }
        }
        public Engine.Components.Component ParticleSystem
        {
            get
            {
                return this._particleSystem;
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
