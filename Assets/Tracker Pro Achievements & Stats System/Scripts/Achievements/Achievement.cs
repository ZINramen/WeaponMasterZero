using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace TrackerPro
{
    [System.Serializable]
    public class Achievement
    {
        public int id;
        public enum TypeOfAchievement
        {
            Numerical = 1,
            Boolean = 2
        }
       
        public TypeOfAchievement typeOfAchievement;

        public string typeOfAchievementString
        {
            get { return typeOfAchievement.ToString(); }
            set { typeOfAchievement = (TypeOfAchievement)Enum.Parse(typeof(TypeOfAchievement), value); }
        }
        public string title;
        public Sprite icon;
        public string iconPath;
        public string trimmedIconPath;
        public string description;
        public int value;
        public int neededValue;
        public bool displayAsPercentage;
        public int points;
        public bool completed;
        public bool secret;
        public string completedDateTime;
        
        //Constructor for creating new achievements
        public Achievement()
        {
            this.id = AchievementMaster.achievementMaster.Count + 1;
            this.title = "New Achievement";
            this.icon = null;
            this.iconPath = "";
            this.value = 0;
            this.description = "This is the description for your new achievement.";
            this.neededValue = 25;
            this.points = 10;
            this.completed = value >= neededValue;
            this.secret = false;
            this.typeOfAchievement = TypeOfAchievement.Numerical;
            this.completedDateTime = DateTime.Now.ToString();
            this.trimmedIconPath = iconPath;
            
            
        }

        public string SetCompletedDateTime(DateTime dateTime)
        {
            completedDateTime = dateTime.ToString("yyyy-MM-ddTHH:mm:ss"); // ISO 8601 format
            return completedDateTime;
        }

        public DateTime GetCompletedDateTime()
        {
            if (DateTime.TryParse(completedDateTime, out DateTime parsedDateTime))
            {
                return parsedDateTime;
            }
            return default(DateTime); // or handle the error
        }
    }
}
    



