using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleChecker : MonoBehaviour
{
    public List<Entity> Mobs;
    public UnityEvent MobZero;

    bool CheckEnd = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(CheckEnd) 
            return;
      
        bool isZero = false;
        foreach (Entity e in Mobs) 
        {
            if (e != null)
            {
                isZero = false;
                break;
            }
            else
            {
                isZero = true;
            }
        }
        if (isZero)
        {
            CheckEnd = true;
            MobZero.Invoke();
        }
    }
}
