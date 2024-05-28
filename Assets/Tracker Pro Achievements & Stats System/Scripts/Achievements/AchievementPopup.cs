using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEditor;

namespace TrackerPro
{
    public class AchievementPopup : MonoBehaviour
    {
        [SerializeField]
        private Settings settings;

        [SerializeField]
        private TextMeshProUGUI achievementTitleText;

        [SerializeField]
        private Image achievementIconImage;

        [SerializeField]
        private Animator animator;


        private void Start()
        {
            if(animator == null)
            {
                animator.GetComponent<Animator>();
            }
            TrackerProEvents.AchievementGranted += ShowAchievementAndSetValues;
        }

        public void ShowAchievementAndSetValues(Achievement achievement)
        {
            animator.Play("AchievementGrantedAnimation", 0, 0f);
            string path = achievement.trimmedIconPath;

            if (string.IsNullOrEmpty(path))
            {
                int startIndex = achievement.iconPath.IndexOf("Resources/") + "Resources/".Length;
                int endIndex = achievement.iconPath.LastIndexOf('.');
                path = endIndex > startIndex ? achievement.iconPath.Substring(startIndex, endIndex - startIndex) : achievement.iconPath.Substring(startIndex);
            }

            achievementIconImage.sprite = Resources.Load<Sprite>(path);
            achievementTitleText.text = string.Format("{0} completed!", achievement.title);
        }
    }

}


