using Microsoft.Xna.Framework;

namespace PBLgame.Engine.Singleton
{
    public class Singleton<T> where T : new()
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if(_instance==null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }
    }
}
