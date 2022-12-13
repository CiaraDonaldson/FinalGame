using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playAnim : MonoBehaviour
{
    public static playAnim instance;
    private Animator anim;
    private AudioSource audio;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void playIt()
    {
        anim.Play("Break");
        audio.Play();
    }
}
