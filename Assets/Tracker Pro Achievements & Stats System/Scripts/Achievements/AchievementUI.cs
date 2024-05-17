using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrackerPro
{

    public class AchievementUI : MonoBehaviour
    {
        public Transform parentListObject;
        private List<AchievementElementUI> achievementPrefabList = new List<AchievementElementUI>();

        // Start is called before the first frame update
        void Start()
        {
            
            BuildAchievementUIList();
            TrackerProEvents.AchievementValueChanged += RefreshUIValues;
        }

        public void BuildAchievementUIList()
        {
            for (int i = 0; i < AchievementMaster.achievementMaster.Count; i++)
            {
                //Instantiate the UI Element
                achievementPrefabList.Add(Instantiate(AchievementMaster.instance.TrackerProSettings.achievementElementUI));

                //Set it's parent to the parentListObject
                achievementPrefabList[i].transform.SetParent(parentListObject, false);
            }

            //Now we populate the data

            for (int i = 0; i < achievementPrefabList.Count; i++)
            {
                achievementPrefabList[i].SetAchievementStats(AchievementMaster.achievementMaster[i]);
            }
        }

        public void RefreshUIValues(Achievement achievement)
        {
            for (int i = 0; i < AchievementMaster.achievementMaster.Count; i++)
            {
                if (AchievementMaster.achievementMaster[i].id == achievement.id)
                {
                    achievementPrefabList[i].SetAchievementStats(achievement);
                }
            }
        }

    }
}
