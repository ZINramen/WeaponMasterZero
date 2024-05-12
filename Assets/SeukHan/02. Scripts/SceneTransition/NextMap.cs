using UnityEngine;

public class NextMap : MonoBehaviour
{
    [SerializeField]
    private string nextRoom;
    [SerializeField]
    private string transitionName;
    
    public void GoToRoom()
    {
        LevelManager.Instance.LoadScene(nextRoom, transitionName);
    }
}
