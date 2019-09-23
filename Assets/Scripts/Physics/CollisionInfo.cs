using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AC_Physics;
public class CollisionInfo
{
    public CollisionInfo()
    {
        hit = false;
        depth = Vec2.Zero;
        collisionNormal = Vec2.Zero;
    }

    public CollisionInfo(bool _hit, Vec2 _depth, Vec2 _collisionNormal)
    {
        hit = _hit;
        depth = _depth;
        collisionNormal = _collisionNormal;
    }
    
    public bool hit = false;
    public Vec2 depth;
    public Vec2 collisionNormal;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
