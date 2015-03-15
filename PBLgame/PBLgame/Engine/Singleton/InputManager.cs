using System;

namespace PBLgame.Engine.Singleton
{
    public class InputManager : Singleton<InputManager>
    {

        public void RightAnalog()
        {
            Console.WriteLine("Right analog");
        }
    }
}
