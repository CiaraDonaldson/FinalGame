using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rb;
    private CapsuleCollider2D boxCollider;
    private CircleCollider2D circleCollider;

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

    private GameObject player;
    void Start()
    {
        player = this.gameObject;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<CapsuleCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        if (player.transform.position.y < -19)
        {
            Scene scene = SceneManager.GetActiveScene(); 
            SceneManager.LoadScene(scene.name);
        }

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
        else if (Input.GetKey(KeyCode.S))
        {
            anim.Play("Attack");
        }
        else if (isGrounded)
        {
            jumpCounter = 0;
            anim.Play("Idle");

        }

        //Jump
        if (Input.GetKey(KeyCode.Space) && isGrounded)
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

    }

    void Dashing()
    {
        if (Input.GetKey(KeyCode.LeftShift) && canDash)
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

   

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "death")
        {
            Debug.Log("dead");
            anim.Play("Death");
        }

        if (collider.gameObject.tag == "Floor")
        {
            isGrounded = true;
            jumpCounter = 0;
        }
        if (collider.gameObject.name == "Catsassin" && Input.GetKey(KeyCode.S))
        {
            SceneManager.LoadScene("Level 1");
        }
    }

    void OnCollisionStay2D(Collision2D collider)
    {
        if (collider.gameObject.name == "Cage")
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                anim.Play("Attack");
                counter++;
                if (counter == 3)
                {
                    Destroy(collider.gameObject);
                    
                }
            }
        }
      
        
    }
}
