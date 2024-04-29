using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Venogear2DPlatformer
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform player;
        public Vector3 offset;

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform; // Searching for a player by tag "Player"
            if (player == null)
            {
                Debug.LogError("Player not found!"); // Error output if player not found
            }
        }

        void LateUpdate()
        {
            if (player != null)
            {
                transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, offset.z); // Camera follows the player with specified offset position
            }
        }
    }
}