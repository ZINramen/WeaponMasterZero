using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EffectCreator : MonoBehaviour
{
    DynamicCamera cam;
    PhotonPlayer network;

    public Transform HammerEffect;

    private void Start()
    {
        network = GetComponent<PhotonPlayer>();
        cam = Camera.main.GetComponent<DynamicCamera>();
    }

    public void CameraDownEffect(float strength)
    {
        cam.downOffsetY = strength;
    }

    public void PlayEffectWithTransform(string name)
    {
        if(name == "Hammer" && HammerEffect)
        {
            GameObject temp = Resources.Load<GameObject>("Effects/StrongHitEffect");
            temp = Instantiate(temp);
            temp.transform.position = HammerEffect.position - new Vector3(0,0.2f,0);
            temp.transform.localEulerAngles = new Vector3(0, 0, 90);
            temp.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }
        else
        {
            GameObject temp = Resources.Load<GameObject>("Effects/" + name);
            temp = Instantiate(temp);
            temp.transform.position = transform.root.position;
            temp.transform.localEulerAngles = new Vector3(0, transform.root.localEulerAngles.y, 0);
            
            HitColider dot = temp.GetComponent<HitColider>();
            if (dot)
                dot.owner = transform.root.GetComponent<Entity>();
        }
    }

    public void PlayEffect(string effectName, RaycastHit2D hit)
    {
        GameObject temp = Resources.Load<GameObject>("Effects/" + effectName);
        temp = Instantiate(temp);
        temp.transform.position = hit.point;
        HitColider hitAction = temp.GetComponent<HitColider>();
        if (hitAction)
        {
            temp.GetComponent<HitColider>().owner = gameObject.GetComponent<Entity>();
        }
        if (network) 
        {
            network.NetworkSyncEffect(hit.point);
        }
    }
}
