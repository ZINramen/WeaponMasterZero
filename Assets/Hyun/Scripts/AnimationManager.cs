using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [HideInInspector] public SkillManager skillManager;
    
    public Entity owner;
    public GameObject temp;
    public bool additionalJump = false;
    public bool onGround = true;
    public bool GetOnGround() { return onGround; }

    public bool ecActive = false;
    public float rotationZ = 0;

    public enum AnimationState 
    { Normal, Jump, Fall, Emotion, Stun }
    
    public Animator ani;
    public AnimationState State = AnimationState.Normal;

    [Header("PlayerSet")] 
    [Tooltip("조종할 플레이어 캐릭터의 경우 True")]
    public bool isPlayer = false;
    public bool isHuman = false;

    public EffectCreator Ec;

    [Header("Key Mapping")]
    public KeyCode Punch;
    public KeyCode Kick;
    public KeyCode Guard;
    public KeyCode Catch;
    public KeyCode Dash;
    public KeyCode Backstep;
    public KeyCode Jump;

    public KeyCode DownArrow;
    public KeyCode UpArrow;

    public KeyCode Emotion_1;
    public KeyCode Heal;



    public void SetPlayerType(bool value)
    {
        isPlayer = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
        skillManager = GetComponent<SkillManager>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit;
        if (hit = Physics2D.Raycast(transform.position - new Vector3(0, transform.localScale.y / 2), Vector3.down, 0.04f))
        {
            if (onGround && State != AnimationState.Normal)
            {
                ani.SetTrigger("Landing");
                State = AnimationState.Normal;
            }
            
            if (State == AnimationState.Fall)
            {
                onGround = true;
                if (isHuman)
                {
                    ani.ResetTrigger("Jump");
                    ani.SetTrigger("Landing");
                }
                if (Ec && ecActive)
                {
                    Ec.PlayEffect("bang", hit);

                    ecActive = false;
                }
            }
        }
        else
        {
            if (State == AnimationState.Normal)
            {
                if (isHuman)
                {
                    ani.SetTrigger("fall");

                }
            }
            if (onGround)
            {
                onGround = false;
                additionalJump = false;
                if (isHuman)
                {
                    ani.ResetTrigger("Landing");
                }
            }
            else
            {
                if (isPlayer)
                {
                    if (Input.GetKeyDown(Punch))
                    {
                        ani.SetTrigger("Punch");

                        ecActive = true;
                    }
                }
            }
        }
        if (isPlayer)
            PlayerAnimation();
        
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, rotationZ);
    }
    void ResetTriggerEvent(string name) 
    {
        ani.ResetTrigger(name);
    }
    void ResetAttackTriggerEvent()
    {
        if (!ani) return;
        ani.ResetTrigger("Punch_Up");
        ani.ResetTrigger("Punch");
        ani.ResetTrigger("Kick");
        ani.ResetTrigger("Catch");
        ani.ResetTrigger("Dodge");
        ani.ResetTrigger("Dash");
        if (owner.ultScreen)
        { 
            ani.ResetTrigger("Gun");
            ani.ResetTrigger("Sword");
            ani.ResetTrigger("Hammer");
            ani.ResetTrigger("Kunai");
            ani.ResetTrigger("Potion");
            ani.ResetTrigger("Ult1");
            ani.ResetTrigger("Ult2");
        }
    }

    void StateChange(AnimationState newState) 
    {
        State = newState;
        ani.ResetTrigger("Jump");
        ani.ResetTrigger("Breaktime");
    }
    public void Hit(float power)
    {
        DynamicCamera actionCam = Camera.main.GetComponent<DynamicCamera>();
        if (Math.Abs(power) > 10)
        {
            if(actionCam)
                actionCam.ShakeScreen(5);

            if (!owner.stun)
            {
                ani.SetTrigger("Hit_Upgrade");
            }
            else 
            {

                ani.SetTrigger("Stun");
                owner.stun = false;
            }
        }
        else
        {
            if (actionCam)
                actionCam.ShakeScreen(5);
            if (!owner.stun) 
            {
                ani.SetTrigger("Hit");
            }
            else
            {
                ani.SetTrigger("Stun");
                owner.stun = false;
            }
        }

    }

    public void FallDown()
    {
        ani.SetTrigger("ComboEnd");
    }

    public void Die()
    {
        ani.SetTrigger("Death");
    }

    public void Network_SetTrigger(string name) 
    {
        if (name == "Potion")
            ani.SetTrigger("Potion_Quick");
        else if (name == "Kunai")
            ani.SetTrigger("Kunai_Quick");
        else if (name == "Gun")
        {
            if (owner.movement.StopMove)
                ani.SetTrigger("Gun");
            else
                ani.SetTrigger("Gun_Quick");
        }
        else if (name == "Ult1") 
        {
            ani.SetTrigger("Ult1_Quick");
        }
        else if(name == "Ult2") 
        {
            ani.SetTrigger("Ult2_Quick");
        }
        else
            ani.SetTrigger(name);
    } 

    public void Network_Effect()
    {
        RaycastHit2D hit;
        if (hit = Physics2D.Raycast(transform.position - new Vector3(0, transform.localScale.y / 2), Vector3.down, 500f))
        {
            GameObject effect = Instantiate(temp);
            effect.transform.position = hit.point;
        }
    }

    void PlayerAnimation() // 조종하는 플레이어 캐릭터의 애니메이션 관리 -> 입력에 반응
    {
        if ((Physics2D.Raycast(transform.position - new Vector3(0, transform.localScale.y/2), Vector3.down, 0.2f) || !additionalJump) && Input.GetKeyDown(Jump) && !Input.GetKey(DownArrow))
        {
            if (!onGround)
            {
                additionalJump = true;
                if (Ec && Ec.jumpDustEffectPrefab) 
                {
                    GameObject eff = GameObject.Instantiate(Ec.jumpDustEffectPrefab);
                    eff.transform.position = gameObject.transform.position - new Vector3(0,0.5f,0);
                }
            }

            State = AnimationState.Jump;
            ani.SetTrigger("Jump");
        }
        if (State == AnimationState.Normal) 
        {
            if (Input.GetAxis("Horizontal") == 0 && Input.GetKeyDown(Emotion_1))
            {
                State = AnimationState.Emotion;
                ani.SetTrigger("Breaktime");
            }
            if (Input.GetKeyDown(Punch))
            {
                if (Input.GetKey(UpArrow))
                {
                    ani.SetTrigger("Punch_Up");
                }
                else if (!ani.GetBool("Down"))
                {
                    ani.SetTrigger("Punch");
                }
            }
            if (Input.GetKeyDown(Kick))
            {
                ani.SetTrigger("Kick");
            }
            if (!owner.movement.is2P && !owner.mpLock)
            {
                int skillNumberKey = -1;
                if (Input.GetKeyDown((KeyCode.Keypad7)))
                {
                    skillNumberKey = 0;
                }
                else if (Input.GetKeyDown((KeyCode.Keypad8)))
                {
                    skillNumberKey = 1;
                }
                else if (Input.GetKeyDown((KeyCode.Keypad9)))
                {
                    skillNumberKey = 2;
                }
                else if (Input.GetKeyDown((KeyCode.KeypadDivide)))
                {
                    skillNumberKey = 3;
                }
                else if (Input.GetKeyDown((KeyCode.KeypadMultiply)))
                {
                    skillNumberKey = 4;
                }
                else if (Input.GetKeyDown((KeyCode.KeypadMinus)))
                {
                    skillNumberKey = 5;
                }

                if (skillNumberKey != -1 && skillManager)
                {
                    if (skillManager.skills[skillNumberKey] != "")
                    {
                        ani.SetTrigger(skillManager.skills[skillNumberKey]);
                    }
                }
            }
            else
                for (int i = 0; i < 6; i++)
                {
                    if (Input.GetKeyDown((KeyCode)(i + 49)))
                    {
                        if (skillManager)
                        {
                            if(skillManager.skills[i] != "") 
                            {
                                ani.SetTrigger(skillManager.skills[i]);
                            }
                        }
                    } 
                }
            if (Input.GetKey(DownArrow))
            {
                ani.SetBool("Down", true);
            }
            if (Input.GetKeyUp(DownArrow))
            {
                ani.SetBool("Down", false);
            }
            if (Input.GetKey(Guard))
            {
                ani.SetBool("Defense", true);
            }
            if (Input.GetKeyUp(Guard))
            {
                ani.SetBool("Defense", false);
            }
            if (Input.GetKeyDown(Dash) && !ani.GetBool("Down"))
            {
                ani.SetTrigger("Dash");
            }
            if (Input.GetKeyUp(Catch))
            {
                ani.SetTrigger("Catch");
            }
            if (Input.GetKeyUp(Backstep))
            {
                ani.SetTrigger("Dodge");
            }
            if (Input.GetKey(Heal))
            {
                ani.SetBool("Heal", true);
            }
            if (Input.GetKeyUp(Heal))
            {
                ani.SetBool("Heal", false);
            }
        }
    }
}
