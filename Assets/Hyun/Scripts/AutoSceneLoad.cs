using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoSceneLoad : MonoBehaviour
{
    public float delaytime;
    public string sname = "TitleMenu(Demo)";
    void Start()
    {
        StartCoroutine(Load());
        
    }
    public void ChangeName(string newName) 
    {
        sname = newName;
    }
    IEnumerator Load() 
    {
        yield return new WaitForSeconds(delaytime);
        if (sname == "Restart")
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        else if (sname != "Quit")
            SceneManager.LoadScene(sname, LoadSceneMode.Single);
        else
            Application.Quit();
    }
}