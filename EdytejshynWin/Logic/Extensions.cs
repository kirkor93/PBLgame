using Microsoft.Xna.Framework;

namespace Edytejshyn.Logic
{
    public static class Extensions
    {
        public static Vector3 GetCopy(this Vector3 src)
        {
            return new Vector3(src.X, src.Y, src.Z);
        }
    }
}