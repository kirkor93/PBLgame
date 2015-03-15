using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.GameObject
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
            List<Engine.Component.IComponent> components = new List<Component.IComponent>();
            //Most common components             
            private Engine.Component.IComponent transform;
            private Engine.Component.IComponent renderer;
            private Engine.Component.IComponent collision;
            private Engine.Component.IComponent animator;
            private Engine.Component.IComponent particleSystem;          
            #endregion
        #endregion

        #region Properties
        public Engine.Component.IComponent Transform
        {
            get
            {
                return this.transform;
            }   
        }
        public Engine.Component.IComponent Renderer
        {
            get
            {
                return this.renderer;
            }
        }
        public Engine.Component.IComponent Collision
        {
            get
            {
                return this.collision;
            }
        }
        public Engine.Component.IComponent Animator
        {
            get
            {
                return this.animator;
            }
        }
        public Engine.Component.IComponent ParticleSystem
        {
            get
            {
                return this.particleSystem;
            }
        }
        #endregion  

        #region Methods
        public Engine.Component.IComponent GetComponent<T>(ref T component )
        {
            return new Engine.Component.IComponent();
        }
        #endregion
    }
}
