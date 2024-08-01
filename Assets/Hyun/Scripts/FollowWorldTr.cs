using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWorldTr : MonoBehaviour
{
    bool trackEnd = false;
    public bool targetIsPlayer;
    public Transform target;
    RectTransform rectTr;

    private void Start()
    {
        rectTr = GetComponent<RectTransform>();
        if (targetIsPlayer)
        {
            target = Entity.Player.transform;
        }
    }
    private void Update()
    {
        if (!trackEnd)
            transform.position = Camera.main.WorldToScreenPoint(target.position);
        if (targetIsPlayer)
        {
            if (Entity.Player.isDie)
            {
                trackEnd = true;
            }
        }
    }
}
