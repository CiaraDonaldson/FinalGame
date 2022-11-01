using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    private float wallJumpCooldown;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    public CircleCollider2D boxCollider;
    public bool IsGrounded = true;
    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 12.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * speed * Time.deltaTime;

        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.position += -Vector3.right * speed * Time.deltaTime;

        }

        if (rb.velocity.y == 0)
        {
            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }

        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

        WallJump();

        if (IsGrounded)
        {
            if (Input.GetKeyDown("space"))
            {
                Jump();

            }
        }

    }

    void WallJump()
    {
        if (wallJumpCooldown > 0.2f)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);


            if (!IsGrounded && OnWall())
            {
                rb.gravityScale = 0;
                rb.velocity = Vector2.zero;
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

    private bool OnWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    private void Jump()
    {
        if (IsGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
          
        }

        else if (OnWall() && IsGrounded)
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
}
