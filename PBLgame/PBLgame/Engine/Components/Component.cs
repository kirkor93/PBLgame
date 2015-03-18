using Microsoft.Xna.Framework;
using PBLgame.Engine.GameObjects;

namespace PBLgame.Engine.Components
{
    public abstract class Component
    {
        #region Variables
        protected bool _enabled;
        protected GameObject _gameObject;
        #endregion
        #region Properties
        public bool Enabled
        {
            get 
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }

        public GameObject GameObject
        {
            get
            {
                return _gameObject;
            }
        }
        #endregion
        #region Methods

        protected Component()
        {
            
        }

        protected Component(GameObject owner)
        {
            _gameObject = owner;
        }

        //void Initialize();
        //void Update();

        #endregion

    }
}
