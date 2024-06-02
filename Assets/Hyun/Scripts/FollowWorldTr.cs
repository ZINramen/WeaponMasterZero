using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWorldTr : MonoBehaviour
{
    public bool targetIsPlayer;
    public Transform target;
    RectTransform rectTr;

    private void Start()
    {
        rectTr = GetComponent<RectTransform>();
        if (targetIsPlayer)
            target = Entity.Player.transform;
    }
    private void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(target.position);
    }
}
