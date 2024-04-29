using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGoomba : MonoBehaviour
{
    public Transform SpawnPoint;
    public GameObject goomba;
    public int GoombaCount
    { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SummonGoomba());
    }

    IEnumerator SummonGoomba()
    {
        while (true)
        {
            if (GoombaCount < 3 && PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Instantiate(goomba.name, SpawnPoint.position, Quaternion.identity);
            }

            yield return new WaitForSeconds(15.0f);
        }
    }
}
