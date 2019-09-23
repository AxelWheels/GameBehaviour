using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace AC_Physics
{
    [Serializable]
    public class Vec2
    {
        public const float pi = 3.14159265359f;
        public float x = 0f;
        public float y = 0f;

        public Vec2 Normalized
        {
            get
            {
                float mag = Magnitude();
                if (mag > 1)
                {
                    float invmag = 1 / mag;

                    return new Vec2(x * invmag, y * invmag);
                }
                return this;
            }
        }

        static public Vec2 Zero
        {
            get { return new Vec2(0.0f,0.0f); }
        }

        static public Vec2 One
        {
            get { return new Vec2(1.0f, 1.0f); }
        }

        static public Vec2 Up
        {
            get { return new Vec2(0.0f, 1.0f); }
        }

        static public Vec2 Down
        {
            get { return new Vec2(0.0f, -1.0f); }
        }

        static public Vec2 Left
        {
            get { return new Vec2(-1.0f, 0.0f); }
        }

        static public Vec2 Right
        {
            get { return new Vec2(1.0f, 0.0f); }
        }

        //Constructors
        public Vec2()
        {
            Initialise(0.0f, 0.0f);
        }
        public Vec2(float x, float y)
        {
            Initialise(x, y);
        }
        public Vec2(Vec2 rhs)
        {
            Initialise(rhs.x, rhs.y);
        }

        private void Initialise(float _x, float _y)
        {
            x = _x;
            y = _y;
        }

        public float DEG_TO_RAD(float angle)
        {
            return angle * (pi / 180f);
        }

        public float RAD_TO_DEG(float radians)
        {
            return radians * (180 / pi);
        }

        public Vec2 Rotate(float angle)
        {
            angle = DEG_TO_RAD(angle);
            x = (Mathf.Cos(angle) * x) + (-(Mathf.Sin(angle)) * y);
            y = (Mathf.Sin(angle) * x) + (Mathf.Cos(angle) * y);
            return this;
        }

        public float Distance(Vec2 rhs)
        {
            Vec2 temp = new Vec2(rhs);
            temp.x = x - temp.x;
            temp.y = y - temp.y;

            return Mathf.Sqrt((temp.x * temp.x) + (temp.y * temp.y));
        }

        public float DistanceSqr(Vec2 rhs)
        {
            Vec2 temp = new Vec2(rhs);
            temp.x = x - temp.x;
            temp.y = y - temp.y;

            return (temp.x * temp.x) + (temp.y * temp.y);
        }

        public float Magnitude()
        {
            return Mathf.Sqrt((x * x) + (y * y));
        }

        public float MagnitudeSqr()
        {
            return (x * x) + (y * y);
        }

        public Vec2 Normalize()
        {
            float mag = Magnitude();
            if (mag > 1)
            {
                float invmag = 1 / mag;

                x *= invmag;
                y *= invmag;
            }
            return this;
        }

        //public Vec2 Normalize(Vec2 otherVec)
        //{
        //    float mag = otherVec.Magnitude();
        //    if (mag > 1)
        //    {
        //        float invmag = 1 / mag;

        //        return new Vec2(x * invmag, y * invmag);
        //    }
        //    return this;
        //}

        public float DotProduct(Vec2 rhs)
        {
            return ((x * rhs.x) + (y * rhs.y));
        }

        public float CrossProduct(Vec2 rhs)
        {
            return (this.x * rhs.y) - (rhs.x * this.y);
        }

        static public Vec2 operator +(Vec2 lhs, Vec2 rhs)
        {
            return new Vec2(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        static public Vec2 operator -(Vec2 lhs, Vec2 rhs)
        {
            return new Vec2(lhs.x - rhs.x, lhs.y - rhs.y);
        }

        static public Vec2 operator -(Vec2 rhs)
        {
            return new Vec2(-rhs.x, -rhs.y);
        }

        static public Vec2 operator *(Vec2 lhs, Vec2 rhs)
        {
            return new Vec2(lhs.x * rhs.x, lhs.y * rhs.y);
        }

        static public Vec2 operator *(Vec2 lhs, float rhs)
        {
            return new Vec2(lhs.x * rhs, lhs.y * rhs);
        }
        static public Vec2 operator *(float lhs, Vec2 rhs)
        {
            return new Vec2(lhs * rhs.x, lhs * rhs.y);
        }
    }
}
