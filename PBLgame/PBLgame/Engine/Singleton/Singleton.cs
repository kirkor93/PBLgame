using Microsoft.Xna.Framework;

namespace PBLgame.Engine.Singleton
{
    public abstract class Singleton<T>
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if(_instance==null)
                {
                    _instance = default(T);
                }
                return _instance;
            }
        }
    }
}
