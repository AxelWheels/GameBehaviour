using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    abstract public class BaseBehaviour : MonoBehaviour, ISimpleBehaviour
    {
        protected float weight = 0f;

        public float Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        virtual public bool ValidBehaviour()
        {
            return false;
        }

       virtual public void ExecuteAction()
        {
        }
    }
}
