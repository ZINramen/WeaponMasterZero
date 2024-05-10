using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Entity owner;

    float h;
    Rigidbody2D body;
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
    public bool StopMove = false;

    [Header("2P Move : I J K L")]
    public bool is2P = false;
    public PhysicsMaterial2D pMaterial;

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
        Application.targetFrameRate = 60;
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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
            body.sharedMaterial = pMaterial;
            if (PlayerType)
                Move();
            else if (owner.ai)
                AIMove();
        }
    }

    // Update is called once per frame
    void Update()
    {
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
            animator.SetBool("isWalk", true);
            if (h < 0)
                transform.localEulerAngles = new Vector3(0, 180, 0);
            else
                transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else
            animator.SetBool("isWalk", false);
    }
    private void AIMove()
    {

        if (Mathf.Abs(owner.ai.player.transform.position.x - transform.position.x) < 1) 
        {
            animator.SetBool("isWalk", false);
            return; 
        }
        

        if (owner.ai.player.transform.position.x < transform.position.x)
            h = -1;
        else if(owner.ai.player.transform.position.x > transform.position.x)
            h = 1;
        
        body.velocity = new Vector2(h * 100 * speed * Time.deltaTime, body.velocity.y);

        if (h != 0)
        {
            animator.SetBool("isWalk", true);
            if (h < 0)
                transform.localEulerAngles = new Vector3(0, 180, 0);
            else
                transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else
            animator.SetBool("isWalk", false);
    }

    public void Jump(float bonus_value)
    {
        if (bonus_value == 0)
            body.velocity = new Vector2(body.velocity.x, JumpPower * 2);
        else
            body.velocity = new Vector2(body.velocity.x, bonus_value * 2);
    }
    public void SetMovementForceX(float x)
    {
        int plus = 1;
        if (transform.localEulerAngles.y == 180) plus = -1;
        body.AddForce(new Vector2(x * 100 * plus, 0));
    }
    public void SetThrustForceX(float x)
    {        
        body.AddForce(new Vector2(x * 30, 0));
    }

    public void SetVelocityZero()
    {
        body.velocity = new Vector3(0, 0, 0);
    }
    public void UnFreeze()
    {
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void Freeze()
    {
        body.constraints = RigidbodyConstraints2D.FreezePositionY & RigidbodyConstraints2D.FreezeRotation;
    }
}
