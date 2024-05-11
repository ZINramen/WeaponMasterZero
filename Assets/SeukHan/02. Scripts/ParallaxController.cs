using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ParallaxController : MonoBehaviour
{
    private Transform cam; // Main Camera;
    private Vector3 camStartPos;
    private float distance;
    
    GameObject[] backgrounds;
    Material[] mats;
    private float[] backSpeeds;

    private float farthestBack;
    
    [Range(0.01f, 0.5f)] public float parallaxpeed;

    private void Start()
    {
        cam = Camera.main.transform;
        camStartPos = cam.position;
        
        int backCount = transform.childCount;
        mats = new Material[backCount];
        backSpeeds = new float[backCount];
        backgrounds = new GameObject[backCount];
        
        for(int i = 0; i < backCount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            mats[i] = backgrounds[i].GetComponent<Renderer>().material;
        }
        
        BackSpeedCalculate(backCount);
    }

    void BackSpeedCalculate(int backCount)
    {
        for (int i = 0; i < backCount; i++)
        {
            if((backgrounds[i].transform.position.z - cam.position.z) > farthestBack)
            {
                farthestBack = backgrounds[i].transform.position.z - cam.position.z;
            }
        }
        
        for(int i = 0; i < backCount; i++)
        {
            backSpeeds[i] = 1 - (backgrounds[i].transform.position.z - cam.position.z) / farthestBack;
        }
    }

    private void LateUpdate()
    {
        distance = cam.position.x - camStartPos.x;
        transform.position = new Vector3(cam.position.x, cam.position.y, 3); //can you lerp this??
        
        for (int i = 0; i < backgrounds.Length; i++)
        {
            float speed = backSpeeds[i] * parallaxpeed;
            mats[i].SetTextureOffset("_MainTex", new Vector2(distance, 0) * speed);
        }
    }
}
