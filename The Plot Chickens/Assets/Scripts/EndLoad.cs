using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "Ending")
        {
            StartCoroutine(Restart());
        }

        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "Begin")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Begin();            
            }
        }
    }

   

    public IEnumerator Restart()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Begin", LoadSceneMode.Single);
    }

    public void Begin()
    {
        SceneManager.LoadScene("Starting Lore", LoadSceneMode.Single);
    }
}
