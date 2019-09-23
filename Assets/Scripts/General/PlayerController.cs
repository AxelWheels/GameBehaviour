using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AC_Physics;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Player player;

	// Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        playerCamera.transform.position = transform.position + new Vector3(0f, 0f, -100f);
        player.horizontalInput = Input.GetAxis("Horizontal");
        player.verticalInput = Input.GetAxis("Vertical");
        
        if (Input.GetButtonDown("Fire1"))
            WorldManager.wmInstance.CreateExplosion(1000f, player._RigidBody);
    }
}
