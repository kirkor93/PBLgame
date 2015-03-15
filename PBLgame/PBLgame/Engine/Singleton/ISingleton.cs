using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Singleton
{
    public class ISingleton
    {
        protected static ISingleton _instance = null;

        public static ISingleton Instance
        {
            get
            {
                if(_instance==null)
                {
                    _instance = new ISingleton();
                }
                return _instance;
            }
        }

        protected ISingleton()
        {

        }
    }
}
