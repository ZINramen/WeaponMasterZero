using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line_Ctrl : MonoBehaviour
{
    private LineRenderer lineRenderer;
    
    private EdgeCollider2D edge_Coll;
    List<Vector2> Line_Position_List;
    private Vector2 anim_StartPos;

    private Vector2 randomPos;
    private float animationDuration = 0.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = this.gameObject.GetComponent<LineRenderer>();
        Line_Position_List = new List<Vector2>();

        randomPos = new Vector2(10, 1);
        Line_Position_List.Add(randomPos);
        
        randomPos = new Vector2(-10, 3);
        Line_Position_List.Add(randomPos);
        
        //Invoke("Create_Line", 1f);
        StartCoroutine(AnimateLine());
    }

    private void Create_Line()
    {
        lineRenderer.SetPosition(1, new Vector2(-10, 3));
        
        edge_Coll = this.gameObject.AddComponent<EdgeCollider2D>();
        edge_Coll.SetPoints(Line_Position_List);
    }
    
    IEnumerator AnimateLine()
    {
        float elapsedTime = 0f;
        Vector3 startPos = new Vector2(10, 1);
        Vector3 endPos = new Vector2(-10, 3);

        while (elapsedTime < animationDuration)
        {
            lineRenderer.SetPosition(0, Vector3.Lerp(endPos, startPos, elapsedTime / animationDuration));
            lineRenderer.SetPosition(1, endPos);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
