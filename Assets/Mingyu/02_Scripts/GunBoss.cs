using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GunBoss : Boss
{
    [SerializeField] private GameObject General_BulletPref;
    [SerializeField] private GameObject Parrying_BulletPref;
    private GameObject videoEffect;
    
    #region 평타 관련 변수 모음

    private bool isSelect_DAttType = false;
    private enum  DAtt_Type
    {
        SpinAtt = 0,
        DownAtt = 1
    }

    private bool isSpinAtt;
    private int dAttType_int;
    #endregion
    
    #region p1_Skill1_변수 모음
    [SerializeField] private GameObject SignPref;
    Transform SignSpon_Pos;
    private List<GameObject> SignPref_dummyObjList = new List<GameObject>();

    private int signAttackTotalCount;
    private int signAttackCount;
    
    private int totalsignCount = 6;
    private int signCount = 0;
    
    private float signSpon_DelayTime = 0.5f;
    private float signAttDelayTime;
    
    private float signDeleteTime = 0.2f;
    
    private float signSpon_WaitTime = 0f;
    private bool isReady_CreateSign = false;
    #endregion
    
    #region p1_Skill2_변수 모음
    private Transform Dynamite_SponPos;
    [SerializeField] private GameObject DynamitePref;
    private GameObject DynamitePref_dummyObj;
    #endregion

    #region p2_Skill1_변수 모음
    private GameObject[] Install_PrefArr = new GameObject[3];
    private GameObject InstallPref_dummyObj;
    #endregion
    
    #region p2_Skill2_변수 모음

    private Transform LeftShootingPos;
    private Transform RightShootingPos;
    
    private float minAngle_R = 0;
    private float maxAngle_R = 0;
    private float p2Skill2_TotalBulletCount = 6;
    #endregion

    #region p2_Skill3_변수 모음
    private Transform P2Skill3_SponMaxPos;
    private int p2S3_BulletTotalCount = 8;
    private float p2S3_SponMinYpos;
    private GameObject p2S3_Plate;
    
    private int p2S3_AttackTotalCount;
    private int p2S3_AttackCount;
    
    private float p2S3_delayTime;
    private float p2S3_BulletSpeed;
    private float p2S3_delayCount;
    private bool isReady_P2S3;

    private float interval_Ypos;
    #endregion

    [Header("소환수")]
    public GameObject prefabMob;
    public Transform prefabMobPos;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        bossState = new Boss_State();
        Init_StateValueData(ref bossState);
        bossType = BossType.Gun;
        Init_ValueData();

        signAttDelayTime = (signSpon_DelayTime * totalsignCount) + 0.5f;    // 표식 생성 p1s1
        selectedTurn_State.Add(Boss_State.State.p1_Skill2);             // 다이너마이트 p1s2 스킬 도중 회전 가능
        videoEffect.gameObject.SetActive(false);                            // 비디오 비활성화 p2s2, p2s3
        p2S3_Plate.gameObject.SetActive(false);
        
        interval_Ypos = (P2Skill3_SponMaxPos.position.y - p2S3_SponMinYpos) / p2S3_BulletTotalCount;
    }
    
    protected override void Init_ValueData()
    {
        // 이동 관련 변수
        move_Speed = 3f;
        groundApproachDist = 0.5f;
        
        // 평타 관련 변수
        DA_HitArea = new GameObject[3];
        DA_HitArea[0] = this.transform.GetChild(0).gameObject;
        DA_HitArea[1] = this.transform.GetChild(1).gameObject;
        DA_HitArea[2] = this.transform.GetChild(2).gameObject;

        videoEffect = GameObject.Find("SandWind_Effect").gameObject;

        // 표식 공격 관련 변수
        totalsignCount = 5;
        signSpon_DelayTime = 0.05f;
        signDeleteTime = 0.05f;
        
        signAttackTotalCount = 3;
        
        // 다이너 마이트 공격 관련 변수
        Dynamite_SponPos = this.transform.GetChild(3).gameObject.transform;
        
        // 설치기 관련 변수
        GameObject installObj_Parent = GameObject.Find("InstallObj").gameObject;
        
        Install_PrefArr = new GameObject[3];
        Install_PrefArr[0] = installObj_Parent.gameObject.transform.GetChild(0).gameObject;
        Install_PrefArr[1] = installObj_Parent.gameObject.transform.GetChild(1).gameObject;
        Install_PrefArr[2] = installObj_Parent.gameObject.transform.GetChild(2).gameObject;

        // 총쏘기 패턴
        LeftShootingPos = this.transform.GetChild(4).gameObject.transform;
        RightShootingPos = this.transform.GetChild(5).gameObject.transform;

        minAngle_R = -30f;
        maxAngle_R = 60f;

        p2Skill2_TotalBulletCount = 10;
        
        // 총알 벽 패턴
        P2Skill3_SponMaxPos = GameObject.Find("Left_BulletSponYMaxPos").gameObject.transform;
        p2S3_Plate = GameObject.Find("Plate_p2s3").gameObject;
        
        p2S3_BulletTotalCount = 8;
        p2S3_SponMinYpos = -1.5f;
        p2S3_delayCount = 2;
        p2S3_BulletSpeed = 7;
        isReady_P2S3 = false;

        p2S3_AttackTotalCount = 3;
        p2S3_AttackCount = 0;
        p2S3_delayTime = 2f;
    }

    protected override void Init_StateValueData(ref Boss_State state)
    {
        state.defaultAtt_dist = 1f;

        state.skill_CoolTime = 3.0f;
    
        state.p1_Skill1_dist = 5000f;
        state.p1_Skill2_dist = 4.5f;
    
        state.p2_Skill1_dist = 5000f;
        state.p2_Skill2_dist = 5000f;
        state.p2_Skill3_dist = 5000f;
        
        state.p1S1_PossibilityNumber = 0;  // 0  ~ 49  50%
        state.p1S2_PossibilityNumber = 50; // 50 ~ 99  50%
    
        state.p2S1_PossibilityNumber = 0;  // 0 ~ 29    30%
        state.p2S2_PossibilityNumber = 30; // 30 ~ 59   30%
        state.p2S3_PossibilityNumber = 60; // 60 ~ 99   40%
    }
    
    #region p1_Skill1_함수
    public void Setting_p1Skill1()
    {
        isReady_CreateSign = true;
        Invoke("Attack_p1Skill1", signAttDelayTime);
    }

    public void Attack_p1Skill1()
    {
        animCtrl.SetBool("isSignAtt", true);
        isReady_CreateSign = false;

        signAttackCount++;
        
        foreach (GameObject dummy_signPref in SignPref_dummyObjList)
        {
            dummy_signPref.GetComponent<HitColider>().owner = this.gameObject.GetComponent<Entity>();
            dummy_signPref.GetComponent<SignCtrl>().Shoot_Bullet();
        }
    }
    
    public void CheckSignAtt_End()
    {
        foreach (GameObject dummy_signPref in SignPref_dummyObjList)
        {
            Destroy(dummy_signPref, signDeleteTime);
        }
        SignPref_dummyObjList.Clear();
        
        if (signAttackCount >= signAttackTotalCount)
        {
            Invoke("End_P1Skill1", 0.5f);
            EndSkill();
        }
        else
        {
            animCtrl.SetBool("isSignAtt", false);
        }
    }

    private void Create_AttSign()
    {
        Transform playerPos = player.transform;
        
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);
        Vector3 randomPos = new Vector3(playerPos.position.x + randomX, playerPos.position.y + randomY, playerPos.position.z);
        
        GameObject dummyObj = Instantiate(SignPref, randomPos, quaternion.identity);
        dummyObj.GetComponent<HitColider>().owner = this.gameObject.GetComponent<Entity>();
        SignPref_dummyObjList.Add(dummyObj);
    }

    private void End_P1Skill1()
    {
        animCtrl.SetBool("isSignAtt", false);

        signAttackCount = 0;
        SignPref_dummyObjList.Clear();
        signCount = 0;
    }
    
    #endregion
    
    #region p1_Skill2_함수
    public void CreateDynamite()
    {
        DynamitePref_dummyObj = Instantiate(DynamitePref, Dynamite_SponPos.position, Quaternion.identity);
        DynamitePref_dummyObj.GetComponent<DynamiteCtrl>().GunBoss = this.gameObject;
        
        DynamitePref_dummyObj.GetComponent<DynamiteCtrl>().pDir = Mathf.Abs(this.transform.rotation.y) > 0
            ? DynamiteCtrl.playerDirection.right
            : DynamiteCtrl.playerDirection.left;

        myRd.velocity = Vector2.zero;
    }
    
    public void AttackDynamite()
    {
        animCtrl.SetBool("isDynamiteAtt", true);
    }

    public void EndP1Skill2_Attack()
    {
        animCtrl.SetBool("isDynamiteAtt", false);
        EndSkill();
    }
    #endregion

    #region p2_Skill1_함수
    public void Create_InstallOBj()
    {
        for (int i = 0; i < 3; i++)
        {
            Install_PrefArr[i].gameObject.SetActive(true);
            Install_PrefArr[i].gameObject.GetComponent<Install_Ctrl>().isActive = true;
            Install_PrefArr[i].gameObject.GetComponent<Install_Ctrl>().BossObj = this.gameObject.GetComponent<Entity>();
            
            Install_PrefArr[i].gameObject.GetComponent<Entity>().SetHp(9000f);
            Install_PrefArr[i].gameObject.GetComponent<Install_Ctrl>().SetMaxHP(9000);
        }
    }

    public void Broken_InstallObj(GameObject brokenOBj)
    {
        brokenOBj.gameObject.SetActive(false);
        brokenOBj.gameObject.GetComponent<Install_Ctrl>().isActive = false;
    }
    #endregion
    
    #region p2_Skill2_함수
    public void PlayOnVideo()
    {
        videoEffect.gameObject.SetActive(true);
        videoEffect.gameObject.GetComponent<Animator>().SetBool("isEnd", false);
    }
    
    public void Attack_p2Skill2()
    {
        float angleInterval = (maxAngle_R - minAngle_R) / (p2Skill2_TotalBulletCount / 2);
        float randomRotationZ;

        GameObject tempObj;
        if (this.transform.eulerAngles.y == 180)
        {
            for (int i = 0; i < p2Skill2_TotalBulletCount / 2; i++)
            {
                randomRotationZ = Random.Range(minAngle_R + (angleInterval * i), minAngle_R + angleInterval + (angleInterval * i));
                Shoot_ParringBullet(randomRotationZ, LeftShootingPos);
            }
        
            for (int i = 0; i < p2Skill2_TotalBulletCount / 2; i++)
            {
                float minAngle_L = 180f - minAngle_R;
                randomRotationZ = Random.Range(minAngle_L - (angleInterval * i), minAngle_L - angleInterval - (angleInterval * i));
                Shoot_ParringBullet(randomRotationZ, RightShootingPos);
            }
        }
        else
        {
            for (int i = 0; i < p2Skill2_TotalBulletCount / 2; i++)
            {
                randomRotationZ = Random.Range(minAngle_R + (angleInterval * i),
                    minAngle_R + angleInterval + (angleInterval * i));
                Shoot_ParringBullet(randomRotationZ, RightShootingPos);
            }

            for (int i = 0; i < p2Skill2_TotalBulletCount / 2; i++)
            {
                float minAngle_L = 180f - minAngle_R;
                randomRotationZ = Random.Range(minAngle_L - (angleInterval * i),
                    minAngle_L - angleInterval - (angleInterval * i));
                Shoot_ParringBullet(randomRotationZ, LeftShootingPos);
            }
        }
    }

    private void Shoot_ParringBullet(float input_RotationZ, Transform shootingPos)
    {
        GameObject dummy_ParrBullet =  Instantiate(Parrying_BulletPref, shootingPos.position, Quaternion.identity);
        dummy_ParrBullet.GetComponent<BulletCtrl>().install_ZValue = input_RotationZ;
        dummy_ParrBullet.GetComponent<Bullet_HitCollder>().owner = this.gameObject.GetComponent<Entity>();
    }
    
    public void PlayOffVideo()
    {
        videoEffect.gameObject.GetComponent<Animator>().SetBool("isEnd", true);
    }
    #endregion
    
    #region p2_Skill3_함수
    public void Set_AttackSetting_P2S3()
    {
        Spon_PlayerDashBan();
        
        isReady_P2S3 = true;
        this.gameObject.GetComponent<Rigidbody2D>().simulated = false;
        p2S3_Plate.gameObject.SetActive(true);
        // 낙타 생성
        for(int i = 2; i < 5; i++) 
        { 
            Destroyer camel = Instantiate(prefabMob, prefabMobPos.position, Quaternion.Euler(new Vector3(0,180,0))).GetComponent<Destroyer>();
            camel.GetComponent<HitColider>().owner = this.gameObject.GetComponent<Entity>();
            camel.moveSpeed = i;
        }
    }
    
    public void Attack_P2Skill3()
    {
        Debug.Log("Attack!!");
        
        // 총알의 갯수가 p2S3_BulletTotalCount라면, p2S3_BulletTotalCount - 1인덱스까지 존재
        // 따라서, 0 ~ p2S3_BulletTotalCount - 2의 랜덤인 값을 받아와야함
        int randomPassIndex = Random.Range(0, p2S3_BulletTotalCount - 1);
        
        SponP2Skill3_Bullet(interval_Ypos, randomPassIndex);
    }

    private void SponP2Skill3_Bullet(float interVal_YPos, int randomPassIndex)
    {
        for (int i = 0; i < p2S3_BulletTotalCount; i++)
        {
            Vector3 SponPos = P2Skill3_SponMaxPos.position;
            SponPos.y = P2Skill3_SponMaxPos.position.y - (i * interVal_YPos);
            
            if (i == randomPassIndex)
            {
                BulletSetting(Instantiate(Parrying_BulletPref, SponPos, Quaternion.identity));
                
                SponPos.y -= interVal_YPos;
                BulletSetting(Instantiate(Parrying_BulletPref, SponPos, Quaternion.identity));
                
                i ++;
                continue;
            }

            BulletSetting(Instantiate(General_BulletPref, SponPos, Quaternion.identity));
        }
    }

    private void BulletSetting(GameObject installBullet)
    {
        if (installBullet.GetComponent<BulletCtrl>().bulletType == BulletCtrl.BulletType.Parring)
        {
            installBullet.GetComponent<BulletCtrl>().wallParringHP = 0;
        }

        installBullet.GetComponent<Bullet_HitCollder>().owner = this.gameObject.GetComponent<Entity>();
        installBullet.GetComponent<BulletCtrl>().SetDeleteTime(6f);
        installBullet.GetComponent<BulletCtrl>().addForce = p2S3_BulletSpeed;
    }

    private void End_P2Skill3()
    {
        Destroy_PlayerDashBan();
        
        isReady_P2S3 = false;
        p2S3_AttackCount = 0;
        
        animCtrl.SetBool("isAppear_p2s3", true);
        
        p2S3_Plate.gameObject.SetActive(false);
        this.gameObject.GetComponent<Rigidbody2D>().simulated = true;
    }
    #endregion

    protected override void EachBoss_UpdateSetting()
    {
        if (isReady_CreateSign && signCount < totalsignCount)
        {
            signSpon_WaitTime += Time.deltaTime;
        
            if (signSpon_WaitTime >= signSpon_DelayTime)
            {
                Create_AttSign();
                
                signSpon_WaitTime = 0;
                signCount++;
            }
        }
        
        else if (signCount >= totalsignCount)
        {
            isReady_CreateSign = false;
            signCount = 0;
        }

        if (isReady_P2S3 && p2S3_AttackCount < p2S3_AttackTotalCount)
        {
            p2S3_delayCount += Time.deltaTime;

            if (p2S3_delayCount >= p2S3_delayTime)
            {
                Attack_P2Skill3();

                p2S3_delayCount = 0;
                p2S3_AttackCount++;
            }
        }
        else if (p2S3_AttackCount >= p2S3_AttackTotalCount)
        {
            Invoke("End_P2Skill3", 2.5f);
        }
    }

    protected override void EachBoss_AttackSetting()
    {
        if (!isSelect_DAttType)
        {
            isSelect_DAttType = true;

            dAttType_int = Random.Range((int)DAtt_Type.SpinAtt, (int)DAtt_Type.DownAtt + 1);
            if (dAttType_int == 1) isSpinAtt = false;
            else isSpinAtt = true;

            animCtrl.SetBool("isSpinAtt", isSpinAtt);
        }
    }
    
    #region 엔드 세팅
    protected override void EachBoss_EndAttack()
    {
        isSelect_DAttType = false;
    }

    public override void EachBoss_EndSkill()
    {
        animCtrl.SetBool("isAppear_p2s3", false);
        isSelect_DAttType = false;
    }

    protected virtual void DieSkillEnd()
    {
        // 사망시 스킬 종료
        /*1. 모래 바람 삭제
         2. 총탄 삭제
         3. 설치기 삭제*/
        
        //1. 모래 바람 삭제
        videoEffect.SetActive(false);
        
        //2. 총탄 삭제
        foreach (GameObject dummy_signPref in SignPref_dummyObjList)
        {
            Destroy(dummy_signPref, signDeleteTime);
        }
        
        //3. 설치기 삭제
        for (int i = 0; i < 3; i++)
        {
            if(Install_PrefArr[i].gameObject.activeSelf)
                Install_PrefArr[i].gameObject.GetComponent<Install_Ctrl>().EndSetting();
        }
        
        EndSkill();
    }
    #endregion
}
