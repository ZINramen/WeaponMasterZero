using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectParent : MonoBehaviour
{
    private StageCtrl stageManager;
    protected Entity[] players;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        stageManager = FindObjectOfType<StageCtrl>();
        players = stageManager.PlayerList;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
