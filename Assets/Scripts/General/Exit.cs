using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AC_Physics;

public class Exit : MonoBehaviour
{
    public int sceneNumber = 0;
    public Rigid_Body _rigidBody;
	// Use this for initialization
	void Start ()
    {
        if (_rigidBody != null)
            _rigidBody.Collider.CollisionEnter = CollisionEnter;
	}

    public void CollisionEnter(Base_Collider rhs, CollisionInfo colInfo)
    {
        if (rhs.tag == "Player")
        {
            SceneManager.LoadScene(sceneNumber);
        }
    }

    public void LoadNextScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}
