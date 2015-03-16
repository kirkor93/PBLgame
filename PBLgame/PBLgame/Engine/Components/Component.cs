using Microsoft.Xna.Framework;

namespace PBLgame.Engine.Components
{
    public abstract class Component
    {
        #region Variables
        private bool _enabled;
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
        #endregion
        #region Methods

        //void Initialize();
        //void Update();

        #endregion

    }
}
