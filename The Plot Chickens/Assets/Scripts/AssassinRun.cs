using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinRun : MonoBehaviour
{

    private Rigidbody2D rb;
    public bool stop = false;
    private Animator anim;
    public int run = 7;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!stop)
        {
            rb.velocity = transform.right * run;
        }
        anim.Play("Run");
    }

    void OnCollisionEnter2D(Collision2D collision) 
    {
        stop = true;
    
    }
    
}
