using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Movement : MonoBehaviour
{
    GameObject boss;
    public bool Alway; // 어떤 개체든 항상 movementForce로 움직일 수 있도록 함.

    public bool BlockDash = false; // 대시 불가
    public Vector3 mousePos;

    public Entity owner;

    [HideInInspector]
    public float h;

    [HideInInspector]
    public Rigidbody2D body;
    
    Animator animator;

    // Public Area
    [Header("Movement Value")]
    [Tooltip("이동 속도")]
    public float speed = 5f;
    public bool super = false;
 
    [Tooltip("점프력")]
    public float JumpPower = 5f;

    [Space]
    [Header("Addtional Setting")]
    [Tooltip("플레이어가 조종 가능 여부")]
    public bool PlayerType = false;

    [Tooltip("움직임 봉쇄")]
    public bool StopMove = false; // 이동키 입력 막음
    
    [Header("2P Move : I J K L")]
    public bool is2P = false;
    public PhysicsMaterial2D pMaterial;

    bool isTrace = false;
    Vector3 targetPosition;

    public void SetPlayerType(bool value) 
    {
        PlayerType = value;
    }

    public Rigidbody2D GetBody() 
    {
        return body;
    }

    // Start is called before the first frame update
    void Awake()
    {
        boss = GameObject.FindWithTag("Boss");
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void TracePlayerPos()
    {
        if (Entity.Player.aManager.groundCheck.GetOnGround)
        {
            animator.SetBool("EndRush", false);
            animator.SetTrigger("isRush");
            isTrace = true;
            targetPosition = Entity.Player.transform.position;
        }
    }

    public void SeePlayer()
    {

        if (Entity.Player.transform.position.x > transform.position.x)
        {
            transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }


    public void PhysicChange() 
    {
        if (StopMove)
        {
            body.sharedMaterial = null;
            h = 0;
        }
        else
        {
            if (body.sharedMaterial != pMaterial)
            {
                body.sharedMaterial = pMaterial;
                pMaterial.friction = 1;
                pMaterial.friction = 0;
            }
            if (PlayerType)
                Move();
            else if (owner && owner.ai)
                owner.ai.AIMove();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isTrace)
        {
            if (Mathf.Abs((int)transform.position.x - (int)targetPosition.x) > 0 || Mathf.Abs((int)transform.position.y - (int)targetPosition.y) < 1)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPosition.x, transform.position.y), Time.deltaTime * speed);
            }
            else
            {
                animator.SetBool("EndRush", true);
                isTrace = false;
            }
        }
        if (owner == Entity.Player)
        {
            if (h != 0)
            {
                animator.SetBool("isWalk", true);
            }
            else
                animator.SetBool("isWalk", false);
        }
        PhysicChange();
        if (super)
            SuperMove();
    }
    private void SuperMove() 
    {
        float ve,he;

        if (is2P)
            he = Input.GetAxisRaw("Horizontal_2P");
        else
            he = Input.GetAxisRaw("Horizontal");
        if (is2P)
            ve = Input.GetAxisRaw("Vertical2P");
        else
            ve = Input.GetAxisRaw("Vertical");

        body.AddForce((Vector3.up * ve * 3f) + Vector3.right * he * 3);
    }

    private void Move()
    {
        if (is2P) 
            h = Input.GetAxisRaw("Horizontal_2P");
        else 
            h = Input.GetAxisRaw("Horizontal");
        body.velocity = new Vector2(h * 100 * speed * Time.deltaTime, body.velocity.y);

        if (h != 0)
        {
            if (h < 0)
                transform.localEulerAngles = new Vector3(0, 180, 0);
            else
                transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }

    public void Jump(float bonus_value)
    {
        if (body.bodyType == RigidbodyType2D.Static)
            return;
        if (bonus_value == 0)
            body.velocity = new Vector2(body.velocity.x, JumpPower * 2);
        else
            body.velocity = new Vector2(body.velocity.x, bonus_value * 2);
    }
    public void SetMovementForceX(float x)
    {
        if (Alway || !boss)
        {
            int plus = 1;
            if (transform.localEulerAngles.y == 180) plus = -1;
            body.AddForce(new Vector2(x * 100 * plus, 0));
        }
    }
    public void SetThrustForceX(float x)
    {        
        body.AddForce(new Vector2(x * 30, 0));
    }

    public void EvasionMove(float x)
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 MoveDir = (mousePos - this.gameObject.transform.position).normalized;

        if (mousePos.x > this.transform.position.x) {
            if (this.transform.rotation.eulerAngles.y != 0)
                this.transform.Rotate(this.transform.rotation.x, 180, this.transform.rotation.z);
        }

        else {
            if (this.transform.rotation.eulerAngles.y == 0)
                this.transform.Rotate(this.transform.rotation.x, 180, this.transform.rotation.z);
        }
        
        body.AddForce(MoveDir *  x * 100);
    }

    public void SetVelocityZero()
    {
        if (body.bodyType != RigidbodyType2D.Static)
            body.velocity = Vector2.zero;
     
    }
    public void UnFreeze()
    {
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void Freeze()
    {
        body.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    public void PushBack(float force)
    {
        // 캐릭터가 뒤로 밀리는 힘을 가합니다.
        // 힘의 방향은 캐릭터의 앞쪽입니다.
        Vector2 direction = -transform.right;
        body.AddForce(direction * force, ForceMode2D.Impulse);
    }
}
