using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AC_Physics
{
    public enum ColliderType
    {
        BASE,
        AABB,
        CIRCLE,
        EDGE
    }

    public class Base_Collider: MonoBehaviour
    {
        //Delegates that can be set for different behaviours on collision
        public delegate void OnCollisionDelegate(Base_Collider rhs, CollisionInfo colInfo);

        [SerializeField] private Vec2 localOffset = Vec2.Zero;
        [SerializeField] private Physics_Material p_mat;
        [SerializeField] protected bool trigger;
        protected ColliderType colliderType = ColliderType.BASE;
        public OnCollisionDelegate CollisionEnter;
        public OnCollisionDelegate CollisionExit;
        public OnCollisionDelegate CollisionStay;

        //A offset from the objects original position
        public Vec2 LocalOffset
        {
            get { return localOffset; }
        }

        public ColliderType Collider_Type
        {
            get { return colliderType; }
        }

        public bool Trigger
        {
            get { return trigger; }
        }

        public Physics_Material P_Mat
        {
            get { return p_mat; }
            set { p_mat = value; }
        }

        public void Initialise()
        {
            CollisionEnter = Empty;
            CollisionExit = Empty;
            CollisionStay = Empty;
        }

        //initialise delegate to avoid null reference
        public void Empty(Base_Collider rhs, CollisionInfo colInfo)
        {

        }
    }
}
