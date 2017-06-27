using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IGravitable, ITemperaturable {

    [SerializeField]
    float speed = 200.0f;

    Rigidbody2D myRb;
    private Animator animator;
    BoxCollider2D collider;

    // this items represent the boots and the vest in that order
    bool[] itemInUse = { false, false };

    // this stores the previous status for each power
    int[] status = { 1, 1 };

    bool dead = false;

	// Used awake for this to avoid conflicts with null pointers when instantiating another player
	void Awake () {
        myRb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        /*
        Vector2 finalPosition = myRb.position;
        finalPosition.x += speed * Time.deltaTime;
        myRb.MovePosition(finalPosition);
        */
        myRb.velocity = new Vector2(speed * Time.deltaTime, myRb.velocity.y);
        if (myRb.velocity.y < 0)
        {
            animator.SetBool("falling", true);
        }
        else
        {
            animator.SetBool("falling", false);
        }
    }

    private void NeutralGravity()
    {
        myRb.gravityScale = 1.0f;
    }

    public void LowGravity()
    {
        if (itemInUse[0] && !dead)
        {
            NeutralGravity();
        }
        else
        {
            myRb.gravityScale = 0.0f;
        }
        status[0] = 0;
    }

    public void NormalGravity()
    {
        NeutralGravity();
        status[0] = 1;
    }

    public void InversedGravity()
    {
        if (itemInUse[0] && !dead)
        {
            NeutralGravity();
        }
        else
        {
            myRb.gravityScale = -1.0f;
        }
        status[0] = 2;
    }

    private void NeutralTemperature()
    {

    }

    public void Cold()
    {

    }

    public void NormalTemperature()
    {

    }

    public void Hot()
    {

    }

    public void SetItemInUse(int powerIndex, bool state)
    {
        itemInUse[powerIndex] = state;
    }

    public void SetCurrentStatus(int powerIndex, int intensity)
    {
        status[powerIndex] = intensity;
    }

    // this sets the player to start being affected again or stop being affected based upon the items he has
    public void CheckStatus()
    {
        if (itemInUse[0])
        {
            Debug.Log("Should have normal gravity");
            NeutralGravity();
        }
        else
        {
            Debug.Log("Returning to previous gravity");
            ReturnToStatus(0);
        }
        if (itemInUse[1])
        {
            Debug.Log("Should have normal temperature");
            NormalTemperature();
        }
        else
        {
            ReturnToStatus(1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Equals("Hazard"))
        {
            if (!dead) Die();
        }
    }

    public void Die()
    {
        dead = true;
        PowerManager.Instance.DeleteFromLists(gameObject);
        // set the layer to dead player to avoid him colliding with anything but the floor and walls
        //gameObject.layer = 8;
        gameObject.tag = "Dead Player";
        animator.SetBool("dead", true);
        collider.size = new Vector2(2.25f, 0.9f);
        myRb.velocity = Vector2.zero;
        GameManager.Instance.SpawnPlayer();
        enabled = false;
        //Destroy(gameObject);
    }

    private void ReturnToStatus(int powerIndex)
    {
        switch (powerIndex)
        {
            case 0:
                switch (status[powerIndex])
                {
                    case 0:
                        LowGravity();
                        break;
                    case 1:
                        NormalGravity();
                        break;
                    case 2:
                        InversedGravity();
                        break;
                    default:
                        Debug.Log("Something went wrong on Return to Grav player");
                        break;
                }
                break;
            case 1:
                switch (status[powerIndex])
                {
                    case 0:
                        Cold();
                        break;
                    case 1:
                        NormalTemperature();
                        break;
                    case 2:
                        Hot();
                        break;
                    default:
                        Debug.Log("Something went wrong on Return to Grav player");
                        break;
                }
                break;
            default:
                Debug.Log("Something went wrong in player Return to status");
                break;
        }
    }
}
