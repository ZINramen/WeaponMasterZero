using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{
    GameObject miniGameBall;

    int score1 = 0;
    int score2 = 0;
    int delayTime = 5;

    public DynamicCamera cam;
    public GameObject ball;
    public GameObject ballEffect;

    public GameObject GameEndUI;
    public GameObject GameEndUI1;
    public GameObject GameEndUI2;

    public Transform p1;
    public Transform p2;

    public Text ScoreT;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        StartCoroutine(SpawnBall(true));
    }

    // Update is called once per frame
    void Update()
    {
        ScoreT.text = score1.ToString() + " : " + score2.ToString(); 
        if (score1 > 9 || score2 > 9) 
        {
            GameEndUI.SetActive(true);
            if (score1 > 9)
                GameEndUI1.SetActive(true);
            else
                GameEndUI2.SetActive(true);
            if (miniGameBall != null)
            {
                Destroy(miniGameBall);
                miniGameBall = null;
            }
            return;
        }
        if (miniGameBall && miniGameBall.transform.position.y < 0.28f)
        {
            if (miniGameBall.transform.position.x < -0.3f)
            {
                score2++;
                StartCoroutine(SpawnBall(false)); 
            }
            else
            {
                score1++;
                StartCoroutine(SpawnBall(true));
            }
            GameObject.Instantiate(ballEffect).transform.position = miniGameBall.transform.position;
            cam.ShakeScreen(10);
            Destroy(miniGameBall);
            miniGameBall = null;
        }
    }

    IEnumerator SpawnBall(bool left)
    {
        yield return new WaitForSeconds(delayTime);
        delayTime = 2;
        if (!(score1 > 9 || score2 > 9))
        {
            miniGameBall = GameObject.Instantiate(ball);
            Transform ballTemp = miniGameBall.transform;

            if (left)
                ballTemp.transform.position = p1.position;
            else
                ballTemp.transform.position = p2.position;
        }
    }
}
