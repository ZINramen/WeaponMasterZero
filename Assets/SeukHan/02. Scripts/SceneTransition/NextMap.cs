using UnityEngine;
using UnityEngine.SceneManagement;

public class NextMap : MonoBehaviour
{
    [SerializeField]
    private string nextRoom;
    [SerializeField]
    private string transitionName;
    
    public void GoToRoom()
    {
        SavePoint.save = false;
        LevelManager.Instance.LoadScene(nextRoom, transitionName);

        if (Entity.Player)
        {
            Entity.Player.movement.Freeze();
            Entity.Player.DamageBlock = Entity.DefenseStatus.invincible;
        }
    }
}
