using System;
using Microsoft.Xna.Framework;

namespace Edytejshyn.GUI
{
    public class EditorMouseState
    {
        public Vector2 Position;
        public bool Left, Middle, Right;

        public EditorMouseState(Vector2 position = new Vector2())
        {
            Position = position;
        }

        public EditorMouseState(EditorMouseState src)
        {
            Position = src.Position;
            Left     = src.Left;
            Middle   = src.Middle;
            Right    = src.Right;
        }

    }
}