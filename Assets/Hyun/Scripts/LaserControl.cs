using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserControl : MonoBehaviour
{
    public Transform owner;
    Vector3 ownerPos;
    LineRenderer line;
    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }
    private void Update()
    {
        ownerPos = owner.position;
        line.SetPosition(0, ownerPos);
        line.SetPosition(1, ownerPos + (owner.right * 100));
    }
}
