using System;
using System.Collections.Generic;
using System.Linq;
using PBLgame.Engine.Component;

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
            List<Engine.Component.IComponent> _components = new List<Component.IComponent>();
            //Most common _components             
            private Engine.Component.IComponent _transform;
            private Engine.Component.IComponent _renderer;
            private Engine.Component.IComponent _collision;
            private Engine.Component.IComponent _animator;
            private Engine.Component.IComponent _particleSystem;          
            #endregion
        #endregion

        #region Properties
        public Engine.Component.IComponent Transform
        {
            get
            {
                return this._transform;
            }   
        }
        public Engine.Component.IComponent Renderer
        {
            get
            {
                return this._renderer;
            }
        }
        public Engine.Component.IComponent Collision
        {
            get
            {
                return this._collision;
            }
        }
        public Engine.Component.IComponent Animator
        {
            get
            {
                return this._animator;
            }
        }
        public Engine.Component.IComponent ParticleSystem
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
            _transform = new Transform();
            _components.Add(_transform);
        }


        public T GetComponent<T>() where T : class, IComponent
        {
            IEnumerable<IComponent> list = 
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
