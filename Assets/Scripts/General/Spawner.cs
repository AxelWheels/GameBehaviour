using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject spawnObject;
    public float spawnRate = 3f;
    private float timer = 0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;
        Debug.Log(timer);
        if (timer >= spawnRate)
        {
            GameObject.Instantiate(spawnObject, transform.position, transform.rotation, transform);
            timer = 0f;
        }
	}
}
