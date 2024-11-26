using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievement : MonoBehaviour
{
    public CsvReader dataTable;
    public SaveService save;

    public int medal_Code = 0;

    public Text medal_name;
    public Text medal_description;

    public ImageData data;
    public Image image;

    private void Update()
    {
        if (medal_Code != 0)
        {
            if (medal_Code < dataTable.lines.Count)
            {
                medal_name.text = medal_Code + ". " + dataTable.lines[medal_Code][1 + PlayerPrefs.GetInt("Country_Code", 1)];
                medal_description.text = dataTable.lines[medal_Code][3 + PlayerPrefs.GetInt("Country_Code", 1)];
            }
            if (image)
            {
                image.sprite = data.medal[medal_Code-1];
            }
        }
    }
}
