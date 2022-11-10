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

    [Header("Floor Check")]
    public bool isGrounded = true;
    private Animator anim;
    private int counter = 0;
    public bool isFacingRight = true;

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
            isFacingRight = true;
          
            transform.position += Vector3.right * speed * Time.deltaTime;
            anim.Play("Run");
        }
        else if (Input.GetKey(KeyCode.A))
        {
            isFacingRight = false;
 
            transform.position += -Vector3.right * speed * Time.deltaTime;
            anim.Play("Run");
        }
        else if (isGrounded)
        {
            jumpCounter = 0;
            anim.Play("Idle");
        }

        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if (jumpCounter < extraJumps)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isGrounded)
            {

                Jump();

            }
        }



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

        Debug.DrawRay(boxCollider.bounds.size, new Vector2(transform.localScale.x, 0), Color.red, 0.1f);
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
        
    }

    /*public bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, .25f, groundLayer);
        return raycastHit.collider != null;
    }*/

    private void Jump()
    {

        //DONT TOUCH
        if (isGrounded)
        {
            Debug.Log("JUMP");

            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            anim.Play("Jump");
        }

        if (jumpCounter < extraJumps)
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpingPower), ForceMode2D.Impulse);
            isGrounded = false;
            anim.Play("Jump");
            jumpCounter += 1;
        }
        if (jumpCounter == extraJumps)
        {
            return;
        }
        //DONT TOUCH


        else if (OnWall() && !isGrounded)
        {
            
           if (horizontal == 0)
            {

                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

            }
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

        }

        
        wallJumpCooldown = 0;

    }


    private void Flip()
    {
        if (transform.localEulerAngles.y != 180 && !isFacingRight)
        { 
            transform.Rotate(0f, 180f, 0f);
        }
        else if (transform.localEulerAngles.y != 0 && isFacingRight)
        {

            transform.Rotate(0f, -180f, 0f);
        }

        /*Vector2 localScale = transform.localScale;

        if (!isFacingRight)
        {
           
        }
        if (!isFacingRight)
        {
            rb.GetComponent<SpriteRenderer>().flipX = true;
        }

        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x = -1f;
            transform.localScale = localScale;
        }*/
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


            if (!isGrounded && OnWall())
            {
                rb.gravityScale = 0;
                rb.velocity = Vector2.zero;
                jumpCounter = 0;
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
        if (collider.gameObject.tag == "Floor")
        {
            isGrounded = true;
            jumpCounter = 0;
        }

        
    }
}
