using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Venogear2DPlatformer
{
    public class ChangeSprite : MonoBehaviour
    {
        public GameObject parentGameObject;
        public Sprite newSprite; //parent main hand sprite
        private object spriteRenderer;

        // Copying sprite based on sprite in main hand to make outline using outline shader.
        void Start()
        {
            newSprite = parentGameObject.GetComponent<SpriteRenderer>().sprite;
            GetComponent<SpriteRenderer>().sprite = newSprite;
        }
    }
}