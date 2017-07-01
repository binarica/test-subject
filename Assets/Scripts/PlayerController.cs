using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IGravitable, ITemperaturable {

    [SerializeField]
    float speed = 200.0f;
    public float jumpPower=0.03f;//Juan
    public LayerMask groundLayer;//Juan
    public LayerMask ObstaclesLayer;//Juan
    

    private Animator animator;
    new private BoxCollider2D collider;
    Rigidbody2D myRb;
    RaycastHit2D lastHit;

    bool dead = false;//juan
    bool jumped = false;//juan
    int layerMask;//juan

    // this items represent the boots and the vest in that order
    bool[] itemInUse = { false, false };

    // this stores the previous status for each power
    int[] status = { 1, 1 };


    CountdownTimer deathTimer;

    // Used awake for this to avoid conflicts with null pointers when instantiating another player
    void Awake () {
        myRb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        deathTimer = GetComponent<CountdownTimer>();
        lastHit = new RaycastHit2D();

        /*En realidad en vez de 11 y 10 deberia obtener el numero de layer de las propiedades: 
         * public LayerMask groundLayer;
         * public LayerMask ObstaclesLayer;
         * por ahora lo deje asi.
        */
        layerMask = (1 << 11) | (1 << 10);//Juan: inicializo los layers a usar por los raycasts.

        layerMask = (1 << 11) | (1 << 10);//Juan: inicializo los layers a usar por los raycasts.


    }
	
	// Update is called once per frame
	void FixedUpdate () {

        DoAI();

        Move();

        Animate();

    }

    private void DoAI()
    {
        /*Juan: TODO: esto no me gusta asi
          Si hay otro slope el timer no se va a reiniciar. 
          Tambien estaria bueno que si no puede saltar deje de intentarlo.*/
        if (!deathTimer.isCountingDown && deathTimer.WasActive && IsSloped())
        {
            Die();
        }
        else if (!deathTimer.isCountingDown && deathTimer.WasActive)
        {
            deathTimer.WasActive = false;
        }

        if (!IsGrounded() && !jumped)//Juan: Chequeo si debe saltar 
        {
            if (myRb.velocity.y <= 0) myRb.AddForce(Vector2.up * jumpPower);
            jumped = true;
        }
        else if (IsGrounded() && jumped)
        {
            jumped = false;
        }

        if (IsSloped() && !jumped)//Juan: Chequeo si debe saltar 
        {
            if (myRb.velocity.y <= 0) myRb.AddForce(Vector2.up * jumpPower);
            jumped = true;

            if (!deathTimer.WasActive) deathTimer.Begin(3);
        }
    }

    private void Move()
    {
        /*
        Vector2 finalPosition = myRb.position;
        finalPosition.x += speed * Time.deltaTime;
        myRb.MovePosition(finalPosition);
        */

        myRb.velocity = new Vector2(speed * Time.deltaTime, myRb.velocity.y);
    }

    private void Animate()
    {
        if (myRb.velocity.x <= 0 && myRb.velocity.x >= -0.1 && myRb.velocity.y == 0)//juan
        {
            animator.SetBool("falling", false);
            animator.SetBool("walking", false);
        }
        else if (myRb.velocity.y < -0.1)//juan
        {
            animator.SetBool("falling", true);
            animator.SetBool("walking", false);
        }
        else
        {
            animator.SetBool("falling", false);
            animator.SetBool("walking", true);
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

    bool IsGrounded()//Juan
    {
        Vector2 position =new Vector2 (transform.position.x,transform.position.y-0.5f);
        Vector2 direction =new Vector2 (1f,-1.5f);
        float distance = 1f;
        Debug.DrawRay(position, direction, Color.green,20,true);
        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, layerMask);
        if (hit != lastHit)
        {
            if (deathTimer.isCountingDown) deathTimer.Begin(3);
        }
        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }

    bool IsSloped()
    {
        Vector2 position = new Vector2(transform.position.x+0.2f, transform.position.y-0.6f);
        Vector2 direction = Vector2.right;
        float distance = 1f;
        Debug.DrawRay(position, direction, Color.red, 20, true);
        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, layerMask);
        
        if (hit.collider != null)
        {
            return true;
        }

        return false;
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
        gameObject.layer = 11;
        gameObject.tag = "Dead Player";
        animator.SetBool("dead", true);
        collider.size = new Vector2(2.25f, 0.9f);
        collider.offset = new Vector2(0f, 0f);
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
