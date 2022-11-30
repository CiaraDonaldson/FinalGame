using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinRun : MonoBehaviour
{

    private Rigidbody2D rb;
    public bool stop = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!stop)
        {
            rb.velocity = transform.right * 8;
        }
        
    }

    void OnCollisionEnter2D(Collision2D collision) 
    {
        stop = true;
    
    }
    
}
