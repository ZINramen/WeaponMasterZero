using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrackerPro
{
    public class DemoTestScript : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                AchievementMaster.AddValueToAchievement(1, 1);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                for (int i = 0; i < AchievementMaster.achievementMaster.Count; i++)
                {
                    AchievementMaster.AddValueToAchievement(i, 1);
                }
            }
            
            if(Input.GetKeyDown(KeyCode.S))
            {
                AchievementMaster.SaveAchievementFile(AchievementMaster.achievementMaster);
            }

            if(Input.GetKeyDown(KeyCode.M))
            {
                AchievementMaster.UnlockSecretAchievement(1);
            }
        }
    }


}


