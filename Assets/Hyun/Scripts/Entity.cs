using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class Entity : MonoBehaviour
{
    DefenseStatus EarlyStatus;
    public Material hitMat;
    Material oringMat;
    SpriteRenderer sp;

    // final boss 용
    public bool activeDesireWeapon = false; 
    public Entity playerFinalBoss;
    public int desireWeaponFinalBoss = 0;
    ////////////////////////////////////////

    public bool stun = false;
    private int[] keyValues;

    //public bool Network_Catch = false;

    [SerializeField] private float hp;
    [SerializeField] private int mp;
    private float attackForce = 0;
    private float thrustpower = 0;

    public Movement movement;
    public AISystem ai;

    public float waitTime = 0;

    public bool mpLock = false;
    public float maxHP = 1000;
    public int maxMp = 100;
    public LayerMask target;
    public GameObject attackPos;
    public float attackLength;
    public bool isDie = false; //캐릭터가 죽었는지 살았는지 여부
    public UnityEvent OnDie;

    public float flyingAttackForce = 0;
    public float flyingDamagedPower = 0;

    public AnimationManager aManager;

    public enum DefenseStatus { Nope, Guard, invincible, Warning }

    public DefenseStatus DamageBlock = DefenseStatus.Nope;


    [Header("Combo")]
    [SerializeField] private int currentCombo = 0;
    public int maxcombo = 6;
    //public ComboView ComboUI;

    [Header("Additional Effect")]
    public bool isDamaged = false;
    public GameObject HitEffect;
    public GameObject StrongHitEffect;

    public GameObject HitTextEffect;
    public GameObject StrongHitTextEffect;

    public GameObject CoolTextEffect;

    public EmoticonController emoticon;
    public RectTransform ultScreen;

    GameObject GameoverUI;

    public static Entity Player;

    private void Awake()
    {
        // keyValues = (int[])System.Enum.GetValues(typeof(KeyCode));
        if (tag == "Player")
            Player = this;
        movement = GetComponent<Movement>();
        if (!movement)
            movement = gameObject.AddComponent<Movement>();
        aManager = GetComponent<AnimationManager>();

        if (aManager)
            aManager.owner = this;

        if (movement)
            movement.owner = this;
        hp = maxHP;
        mp = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SetMp(maxMp);
        }
        if (isDie)
            return;
        if (ultScreen)
        {
            if (transform.localEulerAngles.y != 0)
            {
                ultScreen.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                ultScreen.rotation = Quaternion.Euler(0, 180, 0);
            }

        }
        if (maxHP == 9999) // 무제한 체력
            hp = 9999;

        if (attackPos)
        {
            Vector3 start = attackPos.transform.position;
            Debug.DrawRay(start, attackPos.transform.right * attackLength, Color.red);
        }
        if (waitTime > 0)
        {
            waitTime -= Time.deltaTime;
        }
        else if (waitTime < 0)
        {
            waitTime = 0;
        }
        //생사 상태 확인
        if (hp <= 0)
        {
            if (movement && movement.PlayerType && !GameoverUI)
            {
                GameoverUI = (GameObject)Instantiate(Resources.Load("UI/GameoverUI"));
            }
            DamageBlock = DefenseStatus.invincible;
            if (aManager)
            {
                aManager.Die();
            }
            else
            {
                this.gameObject.GetComponent<Animator>().SetTrigger("Die");
            }

            hp = 0;
            movement.enabled = false;
            //movement.body.constraints = RigidbodyConstraints2D.FreezeAll;
            isDie = true;
            if (ai)
                ai.enabled = false;

            if (CompareTag("Player") == false && CompareTag("Boss") == false)
            {
                Destroy(gameObject, 2);
            }
            OnDie.Invoke();
        }
    }

    public void SupermanState(bool value)
    {
        EarlyStatus = DamageBlock;
        if (value)
            DamageBlock = DefenseStatus.invincible;
        else
            DamageBlock = EarlyStatus;
    }
    public void Destroy_Entity(float delay)
    {
        Destroy(gameObject, delay);
    }
    public int GetMp()
    {
        return mp;
    }
    public void AddMp(int value)
    {
        if (mpLock) return;
        mp += value;
        if (mp > maxMp)
            mp = maxMp;
    }
    public void ResetMp()
    {
        mp = 0;

    }
    public void SetPower(float powerValue)
    {
        attackForce = powerValue;
        thrustpower = powerValue * 0.5f;
    }
    public void Instant_Attack() // 기존 이름 : Attack => 그러므로 애니메이션 이벤트로 해당 함수 호출한 캐릭터의 이전 애니메이션은 사용 시 수정해야 함.
    {
        bool attackAlready = false;
        Vector2 start = attackPos.transform.position;
        RaycastHit2D[] hit;
        hit = Physics2D.RaycastAll(start, transform.right, attackLength, target);

        foreach (RaycastHit2D hitTarget in hit)
        {
            if (hitTarget.collider.gameObject != gameObject)
            {
                Entity enemy = hitTarget.collider.gameObject.GetComponent<Entity>();
                if (!attackAlready && enemy)
                {
                    mp += 2;
                    if (enemy.ai && ai) return;
                    attackAlready = true;
                    float temp = enemy.GetHp();
                    enemy.flyingDamagedPower = flyingAttackForce;
                    if (transform.position.y - enemy.transform.position.y < 0)
                        enemy.Damaged(attackForce, -thrustpower);
                    else
                        enemy.Damaged(attackForce, thrustpower);
                    if (temp > enemy.GetHp())
                    {
                        AddMp(5);
                    }
                }
            }
        }
    }
    public void Teleport()
    {
        Vector2 start = transform.position;
        RaycastHit2D[] hit;
        hit = Physics2D.RaycastAll(start, transform.right, 10, target);

        foreach (RaycastHit2D hitTarget in hit)
        {
            if (hitTarget.collider.gameObject != gameObject)
            {
                Entity enemy = hitTarget.collider.gameObject.GetComponent<Entity>();
                if (enemy)
                {
                    if (enemy.transform.position.x <= transform.position.x)
                        transform.position = new Vector2(enemy.transform.position.x + 0.5f, transform.position.y);
                    else
                        transform.position = new Vector2(enemy.transform.position.x - 0.5f, transform.position.y);

                    break;
                }
            }
        }
    }
    // 날라가지 않고 데미지만
    public void Internal_Damaged(float damageValue)
    {
        if (DamageBlock == DefenseStatus.invincible) return;
        AddMp(5);
        if (waitTime == 0)
        {
            hp -= damageValue;
            waitTime = 0.2f;
        }
    }
    public void SetHpNetwork(float value)
    {
        if (value > maxHP)
            hp = maxHP;
        else hp = value;
    }
    public void SetMp(int value)
    {
        if (value > maxMp)
            mp = maxMp;
        else mp = value;
    }

    // hp 임의 변경 : 아이템용이다.
    public void SetHp(float value, bool noeffect = false)
    {
        if (hp > value)
        {
            if (!noeffect)
                PlayHitEffect(10);
            AddMp(2);
        }
        if (value > maxHP)
            hp = maxHP;
        else hp = value;

    }
    public float GetHp()
    {
        return hp;
    }

    public void SuperDamaged(float damageValue, float thrustValue)
    {
        if (DamageBlock == DefenseStatus.invincible) return;
        stun = true;
        Damaged(damageValue, thrustValue);
    }

    public void Damaged(float damageValue, float thrustValue = 0.5f)
    {
        if (activeDesireWeapon 
            && playerFinalBoss && playerFinalBoss.aManager.ani.GetInteger("Weapon") != desireWeaponFinalBoss) 
            return;

        if (DamageBlock == DefenseStatus.invincible) return;
        if (isDie) return;
        if(!isDamaged)
            StartCoroutine(ShakingEntity(0.15f,0.5f));
        if (hitMat != null)
        {
            StartCoroutine(DamagedEffectMat());
        }
        DynamicCamera actionCam = Camera.main.GetComponent<DynamicCamera>();

        if (Mathf.Abs(damageValue) > 30)
        {
            if (actionCam)
                actionCam.ShakeScreen(1f);
        }
        else
        {
            if (actionCam)
                actionCam.ShakeScreen(0.5f);
        }

        if (gameObject.CompareTag("Boss")) 
        {
            flyingDamagedPower = 0;
            thrustValue = 0;
        }
        if (DamageBlock == DefenseStatus.invincible) return;
        AddMp(10);
        if (currentCombo < maxcombo && damageValue != 0)
        {
            currentCombo++;
        }
        if(currentCombo == maxcombo)
        {
            if(aManager)
                aManager.FallDown();
            currentCombo = 0;
        }

        if (waitTime == 0)
        {
            if (aManager)
            {
                if (DamageBlock != DefenseStatus.Guard || damageValue == 0)
                    aManager.Hit(damageValue);
            }
            else if (this.gameObject.GetComponent<Default_Monster>())
            {
                if (DamageBlock != DefenseStatus.Guard || damageValue == 0)
                    this.gameObject.GetComponent<Default_Monster>().HitMotion();
            }
            
            if (flyingDamagedPower != 0)
            {
                movement.Jump(flyingDamagedPower);
            }
            if (DamageBlock != DefenseStatus.Guard && damageValue != 0)
            {
                //if (ComboUI)
                //{
                //    Instantiate(ComboUI);
                //}

                // 맞는 방향으로 회전
                //if (!gameObject.CompareTag("Boss"))
                //{
                //    if (thrustValue < 0)
                //        transform.localEulerAngles = new Vector3(0, 0, 0);
                //    else
                //        transform.localEulerAngles = new Vector3(0, 180, 0);
                //}
                if (HitEffect && damageValue > 0)
                {
                    PlayHitEffect(damageValue);
                }
                hp -= damageValue;

            }
            else
            {

                hp -= (float)damageValue / 2;
                if (damageValue != 0)
                    Instantiate(CoolTextEffect).transform.position = transform.position;
            }
            if (DamageBlock == DefenseStatus.Warning)
            {

                hp -= 10;

            }
            waitTime = 0.05f;
            movement.StopMove = true;
            StartCoroutine(ThrustPlayer(thrustValue));
        }
    }

    IEnumerator ShakingEntity(float shakingForce, float reduceSpeed) 
    {
        isDamaged = true;

        Vector3 Origin;
        float offsetX;

        yield return new WaitForSeconds(0.01f);
        while (shakingForce > 0) 
        {
            Origin = transform.position;
            offsetX = Random.Range(-shakingForce, shakingForce);
            transform.position = Origin + new Vector3(offsetX, 0);
            shakingForce -= reduceSpeed * Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
            transform.position = Origin;
            yield return new WaitForSeconds(0.01f);
        }
        isDamaged = false;
        movement.StopMove = false;
    }

    IEnumerator ThrustPlayer(float thrustValue) 
    {
        yield return new WaitForSeconds(0.01f);
        if (movement)
        {
            movement.SetThrustForceX(thrustValue);
        }
    }

    public void HitEffectWhenFly() 
    {
        if(flyingDamagedPower != 0) 
        {
            PlayHitEffect(10);
            flyingDamagedPower = 0;
        }
    }

    public void RiskSkill(int value) 
    {
        SetHp(GetHp() - 5);
    }

    public void PlayHitEffect(float damageValue)
    {
        if (damageValue < 35)
        {
            Instantiate(HitEffect).transform.position = transform.position;
            Instantiate(HitTextEffect).transform.position = transform.position;
        }
        else
        {
            GameObject strongHit = Instantiate(StrongHitEffect);
            strongHit.transform.position = transform.position;
            if (transform.localEulerAngles.y != 0)
                strongHit.transform.localEulerAngles = new Vector3(0, 0, 0);
            else
                strongHit.transform.localEulerAngles = new Vector3(0, -180, 0);
            Instantiate(StrongHitTextEffect).transform.position = transform.position;
        }
    }

    IEnumerator DamagedEffectMat()
    {
        foreach (SpriteRenderer sp in GetComponentsInChildren<SpriteRenderer>())
        {
            if (oringMat == null)
            {
                oringMat = sp.material;
            }
            sp.material = hitMat;
        }
        yield return new WaitForSeconds(0.3f);
        foreach (SpriteRenderer sp in GetComponentsInChildren<SpriteRenderer>())
        {
            sp.material = oringMat;
        }
        
    }
}
