using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhaseManager : MonoBehaviour
{
    public int idx = 0;
    public List<float> waitTimes;
    public List<GameObject> PhaseEnemys;
    public List<HingeJoint2D> ElevatorPlatformJoints;
    public List<Rigidbody2D> ElevatorPlatforms;

    public AISystem FinalEnemy;
    public bool EndPhase = false;

    public GameObject ScreenDown;
    public string nextScene;

    public Animation BgAnim;

    public IEnumerator EndStage()
    {
        BgAnim.Stop();
        yield return new WaitForSeconds(8);
        ScreenDown.SetActive(true);
        yield return new WaitForSeconds(3);
        if(!Entity.Player.isDie)
            SceneManager.LoadScene(nextScene);
    }
    public IEnumerator PlayPhase()
    {
        while (PhaseEnemys.Count > 0)
        {
            yield return new WaitForSeconds(waitTimes[idx]);
            PhaseEnemys[0].SetActive(true);

            while (PhaseEnemys[0].transform.childCount > 0)
            {
                yield return new WaitForSeconds(3);
            }
            Destroy(PhaseEnemys[0]);
            PhaseEnemys.RemoveAt(0);
            idx++;
        }
    }
    
    public void BreakPlatform(int num)
    {
        if(num == ElevatorPlatforms.Count - 1) 
        {
            ElevatorPlatforms[num-1].constraints = RigidbodyConstraints2D.None;
            ElevatorPlatformJoints[num-1].enabled = false;
        }
        else
        {
            ElevatorPlatforms[num+1].constraints = RigidbodyConstraints2D.None;
            ElevatorPlatformJoints[num+1].enabled = false;
        }
        ElevatorPlatforms[num].constraints = RigidbodyConstraints2D.None;
        ElevatorPlatformJoints[num].enabled = false;
    }

    void Start()
    {
        StartCoroutine(PlayPhase());
    }

    // Update is called once per frame
    void Update()
    {
        if (!EndPhase)
        {
            if (FinalEnemy && FinalEnemy.eventEnd && FinalEnemy.transform.position.y <= -1)
            {
                int breakNum = 0;
                int idx = 0;
                float distance, minDistance = 9999;
                foreach (Rigidbody2D ElevatorPlatform in ElevatorPlatforms)
                {
                    distance = Vector3.Magnitude(ElevatorPlatform.transform.position - FinalEnemy.transform.position);
                    if (distance <= minDistance)
                    {
                        minDistance = distance;
                        breakNum = idx;
                    }
                    idx++;
                }
                EndPhase = true;
                BreakPlatform(breakNum);
                StartCoroutine(EndStage());
            }
        }
    }
}
