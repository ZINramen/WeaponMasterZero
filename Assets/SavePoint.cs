using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public static bool save = false;
    public Color awaken;
    SpriteRenderer sr;

    public GameObject TimeLine;
    public GameObject TimeLine2;

    public void MapSave()
    {
        save = true;
        sr.color = awaken;
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if(save)
            sr.color = awaken;
    }
    private void Start()
    {
        if (save)
            Entity.Player.transform.position = transform.position;
        else
        {
            if (TimeLine != null)
                TimeLine.SetActive(true);
            if (TimeLine2 != null)
                TimeLine2.SetActive(true);
        }
    }
}
