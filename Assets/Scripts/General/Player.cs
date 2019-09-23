using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using AC_Physics;

public class Player: MonoBehaviour
{
    [SerializeField] private Rigid_Body _rigidBody;
    public float horizontalInput = 0.0f;
    public float verticalInput = 0.0f;
    //To counter gravity requires a larger force. This is why the y sensitivity is greater than the x
    public float ySensitivity = 800f;
    public float xSensitivity = 20f;

    private bool canJump = false;

    public Rigid_Body _RigidBody
    {
        get { return _rigidBody; }
    }

    public bool CanJump
    {
        get { return canJump; }
        set { canJump = value; }
    }

    public void Start()
    {
        _rigidBody.Collider.CollisionEnter = CollisionEnter;
        _rigidBody.Collider.CollisionExit = CollisionExit;
    }

    public void Update()
    {
        //Debug.Log(canJump);
        ManageInput();
    }

    public void ManageInput()
    {
        Vec2 forceToApply = Vec2.Zero;

        //canJump is set when the player enters a collision
        //Once a vertical input is detected this is set to false so that it can not jump in midair
        if (canJump)
        {
            if (verticalInput != 0.0f)
            {
                canJump = false;
                if (verticalInput > 0.001f)
                {
                    _rigidBody.velocity.y = 0f;
                }
                _rigidBody.acceleration = WorldManager.wmInstance.WorldGravity;
                _rigidBody.Position.y += 10.0f;
                forceToApply.y = verticalInput * ySensitivity * _rigidBody.Mass;
            }
        }

        forceToApply.x = horizontalInput * xSensitivity * _rigidBody.Mass;

        _rigidBody.ApplyForceInstant(forceToApply);
    }

    public void CollisionEnter(Base_Collider rhs, CollisionInfo colInfo)
    {
        //Debug.Log("Player Collision " + colInfo.collisionNormal.y);

        if (rhs.tag == "Floor" && colInfo.collisionNormal.y < 0.0f)
        {
            canJump = true;
        }
    }

    public void CollisionExit(Base_Collider rhs, CollisionInfo colInfo)
    {
        if (rhs.tag == "Floor")
        {
            //Debug.Log("Exit Collision");
            canJump = false;
        }
    }
}
