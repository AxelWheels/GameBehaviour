using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AC_Physics;

public class Lava : MonoBehaviour
{
    public Rigid_Body _rigidBody;
    // Use this for initialization
    void Start ()
    {
        _rigidBody.Collider.CollisionEnter = RestartCurrentScene;
	}

    public void RestartCurrentScene(Base_Collider rhs, CollisionInfo colInfo)
    {
        if (rhs.tag == "Player")
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

}
