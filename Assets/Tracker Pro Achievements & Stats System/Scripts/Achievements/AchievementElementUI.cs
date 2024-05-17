using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TrackerPro
{

    public class AchievementElementUI : MonoBehaviour
    {
        public Image icon;
        public TextMeshProUGUI achievementTitle;
        public TextMeshProUGUI achievementDescription;
        public TextMeshProUGUI achievementReward;

        public Slider achivementValueSlider;
        public TextMeshProUGUI valueText;
        public GameObject completedPanel;
        public GameObject secretPanel;
        public Sprite lockIcon;

        public void SetAchievementStats(Achievement achievement)
        {
            
            //If Achievement is Secret and Not Completed
            if (achievement.secret && !achievement.completed)
            {
                secretPanel.SetActive(true);
                icon.sprite = lockIcon;
                achievementTitle.text = AchievementMaster.instance.TrackerProSettings.secretAchievementTitle;
                achievementDescription.text = AchievementMaster.instance.TrackerProSettings.secretAchievementDescription;
                achievementReward.text = AchievementMaster.instance.TrackerProSettings.secretRewardString;

                valueText.text = string.Format("{0}/{1}", "??", "??");
            }
            else if(achievement.secret && achievement.completed || !achievement.secret)
            { //If Achievement is secret and completed OR isn't a secret at all...
                secretPanel.SetActive(false);

                if(achievement.icon == null)
                {
                    icon.sprite = AchievementMaster.instance.TrackerProSettings.defaultSpriteForAchievements;
                }
                else
                {
                    icon.sprite = achievement.icon;

                }
               
                achievementTitle.text = achievement.title;
                achievementDescription.text = achievement.description;
                achievementReward.text = achievement.points.ToString();
                //Display as Percentage Functionality
                if (achievement.displayAsPercentage)
                {
                    valueText.text = string.Format("{0}%", ((float)achievement.value / (float)achievement.neededValue) * 100);
                }
                else
                {
                    valueText.text = string.Format("{0}/{1}", achievement.value, achievement.neededValue);
                }
                if(achievement.typeOfAchievement == Achievement.TypeOfAchievement.Numerical)
                {
                    
                }
                switch (achievement.typeOfAchievement)
                {
                    case Achievement.TypeOfAchievement.Numerical:
                        achivementValueSlider.gameObject.SetActive(true);
                        valueText.gameObject.SetActive(true);
                        achivementValueSlider.value = ((float)achievement.value / (float)achievement.neededValue) * 100;
                        break;
                    case Achievement.TypeOfAchievement.Boolean:
                        achivementValueSlider.gameObject.SetActive(false);
                        valueText.gameObject.SetActive(false);
                        break;
                    default:
                        break;
                }

            }

            if(achievement.completed)
            {
                secretPanel.SetActive(false);
                completedPanel.SetActive(true);
            }
            else
            {
                completedPanel.SetActive(false);
            }
        }
    }
}
