using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowWave_HitCollider : HitColider
{
    [SerializeField] private string groundLayerNum;
    
    private void Start()
    {
        Destroy(this.gameObject, 3f);
    }

    protected override void EachObj_HitSetting(Collider2D other)
    {
        Debug.Log(other.gameObject.layer.ToString());

        // 오너거나 땅에 부딪히거나 같은 충격파에 부딪히면, 처리가 안됨
        if (other != owner
            && other.gameObject.layer.ToString() != groundLayerNum
            && !other.gameObject.name.Contains("ShockWave")) 
        {
            isAbleDestroy = true;
            Debug.Log(other.gameObject.name);
            
        }
    }
}
