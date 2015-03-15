using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine.Singleton
{
    public class InputManager : Engine.Singleton.ISingleton
    {
        private static InputManager _instance;

        public static InputManager Instance
        {
            get
            {
                if(_instance==null)
                {
                    _instance = new InputManager();
                }
                return _instance;
            }
        }

        private InputManager()
        {

        }

        public void RightAnalog()
        {
            Console.WriteLine("Right analog");
        }
    }
}
