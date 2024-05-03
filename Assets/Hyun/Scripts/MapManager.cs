using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Text mapText;
    static string currentMapName = "Forest";
    public AutoSceneLoad sceneLoad;
    // Start is called before the first frame update

    private void Start()
    {
        mapText.text = currentMapName;
    }

    public void ChangeCurrentMapName(string newName)
    {
        currentMapName = newName;
        sceneLoad.sname = newName;
        mapText.text = newName;
    }


}
