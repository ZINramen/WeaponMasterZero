using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrackerPro
{
    [CreateAssetMenu(fileName = "Settings", menuName = "Tracker Pro/Settings File")]
    public class Settings : ScriptableObject
    {
        public string secretAchievementTitle;
        public string secretAchievementDescription;
        public string secretRewardString;
        public AchievementElementUI achievementElementUI;
        public StatElementUI statElementUI;
        public bool autosave = true;
        public Sprite defaultSpriteForAchievements;
    }
}
