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

        if (owner != Entity.Player)
        {
            // 플레이어가 아니면, 깨지지 않음
            if (other.gameObject.name.Contains("APO") &&
                other.gameObject.GetComponent<SkillManager>())
            {
                isAbleDestroy = true;
            }
        }
    }
}
