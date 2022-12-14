using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private IEnumerator coroutine;
   


    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;
    private CircleCollider2D circleCollider;
    private BoxCollider2D boxCollider;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("WallJump")]
    [SerializeField] private float wallJumpX;
    [SerializeField] private float wallJumpY;

    private float wallJumpCooldown;
    private float horizontal;
    private float speed = 10f;
    public int counter = 0;
    public int cageCounter = 0;

    [Header("Jumps")]
    [SerializeField] private float jumpingPower = 10f;
    public int extraJumps = 1;
    public int jumpCounter = 0;

    [Header("Dash")]
    public bool isDashing;
    [SerializeField] private float dashingPower = 15f;
    private float dashingTime = 1f;
    private float dashingCooldown = 1f;
    [SerializeField] private bool canDash = true;
    public GameObject alert;
    public SpriteRenderer alertSprite;

    [Header("Floor Check")]
    public bool isGrounded = true;
    private Animator anim;

    public bool isFacingRight = true;

    public playAnim refScript;
    private GameObject player;

    [SerializeField]
    private float glidingSpeed;
    private float initialGravityScale;


    public AudioSource dashAudio;
    public AudioSource jumpAudio;
    public AudioSource kickAudio;

    void Start()
    {
        player = this.gameObject;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        alert = transform.GetChild(1).gameObject;
        alertSprite = alert.GetComponent<SpriteRenderer>();
        initialGravityScale = rb.gravityScale;
        refScript = FindObjectOfType<playAnim>();

        Cursor.visible = false;
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
        else if (Input.GetKey(KeyCode.S))
        {
            anim.Play("Attack");
            kickAudio.Play();
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

        //DoubleJump
        if (jumpCounter < extraJumps)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isGrounded)
            {

                Jump();

            }
        }

        //Adjustable jump height
        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2);

        }

        //Glide
        if (counter <= 10)
        {
            if (Input.GetKeyDown(KeyCode.Space) && rb.velocity.y <= 0)
            {
                rb.gravityScale = 0;
                rb.velocity = new Vector2(rb.velocity.x, glidingSpeed);

                anim.Play("Jump 1");
                dashAudio.Play();
                counter++;
            }
            else
            {
                rb.gravityScale = initialGravityScale;
            }

        }
        else 
        {
            rb.gravityScale = 2;
        }






        if (canDash)
        {
            //alert.SetActive(true);
            print("dashedAlert");
            alertSprite.color = new Color(255, 255, 255, 1);
            //color.a = 1;
        }
        else
        {
            print("dashedAlertELSE");
            //alert.SetActive(false);
            alertSprite.color = new Color(255, 255, 255, 0);
        }

        
        Dashing();
        Flip();

        if (player.transform.position.y < -19)
        {
            StartCoroutine(Dying());
        }

    }

  
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
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            isGrounded = false;
            anim.Play("Jump");
            jumpCounter += 1;
            jumpAudio.Play();
        }
        if (jumpCounter == extraJumps)
        {
            return;
        }
        //DONT TOUCH


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



    public IEnumerator Dying()
    {
        anim.Play("Death");
        yield return new WaitForSeconds(2f);
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    void Dashing()
    {      
        coroutine = Dash();

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            Debug.Log("Dash");
            anim.Play("Dash");
            StartCoroutine(coroutine);
            dashAudio.Play();

        }
    }

    



    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        if (isFacingRight)
        {
            
            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0f;
            transform.position += Vector3.right * dashingPower;
            yield return new WaitForSeconds(.02f);
            transform.position += Vector3.right * dashingPower;
            yield return new WaitForSeconds(.02f);
            transform.position += Vector3.right * dashingPower;
            yield return new WaitForSeconds(dashingTime);
            isDashing = false;
            rb.gravityScale = originalGravity;

        }
        else if (!isFacingRight)
        {
            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0f;
            transform.position -= Vector3.right * dashingPower;
            yield return new WaitForSeconds(.02f);
            transform.position -= Vector3.right * dashingPower;
            yield return new WaitForSeconds(.02f);
            transform.position -= Vector3.right * dashingPower;
            yield return new WaitForSeconds(dashingTime);
            isDashing = false;
            rb.gravityScale = originalGravity;

        }
        
        anim.Play("Dash");
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
        yield break;
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
            counter = 0;
        }

        if (collider.gameObject.name == "Catsassin" && Input.GetKey(KeyCode.S))
        {
            SceneManager.LoadScene("Ending");
        }

        if (collider.gameObject.name == "Cage")
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                anim.Play("Attack");
                playAnim.instance.playIt();
                cageCounter++;
                if (cageCounter == 3)
                {
                    Destroy(collider.gameObject);

                }
            }

        }
    }
}

