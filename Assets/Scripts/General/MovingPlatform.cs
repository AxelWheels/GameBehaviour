using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AC_Physics;
public class MovingPlatform : MonoBehaviour {

    public bool changeX = true;
    public bool changeY = false;
    public float time = 0f;
    public float amplitude = 1f;
    public float frequency = 1f;
    public Rigid_Body _rigidBody;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        time += Time.deltaTime;
        if (changeX)
            _rigidBody.velocity.x = amplitude * Mathf.Sin(2 * Mathf.PI * frequency * time);

	}
}
