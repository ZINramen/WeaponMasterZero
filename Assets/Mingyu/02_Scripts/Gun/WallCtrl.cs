using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCtrl : MonoBehaviour
{
    public enum WallType
    {
        Up,
        Right,
        Down,
        Left
    }

    public WallType wallType;
    public Vector3 Direction;
    
    // Start is called before the first frame update
    void Start()
    {
        if (wallType == WallType.Up)
            Direction = -transform.up;
        
        else if (wallType == WallType.Right)
            Direction = -transform.right;
        
        else if (wallType == WallType.Down)
            Direction = -transform.up;
        
        else if (wallType == WallType.Left)
            Direction = -transform.right;
    }
}