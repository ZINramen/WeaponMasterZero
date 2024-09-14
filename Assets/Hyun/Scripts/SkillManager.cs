using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    Entity owner;
    int currentLeftWeapon = 2;
    int currentRightWeapon = 3;

    public GameObject healEffect;
    public HUDController hudControl;

    public int currentWeapon = 0;
    public int haveWeaponNum = 0;
    public bool infinite = false;
    bool isHealthEvent = false;
    private void Start()
    {
        owner = GetComponent<Entity>();

        if (hudControl) // 플레이어가 보는 UI에서 currentWeapon 값에 해당하는 무기가 무엇인지 보여줌.
            hudControl.ChangeCurrentWeapon(currentWeapon);

        ChangeWeaponSkill(false, currentWeapon); // currentWeapon 값에 해당하는 무기로 현재 무기 바꿈,
        CheckMPUI(); // Gauge 값을 체크하여 Gauge가 얼마나 모였는지 표시함. (Gauge는 보라색 마름모로 표시함. Gauge가 3이면 보라색 마름모가 3개이다.)
    }

    private void Update()
    {
        if (owner)
            owner.aManager.ani.SetInteger("Weapon", currentWeapon); // 애니메이터의 파라미터 값을 바꿔 현재 무기에 맞는 애니메이션이 재생되도록 함.
        
        if (hudControl)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ChangeWeaponSkill(true); // 왼쪽무기와 현재무기교체 (현재 무기는 왼쪽으로 이동)
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                ChangeWeaponSkill(false); // 오른쪽무기와 현재무기교체 (현재 무기는 오른쪽으로 이동)
            }
            if (owner.GetHp() > 0 && Input.GetKeyDown(KeyCode.W)) // 체력 회복
            {
                int curGauge = owner.aManager.ani.GetInteger("Gauge"); // 애니메이터에서 Gauge 값을 가져옴. Gauge는 스킬, 필살기 사용에 필요한 값이다.
                if (curGauge > 0) // Gauge 값이 0보다 크면 게이지 값 1을 소모하여 체력 회복이 가능하다.
                {
                    MedalControlSystem.hpBeforeChange = (int)owner.GetHp(); 
                    Instantiate(healEffect, owner.transform.position, Quaternion.identity).transform.parent = owner.transform;
                    owner.SetHp(owner.GetHp() + owner.maxHP * 0.3f); // 체력 회복 퍼센테이지 0.3
                    isHealthEvent = true;
                    ReduceGauge();
                }
            }
        }
        if (owner.GetMp() == owner.maxMp) // 현재 MP값이 최대치일 때 처리
        {
            int curGauge = owner.aManager.ani.GetInteger("Gauge");
            if (curGauge < 3) // 값이 3이 아닐 경우 게이지 값이 증가한다.
            {
                owner.aManager.ani.SetInteger("Gauge", curGauge + 1);
                CheckMPUI();
            }
            owner.SetMp(0); // MP값이 0으로 초기화됨.
        }
    }


    public void ChangeWeaponSkill(bool isLeftWeapon, int newWeapon = -1) // 첫번째 매개변수는 왼쪽 무기와 교체하는지 여부, 두번째 매개변수는 바꿀 무기의 번호이다.
    {
        var previousWeapon = currentWeapon; // 교체 전 무기를 변수로 저장
        if (newWeapon == -1)  // 무기의 번호를 매개변수로 전달 안 할 경우
        {
            if (isLeftWeapon) // 왼쪽으로 교체하는 버튼을 눌렀을 경우
                currentWeapon = currentLeftWeapon; // 현재 무기를 기본 값으로 설정된 왼쪽무기로 바꿈.
            else
                currentWeapon = currentRightWeapon; // 현재 무기를 기본 값으로 설정된 오른쪽무기로 바꿈.
        }
        else
            currentWeapon = newWeapon; // 무기 번호에 맞는 무기로 바뀜.

        if(haveWeaponNum < currentWeapon) // 바꾼 무기의 번호(무기 번호는 얻는 순서와 같음)가 현재 보유한 무기의 수보다 작을 경우
        {
            currentWeapon = previousWeapon; // 이전 무기로 다시 바꿈.
            return;
        }

        hudControl.ChangeCurrentWeapon(currentWeapon); // 현재 무기를 UI에 반영

        bool left = false;
        for (int i = 0; i < 3; i++) // 다음 왼쪽 무기와 오른쪽 무기를 번호 순서에 따라 정함.
        {
            if (i != (currentWeapon - 1))
            {
                if (!left)
                {
                    left = true;
                    currentLeftWeapon = i + 1;
                }
                else
                    currentRightWeapon = i + 1;
            }
        }
    }

    public void CheckMPUI() 
    {
        int curGauge = owner.aManager.ani.GetInteger("Gauge");
        for (int i = 0; i < hudControl.GaugeIcons.Length; i++)
        {
            hudControl.GaugeIcons[i].SetActive(false);
        }
        for (int i = 0; i < curGauge; i++)
        {
            hudControl.GaugeIcons[i].SetActive(true);
        }
    }

    public void ReduceGauge() 
    {
        int curGauge = owner.aManager.ani.GetInteger("Gauge");
        if (curGauge == 3 && !isHealthEvent)
        {
            owner.aManager.ani.SetInteger("Gauge", 0);
            for (int i = 0; i < hudControl.GaugeIcons.Length; i++)
            {
                hudControl.GaugeIcons[i].SetActive(false);
            }
        }
        else
        {
            owner.aManager.ani.SetInteger("Gauge", curGauge - 1);
            hudControl.GaugeIcons[curGauge - 1].SetActive(false);
        }
        isHealthEvent = false;
    }
}
