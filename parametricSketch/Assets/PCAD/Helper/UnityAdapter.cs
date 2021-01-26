using System;
using UnityEngine;

namespace PCAD.Helper
{
    public static class UnityAdapter
    {
        public static Vector3 ToUnityVector(Vec<float> vector) => new Vector3(vector.X, vector.Y, vector.Z);

        public static Vector3 DirectionVector(Vec.AxisID axis)
        {
            switch (axis)
            {
                case Vec.AxisID.X:
                    return Vector3.right;
                case Vec.AxisID.Y:
                    return Vector3.forward;
                case Vec.AxisID.Z:
                    return Vector3.up;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }
    }
}