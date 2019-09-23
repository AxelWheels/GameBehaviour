using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AC_Physics;
using System;

namespace AC_Physics
{
    [Serializable]
    public class Circle_Collider : Base_Collider
    {
        [SerializeField] private float radius = 1f;

        public float Radius
        {
            get { return radius; }
        }

        public void Awake()
        {
            colliderType = ColliderType.CIRCLE;
            Initialise();
        }
    }
}
