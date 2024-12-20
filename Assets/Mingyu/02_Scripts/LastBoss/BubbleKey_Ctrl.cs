using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum BubbleSponType
{
    UP_Spon,
    Right_Spon,
    Down_Spon,
    Left_Spon
}

public class BubbleData
{
    public BubbleSponType bubbleSponType;

    private Transform LeftUp_SponPos;
    private Transform RightDown_SponPos;

    public float bubblePower;
    
    public void Set_SponAblePos(Transform inputLeftUP, Transform inputRightDown)
    {
        LeftUp_SponPos = inputLeftUP;
        RightDown_SponPos = inputRightDown;
    }
    
    public void SetBubbleData(ref Vector2 sponPos)
    {
        int random_SponPos = Random.Range((int)BubbleSponType.UP_Spon, (int)BubbleSponType.Left_Spon + 1);
        
        switch (random_SponPos)
        {
            case (int)BubbleSponType.UP_Spon:
                bubbleSponType = BubbleSponType.UP_Spon;
                
                sponPos.x = Random.Range
                    (LeftUp_SponPos.position.x + 2f, RightDown_SponPos.position.x - 2f);

                sponPos.y = LeftUp_SponPos.position.y;
                break;
            
            case (int)BubbleSponType.Right_Spon:
                bubbleSponType = BubbleSponType.Right_Spon;

                sponPos.x = RightDown_SponPos.position.x;
                
                sponPos.y = Random.Range
                    (RightDown_SponPos.position.y + 3f, LeftUp_SponPos.position.y - 3f);
                break;
            
            case (int)BubbleSponType.Down_Spon:
                bubbleSponType = BubbleSponType.Down_Spon;
                
                sponPos.x = Random.Range
                    (LeftUp_SponPos.position.x + 2f, RightDown_SponPos.position.x - 2f);

                sponPos.y = RightDown_SponPos.position.y;
                break;
            
            default:
                bubbleSponType = BubbleSponType.Left_Spon;
                
                sponPos.x = LeftUp_SponPos.position.x;
                
                sponPos.y = Random.Range
                    (RightDown_SponPos.position.y + 3f, LeftUp_SponPos.position.y - 3f);
                break;
        }
    }
}

public class BubbleKey_Ctrl : MonoBehaviour
{
    public GameObject Player;
    public GameObject LastBoss;
    private GameObject particleObj;
    
    private BubbleData currentBubbleData;
    [SerializeField] private bool is_Pass = true;
    [SerializeField] private GameObject keyObj;

    private Rigidbody2D BubbleRd;
    private Vector2 Chaged_Pos;
    
    void Start()
    {
        BubbleRd = this.gameObject.GetComponent<Rigidbody2D>();

        AddForce(currentBubbleData.bubbleSponType, currentBubbleData.bubblePower);
    }

    public void SetBubbleData(ref BubbleData input_bubbleData)
    {
        currentBubbleData = new BubbleData();
        currentBubbleData = input_bubbleData;
    }
    
    public void AddForce(BubbleSponType bubbleSponType, float bubblePower)
    {
        while (BubbleRd.velocity.magnitude > 0.5f)
            BubbleRd.velocity = Vector2.zero;
        
        switch (bubbleSponType)
        {
            case BubbleSponType.UP_Spon:
                BubbleRd.AddForce(Vector2.down * bubblePower, ForceMode2D.Impulse);
                break;
            
            case BubbleSponType.Right_Spon:
                BubbleRd.AddForce(Vector2.left * bubblePower, ForceMode2D.Impulse);
                break;
            
            case BubbleSponType.Down_Spon:
                BubbleRd.AddForce(Vector2.up * bubblePower, ForceMode2D.Impulse);
                break;
            
            default:
                BubbleRd.AddForce(Vector2.right * bubblePower, ForceMode2D.Impulse);
                break;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (is_Pass != true && other.gameObject.tag == "Wall")
        {
            is_Pass = true;
            currentBubbleData.SetBubbleData(ref Chaged_Pos);
            this.gameObject.transform.position = Chaged_Pos;
            AddForce(currentBubbleData.bubbleSponType, currentBubbleData.bubblePower);
        }

        if (other.gameObject.GetComponent<HitColider>() &&
            other.gameObject.GetComponent<HitColider>().owner == Player.gameObject.GetComponent<Entity>())
        {
            particleObj = this.gameObject.transform.GetChild(0).gameObject;
            
            particleObj.SetActive(true);
            particleObj.gameObject.GetComponent<ParticleSystem>().Play();
            
            while (BubbleRd.velocity.magnitude > 0.5f)
                BubbleRd.velocity = Vector2.zero;

            this.gameObject.GetComponent<SpriteRenderer>().color =new Color(1f, 1f, 1f, 0);
                
            Invoke("Install_KeyObj", particleObj.gameObject.GetComponent<ParticleSystem>().duration);
            Destroy(this.gameObject, particleObj.gameObject.GetComponent<ParticleSystem>().duration);
        }
    }

    private void Install_KeyObj()
    {
        GameObject dummyKey = Instantiate(keyObj, this.gameObject.transform.position, Quaternion.identity);
        dummyKey.gameObject.GetComponent<Key_Ctrl>().Player = Player;
        dummyKey.gameObject.GetComponent<Key_Ctrl>().LastBoss = LastBoss;
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (is_Pass && other.gameObject.tag == "Wall")
            is_Pass = false;
    }
}
