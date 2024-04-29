using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mingyu_Goto_TitleScene : MonoBehaviour
{
    public void Goto_TitleScene()
    {
        SceneManager.LoadScene("TitleMenu(Demo)");
    }

    // #디버깅용
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            SceneManager.LoadScene("TitleMenu(Demo)");
        }
    }
}
