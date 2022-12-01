using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextFadeIn : MonoBehaviour
{

    public Animator anim;
    public TextMeshProUGUI text;
    public bool on = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        text = GetComponent<TextMeshProUGUI>();

        //anim.Play()
    }

    // Update is called once per frame
    void Update()
    {
        if (on)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 255);
            anim.StopPlayback();
        }   
    }

    public void StayOn()
    {
        on = true;
    }
}
