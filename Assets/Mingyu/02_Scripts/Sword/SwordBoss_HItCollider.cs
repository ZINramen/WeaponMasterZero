using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordBoss_HItCollider : HitColider
{
    protected override void EachObj_HitSetting(Collider2D other)
    {
        // 보스가 특정 스킬에 플레이어에게 타격을 입혔는지 체크
        if (owner && owner.GetComponent<Boss>() && owner.GetComponent<Boss>().bossType == BossType.Sword)
        {
            if (owner.GetComponent<SwordBoss>().Get_CurrBossState().currentState == Boss_State.State.p2_Skill2)
            {
                if (other.gameObject.tag == "Player")
                {
                    owner.GetComponent<SwordBoss>().Set_HitPlayer_fromP2S2(true);
                }
            }
        }
    }
}
