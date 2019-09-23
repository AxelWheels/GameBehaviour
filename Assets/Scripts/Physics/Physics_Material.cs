using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace AC_Physics
{
    [Serializable]
    public class Physics_Material
    {
        public enum FrictionCombine
        {
            MULTIPLY,
            MINIMUM,
            MAXIMUM,
            AVERAGE
        }

        public float staticFriction = 1f;
        public float dynamicFriction = 1f;
        public float density = 1f;
        public float restitution = 1f;
        public FrictionCombine frictionCombine = FrictionCombine.AVERAGE;

        static public float CombineFrictions(float lhs, float rhs, FrictionCombine _frictionCombine)
        {
            switch (_frictionCombine)
            {
                case FrictionCombine.MULTIPLY:
                    return lhs * rhs;
                case FrictionCombine.MINIMUM:
                    return lhs < rhs ? lhs : rhs;
                case FrictionCombine.MAXIMUM:
                    return lhs > rhs ? lhs : rhs;
                case FrictionCombine.AVERAGE:
                    return lhs + rhs * 0.5f;
                default:
                    return 0f;
            }
        }
    }
}
