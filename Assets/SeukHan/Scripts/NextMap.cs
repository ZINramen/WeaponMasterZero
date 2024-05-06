using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextMap : MonoBehaviour
{
    public void GoToRoom(string roomName)
    {
        SceneManager.LoadScene(roomName);
    }
}
