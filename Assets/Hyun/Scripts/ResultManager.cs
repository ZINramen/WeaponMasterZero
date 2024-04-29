using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public static bool isWinner1P = false;

    public GameObject[] player1pWin;
    public GameObject[] player2pWin;

    public static int KillCount;

    public Text score;
    public Text bestScore;

    public bool sync = false;

    private void Awake()
    {
        if (PlayerPrefs.GetInt("BestScore", 0) < KillCount)
        {
            PlayerPrefs.SetInt("BestScore", KillCount);
        }

        if (score)
            score.text = KillCount.ToString();

        if (bestScore)
            bestScore.text = PlayerPrefs.GetInt("BestScore", 0).ToString();

        if (isWinner1P)
        {
            foreach (GameObject obj in player1pWin)
            {
                obj.SetActive(true);
            }
        }
        else if (player2pWin.Length > 0)
        {
            foreach (GameObject obj in player2pWin)
                obj.SetActive(true);
        }
    }

    private void Update()
    {
        if (!sync) return;
        if (PlayerPrefs.GetInt("BestScore", 0) < KillCount)
        {
            PlayerPrefs.SetInt("BestScore", KillCount);
        }

        if (score)
            score.text = KillCount.ToString();

        if (bestScore)
            bestScore.text = PlayerPrefs.GetInt("BestScore", 0).ToString();

    }
}
