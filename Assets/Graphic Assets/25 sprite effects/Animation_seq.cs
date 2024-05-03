using UnityEngine;
using System.Collections;

public class Animation_seq: MonoBehaviour
{
    public float fps = 24.0f;
    public Texture2D[] frames;

    private int frameIndex;
    private MeshRenderer rendererMy;
    public bool loop = false;

    void Start()
    {
        rendererMy = GetComponent<MeshRenderer>();
        NextFrame();
        InvokeRepeating("NextFrame", 1 / fps, 1 / fps);
    }
    private void Update()
    {

        if (transform.root.localEulerAngles.y != 0)
        {
            transform.localEulerAngles = new Vector3(-270, 90, 90);
        }
        else
        {
            transform.localEulerAngles = new Vector3(-270, -90, 90);
        }
    }
    void NextFrame()
    {
        rendererMy.sharedMaterial.SetTexture("_MainTex", frames[frameIndex]);
        frameIndex = (frameIndex + 0001) % frames.Length;
        if(frameIndex == frames.Length - 1) 
        {
            if(!loop)
                Destroy(this.gameObject, 1 / fps);
        }
    }
}