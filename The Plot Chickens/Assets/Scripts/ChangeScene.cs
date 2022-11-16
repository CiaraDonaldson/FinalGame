using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnCollisionEnter2D(Collision2D collider)
    {
        Scene scene = SceneManager.GetActiveScene();

        if (scene.name == "Level 1")
        {
            SceneManager.LoadScene("Level 2");
        }
        if (scene.name == "Level 2")
        {
            SceneManager.LoadScene("Level 3");
        }
        if (scene.name == "Level 3")
        {
            SceneManager.LoadScene("Level 4");
        }
        if (scene.name == "Level 4")
        {
            SceneManager.LoadScene("Level 5");
        }
        if (scene.name == "Level 5")
        {
            SceneManager.LoadScene("Level 6");
        }
    }
}
