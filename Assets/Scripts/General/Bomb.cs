using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AC_Physics;
public class Bomb : MonoBehaviour
{
    [SerializeField] private Rigid_Body _rigidBody;
    [SerializeField] private float explosionForce;

	// Use this for initialization
	void Start () {
        _rigidBody.Collider.CollisionEnter = CollisionEnter;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void CollisionEnter(Base_Collider rhs, CollisionInfo colInfo)
    {
        WorldManager.wmInstance.CreateExplosion(explosionForce, _rigidBody);
        //After explosion remove object from game world
        gameObject.SetActive(false);
        //WorldManager.wmInstance.removeNextUpdate.Add(_rigidBody);
    }
}
