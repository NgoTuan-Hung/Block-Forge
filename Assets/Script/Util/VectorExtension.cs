using UnityEngine;

namespace BlockBlast.Util
{
    public static class VectorExtension
    {
        public static Vector3 WithZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }
    }
}
