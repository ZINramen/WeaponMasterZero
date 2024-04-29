using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CastleCtrl : ObjectParent
{
    private GameObject physics;
    private SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        base.Start();
        physics = gameObject.transform.GetChild(0).gameObject;
        spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(PlayerCheckY());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator PlayerCheckY()
    {
        while (true)
        {
            if (players[players.Length - 2].transform.position.y > 0.2 || players[players.Length - 1].transform.position.y > 0.2 )
            {
                this.spriteRenderer.color = Color.white;
                physics.SetActive(true);
            }
            else
            {
                this.spriteRenderer.color = new Color(111f / 255f, 111f / 255f, 111f / 255f);
                physics.SetActive(false);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
