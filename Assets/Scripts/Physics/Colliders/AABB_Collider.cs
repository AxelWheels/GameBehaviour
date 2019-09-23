using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AC_Physics
{
    [Serializable]
    class AABB_Collider: Base_Collider
    {
        [SerializeField] private Vec2 bottomLeft = new Vec2();
        [SerializeField] private Vec2 topRight = new Vec2();

        public Vec2 BottomLeft
        {
            get { return bottomLeft; }
        }

        public Vec2 TopRight
        {
            get { return topRight; }
        }

        public float Width
        {
            get { return Mathf.Abs(topRight.x - bottomLeft.x); }
        }

        public float Height
        {
            get { return Mathf.Abs(topRight.y - bottomLeft.y); }
        }
        
        public void Awake()
        {
            colliderType = ColliderType.AABB;
            Initialise();
        }
    }
}
