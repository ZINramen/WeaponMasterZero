using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line_Ctrl : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private enum LineType
    {
        HorizontalLine,
        VerticalLine
    }

    [SerializeField] private LineType linetype;

    [SerializeField] private float maxXPos;
    [SerializeField] private float minXPos;

    [SerializeField] private float maxYPos;
    [SerializeField] private float minYPos;

    private EdgeCollider2D edge_Coll;
    private List<Vector2> Line_Position_List = new List<Vector2>();
    private List<Vector2> dummy_Position_List = new List<Vector2>();
    public Material HitMaterial;

    private Vector2 startPosision;
    private Vector2 endPosision;

    public float animationDuration = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = this.gameObject.GetComponent<LineRenderer>();
        edge_Coll = this.gameObject.GetComponent<EdgeCollider2D>();

        if (linetype == LineType.HorizontalLine)
        {
            float randomY = Random.Range(0, maxYPos);

            startPosision = new Vector2(maxXPos, randomY);
            Line_Position_List.Add(startPosision);

            randomY = Random.Range(minYPos, maxYPos);

            endPosision = new Vector2(minXPos, randomY);
            Line_Position_List.Add(endPosision);
        }

        else
        {
            float randomX = Random.Range(minXPos + 0.5f, maxXPos - 0.5f);

            startPosision = new Vector2(randomX, maxYPos + 1f);
            Line_Position_List.Add(startPosision);

            randomX = Random.Range(minXPos, maxXPos);

            endPosision = new Vector2(randomX, minYPos - 1f);
            Line_Position_List.Add(endPosision);
        }

        StartCoroutine(AnimateLine(startPosision, endPosision));
    }

    IEnumerator AnimateLine(Vector3 startPos, Vector3 endPos)
    {
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            lineRenderer.SetPosition(0, Vector3.Lerp(endPos, startPos, elapsedTime / animationDuration));
            lineRenderer.SetPosition(1, endPos);
            edge_Coll.SetPoints(Line_Position_List);

            elapsedTime += Time.deltaTime;
            yield return null;
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
}
