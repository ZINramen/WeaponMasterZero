using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    [Header("Camera Auto Setting")]
    [Tooltip("Automatical Set Target")]
    public bool AutoSet = true;

    [Space]

    [Header("Camera Area Control")]
    public float height = 0;
    public float offsetX = 0;
    public float offsetY = 0;

    public float downOffsetY = 0;

    [Space]

    [Header("Camera Shake Control")]
    /// Camera Shake Power;
    public float shakePowerTest = 0;
    public float shakePowerTemp = 0;
    public float shakeReductionSpeed = 10;

    [Space]

    [Header("Camera Speed Control")]
    // Camera Move Speed
    public float moveSpeed = 1;

    [Space]

    [Header("Player Setting")]
    public Transform player1;
    public Transform player2;

    [Space]

    [Header("Camera Size Control")]
    // Camera Size Control
    public float size_AddValue = 1;
    public float size_OriginalValue;

    Camera cam;
    Vector3 targetingPoint;
    float targetingPointX;
    float targetingPointY;
    float farAmount;

    void Start()
    {
        cam = GetComponent<Camera>();
        size_OriginalValue = cam.orthographicSize;

        if (AutoSet)
        {
            Entity[] entitys = FindObjectsByType<Entity>(FindObjectsSortMode.None);
            player1 = entitys[0].transform;
            player2 = entitys[1].transform;
        }
    }

    void Update()
    {
        if (shakePowerTemp > 0)
            shakePowerTemp -= shakeReductionSpeed * Time.deltaTime;
       
        else if (shakePowerTemp < 0)
            shakePowerTemp = 0;

        if (downOffsetY > 0)
            downOffsetY -= shakeReductionSpeed * 2 * Time.deltaTime;
        else if (downOffsetY < 0)
            downOffsetY = 0;
    }

    void LateUpdate()
    {
        offsetX = Random.Range(-shakePowerTemp, shakePowerTemp);
        offsetY = Random.Range(-shakePowerTemp, shakePowerTemp);

        targetingPointX = (player1.position.x + player2.position.x) / 2;
        targetingPointY = (player1.position.y + player2.position.y) / 2 + height;

        targetingPoint = new Vector3(targetingPointX + offsetX, targetingPointY + offsetY - downOffsetY, transform.localEulerAngles.z);
        transform.position = Vector3.Lerp(transform.position, targetingPoint, moveSpeed * Time.deltaTime);
        
        farAmount = (player1.position- player2.position).magnitude * size_AddValue + size_OriginalValue;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, farAmount, moveSpeed * Time.deltaTime);
    }
    
    // Camera Vibration
    public void ShakeScreen(float shakePower)
    {
        shakePowerTemp = shakePower;
    }
}
