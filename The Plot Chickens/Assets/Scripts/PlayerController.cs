using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rb;
    private CapsuleCollider2D boxCollider;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("WallJump")]
    [SerializeField] private float wallJumpX; 
    [SerializeField] private float wallJumpY;

    private float wallJumpCooldown;
    private float horizontal;
    private float speed = 8f;
    

    [Header("Jumps")]
    [SerializeField] private float jumpingPower = 10f;
    public int extraJumps = 2;
    public int jumpCounter = 0;

    [Header("Dash")]
    public bool isDashing;
    [SerializeField] private float dashingPower = 15f;
    private float dashingTime = 1f;
    private float dashingCooldown = 1f;
    private bool canDash = true;

    //public bool isGrounded = true;
    private Animator anim;
    public int counter = 0;
    private bool isFacingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        //Movement
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
            anim.Play("Run");
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.position += -Vector3.right * speed * Time.deltaTime;
            anim.Play("Run");
        }
        else if (isGrounded())
        {
            jumpCounter = 0;
            anim.Play("Idle");
        }

        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            Jump();
        }

        /*if (Input.GetKeyDown(KeyCode.Space) && !isGrounded())
        {
           
            Jump();

        }
        else if(jumpCounter > extraJumps)
        {
            Debug.Log("STOP");
        }*/


             //Adjustable jump height
            //if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0)
            // {
            //     rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2);
            // }




        WallJump();
        Dashing();
        Flip();


    }

    private bool OnWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 1f, groundLayer);
        return raycastHit.collider != null;
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    void Dashing()
    {
        if (Input.GetKeyDown(KeyCode.W) && canDash)
        {
            Debug.Log("Dash");
            anim.Play("Dash");
            StartCoroutine(Dash());
            
        }
    }

    private void WallJump()
    {
        if (wallJumpCooldown > 0.2f)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);


            if (!isGrounded() && OnWall())
            {
                rb.gravityScale = 0;
                rb.velocity = Vector2.zero;
                anim.Play("WallHold");

            }
            else
            {
                rb.gravityScale = 2;
            }

            if (Input.GetKeyDown("space"))
            {
                Jump();
            }


        }
        else
        {
            wallJumpCooldown += Time.deltaTime;
        }
    }


    private void Jump()
    {
        jumpCounter += 1;

        if (isGrounded())
        {
            Debug.Log("JUMP");
            
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            anim.Play("Jump");
        }

        /*if (!isGrounded() && jumpCounter < extraJumps)
        {

            anim.Play("Jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }*/


        else if (OnWall() && isGrounded())
        {
            if (horizontal == 0)
            {
                rb.velocity = new Vector3(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            }
        }
        else
        {
            rb.velocity = new Vector3(-Mathf.Sign(transform.localScale.x) * 3, 6);
        }

        wallJumpCooldown = 0;

    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "death")
        {
            Debug.Log("dead");
            anim.Play("Dead");
        }
    }

    void OnCollisionStay2D(Collision2D collider)
    {
        if (collider.gameObject.name == "Cage")
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                counter++;
                if (counter == 3)
                {
                    Destroy(collider.gameObject);
                }
            }
        }
    }
}
