using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    [Header("Cinemachine")] public Vector2 originCamOffset;
    public CinemachineCameraOffset CamOffset;
    
    [Header("Camera Area Control")]
    public float offsetX = 0;
    public float offsetY = 0;

    public float downOffsetY = 0;

    [Space]

    [Header("Camera Shake Control")]
    /// Camera Shake Power;
    public float shakePowerTemp = 0;
    public float shakeReductionSpeed = 5;

    [Space]

    [Header("Camera Speed Control")]
    // Camera Move Speed
    public float moveSpeed = 1;

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

        CamOffset.m_Offset = new Vector3(originCamOffset.x + offsetX, originCamOffset.y + offsetY);
    }
    
    // Camera Vibration
    public void ShakeScreen(float shakePower)
    {
        shakePowerTemp = shakePower;
    }
}
