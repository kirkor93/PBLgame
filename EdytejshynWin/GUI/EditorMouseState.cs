using Microsoft.Xna.Framework;

namespace Edytejshyn.GUI
{
    public class EditorMouseState
    {
        public int X, Y;

        public Point Point
        {
            get
            {
                return new Point(X, Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public Vector2 Vector
        {
            get
            {
                return new Vector2(X, Y);
            }
        }

        public bool AnyMiddle
        {
            get { return (Left && Right) || Middle; }
        }

        public bool NoneButton
        {
            get { return !( Left || Middle || Right ); }
        }
        
        public bool Left, Middle, Right;

        public MouseBtn OnlyButton
        {
            get
            {
                if (AnyMiddle) return MouseBtn.Middle;
                if (NoneButton) return MouseBtn.None;
                if (Left && !Right) return MouseBtn.Left;
                if (Right && !Left) return MouseBtn.Right;
                return MouseBtn.Other;
            }
        }

        public EditorMouseState() { }

        public EditorMouseState(EditorMouseState src)
        {
            X       = src.X;
            Y       = src.Y;
            Left    = src.Left;
            Middle  = src.Middle;
            Right   = src.Right;
        }

    }

    public enum MouseBtn
    {
        Left, Middle, Right, None, Other
    }
}