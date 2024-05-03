using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Venogear2DPlatformer
{
    public class Shopkeeper : MonoBehaviour
    {
        public bool reverse = false;
        public Transform player;
        public float distance;
        public GameObject SpeechCloud;
        private SpriteRenderer ShopkeeperSprite;

        void Start()
        {
            ShopkeeperSprite = GetComponent<SpriteRenderer>();
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            player = go.transform;
        }

        void FixedUpdate()
        {
            distance = Vector2.Distance(transform.position, player.transform.position);

            if (distance >= 3)
            {
                SpeechCloud.SetActive(false);
            }
            else SpeechCloud.SetActive(true);

            if (transform.position.x <= player.transform.position.x)
            {
                ShopkeeperSprite.flipX = !reverse;
            }
            else
            {
                ShopkeeperSprite.flipX = reverse;
            }

        }

    }
}
