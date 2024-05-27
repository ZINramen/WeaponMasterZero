using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line_Ctrl : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Animator anim;

    private enum LineType
    {
        HorizontalLine,
        VerticalLine
    }

    [SerializeField] private LineType linetype;

    public Transform maxXYPos; // 오른쪽 위에 좌표
    public Transform minXYPos; // 왼쪽 아래 좌표
    private float randomY1;
    private float randomY2;
    
    private float randomX1;
    private float randomX2;

    private EdgeCollider2D edge_Coll;
    private List<Vector2> Line_Position_List = new List<Vector2>();
    private List<Vector2> dummy_Position_List = new List<Vector2>();
    public Material HitMaterial;

    private Vector2 startPosision;
    private Vector2 endPosision;

    public float animationDuration = 0.10f;
    private float lineSize = 0.15f;
    private float deltaSize = 0f;

    public float attTime = 0.1f;
    public float deleteTime;
    public GameObject SwordBoss;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.gameObject.GetComponent<Animator>();
        lineRenderer = this.gameObject.GetComponent<LineRenderer>();
        edge_Coll = this.gameObject.GetComponent<EdgeCollider2D>();
        
        lineRenderer.SetWidth(lineSize, lineSize);

        if (linetype == LineType.HorizontalLine)
        {
            randomY1 = Random.Range(0f, maxXYPos.position.y);
            randomY2 = Random.Range(0f, maxXYPos.position.y);

            if (randomY1 >= maxXYPos.position.y - 4f && randomY2 >= maxXYPos.position.y - 4f)
            {
                randomY2 = Random.Range(0f, maxXYPos.position.y - 3f);
            }

            startPosision = new Vector2(maxXYPos.position.x + 1.0f, randomY1);
            Line_Position_List.Add(startPosision);

            endPosision = new Vector2(minXYPos.position.x, randomY2);
            Line_Position_List.Add(endPosision);
        }

        else
        {
            randomX1 = Random.Range(minXYPos.position.x, maxXYPos.position.x);
            randomX2 = Random.Range(minXYPos.position.x, maxXYPos.position.x);

            if (randomX1 >= maxXYPos.position.x - 4f && randomX2 >= maxXYPos.position.x - 4f)
            {
                randomX2 = Random.Range(0f, maxXYPos.position.x - 3f);
            }
            else if (randomX1 <= maxXYPos.position.x - 7f && randomX2 <= maxXYPos.position.x - 7f)
            {
                randomX1 = Random.Range(0f, minXYPos.position.x + 3f);
            }
            
            startPosision = new Vector2(randomX1, maxXYPos.position.y);
            Line_Position_List.Add(startPosision);
            
            endPosision = new Vector2(randomX2, minXYPos.position.y);
            Line_Position_List.Add(endPosision);
        }

        StartCoroutine(AnimateLine(startPosision, endPosision));
    }

    IEnumerator AnimateLine(Vector3 startPos, Vector3 endPos)
    {
        float elapsedTime = 0f;

        while (elapsedTime <= animationDuration)
        {
            lineRenderer.SetPosition(0, Vector3.Lerp(endPos, startPos, elapsedTime / animationDuration));
            lineRenderer.SetPosition(1, endPos);
            edge_Coll.SetPoints(Line_Position_List);

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        
        if (elapsedTime > animationDuration)
        {
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
            edge_Coll.SetPoints(Line_Position_List);
        }
    }

    public void On_Collider()
    {
        this.gameObject.GetComponent<LineRenderer>().material = HitMaterial;
        this.gameObject.GetComponent<EdgeCollider2D>().enabled = true;
    }

    public void Off_Collider()
    {
        this.gameObject.GetComponent<EdgeCollider2D>().enabled = false;
    }

    public void EndAnimation()
    {
        Invoke("DelayAttackTime", attTime);
    }

    private void DelayAttackTime()
    {
        Off_Collider();
        StartCoroutine(DeleteAnimatoin_Line());
        
        Invoke("Delete_Setting", deleteTime);
    }

    private void Delete_Setting()
    {
        SwordBoss.gameObject.GetComponent<SwordBoss>().End_P2Skill3();
    }

    IEnumerator DeleteAnimatoin_Line()
    {
        float elapsedTime = 0f;

        while (elapsedTime <= lineSize)
        {
            elapsedTime += Time.deltaTime;
            deltaSize = lineSize - elapsedTime;
            
            Debug.Log(deltaSize);
            
            lineRenderer.SetWidth(deltaSize, deltaSize);
            yield return null;
        }
    }
}
