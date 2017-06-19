using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Rock : MonoBehaviour, IGravitable {

    Rigidbody2D myRb;

	// Use this for initialization
	void Start () {
        myRb = GetComponent<Rigidbody2D>();
	}
	
    public void LowGravity()
    {
        myRb.gravityScale = 0.0f;
    }

    public void NormalGravity()
    {
        myRb.gravityScale = 1.0f;
    }

    public void InversedGravity()
    {
        myRb.gravityScale = -1.0f;
    }
    
}
