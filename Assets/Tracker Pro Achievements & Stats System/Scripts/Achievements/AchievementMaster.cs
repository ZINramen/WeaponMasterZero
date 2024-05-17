using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace TrackerPro
{
    public class AchievementMaster : MonoBehaviour
    {
        public Settings TrackerProSettings;

        public static AchievementMaster instance;
        public static List<Achievement> achievementMaster = new List<Achievement>();

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
        }

        private void OnEnable()
        {
            achievementMaster = LoadAchievementFile();
            //Debug.Log("Test");
        }

        /// <summary>
        /// Returns the Achievement based on the int id
        /// </summary>
        /// <param name="id">ID of the Achievement.</param>
        public static Achievement GetAchievement(int id)
        {
            foreach(Achievement achievement in achievementMaster)
            {
                if(achievement.id == id)
                {
                    return achievement;
                }
            }

            Debug.LogWarning("Achievement: " + id.ToString() + "doesn't exist! Check the Tracker Pro Manager");
            return null;
        }

        /// <summary>
        /// Returns the Achievement based on the string name
        /// </summary>
        /// <param name="nameOfAchievement">ID of the Achievement.</param>
        public static Achievement GetAchievement(string nameOfAchievement)
        {
            foreach (Achievement achievement in achievementMaster)
            {
                if (achievement.title == nameOfAchievement)
                {
                    return achievement;
                }
            }

            Debug.LogWarning("Achievement: " + nameOfAchievement + "doesn't exist! Check the Tracker Pro Manager");
            return null;
        }

        /// <summary>
        /// Increment the achievement progress value based on an int amount
        /// </summary>
        /// <param name="id">ID of the Achievement.</param>
        /// <param name="amount">Amount to increment by.</param>
        public static void AddValueToAchievement(int id, int amount)
        {
            
            Achievement achievement = GetAchievement(id);
            if (achievement.typeOfAchievement != Achievement.TypeOfAchievement.Numerical) return;

            if(achievement == null)
            {
                Debug.LogWarning("Achievement: {0} could not be found! Check your ID's in the Tracker Pro Manager");
            }

            
            if (achievement.value < achievement.neededValue)
            {
                achievement.value += amount;
                CheckForCompletion(achievement);
                TrackerProEvents.OnAchievementValueChanged(achievement);
                    //Autosave
                if (instance.TrackerProSettings.autosave)
                {
                    SaveAchievementFile(AchievementMaster.achievementMaster);
                }
            }
            

            
        }

        /// <summary>
        /// Increment the achievement progress value based on an int amount
        /// </summary>
        /// <param name="achievementName">Name of the Achievement.</param>
        /// <param name="amount">Amount to increment by.</param>
        public static void AddValueToAchievement(string achievementName, int amount)
        {
            Achievement achievement = GetAchievement(achievementName);
            if(achievement.typeOfAchievement != Achievement.TypeOfAchievement.Numerical) return;
            if (achievement == null)
            {
                Debug.LogWarning("Achievement: {0} could not be found! Check your ID's in the Achievement Manager!");
            }

            if (achievement.value < achievement.neededValue)
            {
                achievement.value += amount;
                CheckForCompletion(achievement);
                TrackerProEvents.OnAchievementValueChanged(achievement);
                //Autosave
                if (instance.TrackerProSettings.autosave)
                {
                    SaveAchievementFile(AchievementMaster.achievementMaster);
                }
            }
        }

        //Adds One - Easily Accessible via Events.
        public static void AddValueToAchievement(string achievementName)
        {
            Achievement achievement = GetAchievement(achievementName);
            if (achievement.typeOfAchievement != Achievement.TypeOfAchievement.Numerical) return;
            if (achievement == null)
            {
                Debug.LogWarning("Achievement: {0} could not be found! Check your ID's in the Achievement Manager!");
            }

            if (achievement.value < achievement.neededValue)
            {
                achievement.value += 1;
                CheckForCompletion(achievement);
                TrackerProEvents.OnAchievementValueChanged(achievement);
                //Autosave
                if (instance.TrackerProSettings.autosave)
                {
                    SaveAchievementFile(AchievementMaster.achievementMaster);
                }
            }
        }

        public static void UnlockSecretAchievement(string nameOfAchievement)
        {
            Achievement achievement = GetAchievement(nameOfAchievement);

            achievement.secret = false;
            if (instance.TrackerProSettings.autosave)
            {
                SaveAchievementFile(AchievementMaster.achievementMaster);

            }
            CheckForCompletion(achievement);
            TrackerProEvents.OnAchievementValueChanged(achievement);
        }

        public static void UnlockSecretAchievement(int id)
        {
            Achievement achievement = GetAchievement(id);

            achievement.secret = false;
            if (instance.TrackerProSettings.autosave)
            {
                SaveAchievementFile(AchievementMaster.achievementMaster);

            }
            CheckForCompletion(achievement);
            TrackerProEvents.OnAchievementValueChanged(achievement);
        }

        #region Handlers
        public static void CheckForCompletion(Achievement achievement)
        {
            if(!achievement.completed && achievement.value >= achievement.neededValue)
            {
                achievement.completed = true;
                GiveAchievement(achievement);
            }
            else
            {
                achievement.completed = false;
            }
        }

        public static void GiveAchievement(Achievement achievement)
        {
            achievement.completed = true;
            achievement.value = achievement.neededValue;

            achievement.completedDateTime = achievement.SetCompletedDateTime(System.DateTime.Now);
            TrackerProEvents.OnAchievementValueChanged(achievement);
            TrackerProEvents.OnAchievementGranted(achievement);
            //Autosave
            if (instance.TrackerProSettings.autosave)
            {
                SaveAchievementFile(AchievementMaster.achievementMaster);
            }
        }

        public static void GiveAchievement(int id)
        {
            Achievement achievement = GetAchievement(id);
            achievement.completed = true;
            achievement.value = achievement.neededValue;
            achievement.completedDateTime = achievement.SetCompletedDateTime(System.DateTime.Now);
            TrackerProEvents.OnAchievementValueChanged(achievement);
            TrackerProEvents.OnAchievementGranted(achievement);
            //Autosave
            if (instance.TrackerProSettings.autosave)
            {
                SaveAchievementFile(AchievementMaster.achievementMaster);
            }
        }
        public static void GiveAchievement(string nameOfAchievement)
        {
            Achievement achievement = GetAchievement(nameOfAchievement);
            achievement.completed = true;
            achievement.value = achievement.neededValue;
            achievement.completedDateTime = achievement.SetCompletedDateTime(System.DateTime.Now);
            TrackerProEvents.OnAchievementValueChanged(achievement);
            TrackerProEvents.OnAchievementGranted(achievement);
            //Autosave
            if (instance.TrackerProSettings.autosave)
            {
                SaveAchievementFile(AchievementMaster.achievementMaster);
            }
        }


        public static void RemoveAchievement(Achievement achievement, bool resetTotalValue = false)
        {
            achievement.completed = false;
            achievement.completedDateTime = string.Empty;
            if(resetTotalValue)
            {
                achievement.value = 0;
            }
            TrackerProEvents.OnAchievementValueChanged(achievement);
            //Autosave
            if (instance.TrackerProSettings.autosave)
            {
                SaveAchievementFile(AchievementMaster.achievementMaster);
            }
        }
        #endregion

        #region SaveLoadAchievements
        public static List<Achievement> LoadAchievementFile(bool demo = false)
        {
            string loadedJSON = "";
            // If no Achievement.json file, load the demo achievement data.
            if (File.Exists(Application.dataPath + "/TrackerPro/Resources/JSON/Achievements.json"))
            {
                loadedJSON = File.ReadAllText(Application.dataPath + "/TrackerPro/Resources/JSON/Achievements.json");
            }
            else if (File.Exists(Application.dataPath + "/TrackerPro/Resources/JSON/DemoFolder/Demo-Achievements.json"))
            {
                loadedJSON = File.ReadAllText(Application.dataPath + "/TrackerPro/Resources/JSON/DemoFolder/Demo-Achievements.json");

            }
            RuntimeAchievementList achievements = JsonUtility.FromJson<RuntimeAchievementList>(loadedJSON);
            // After loading the data, save it to make sure icon instanceIDs are fine
            // If no Achievements.json exists, this will create one using the demo data
            //SaveAchievementFile(achievements.AchievementList);
            return new List<Achievement>(achievements.AchievementList);
        }

        public static void SaveAchievementFile(List<Achievement> achievementList)
        {
            // Grab the absolute path to the JSON data in Resources
            string path = Application.dataPath + "/TrackerPro/Resources/JSON";

            // Write out the serialized data to the Achievements.json file, overwriting it if it exists (it's okay, we have backup!)
            //File.WriteAllText(path + "Achievements.json", Newtonsoft.Json.JsonConvert.SerializeObject(achievementList, Newtonsoft.Json.Formatting.Indented));

            // Take the data in our achievement list and convert it into
            // Human readable JSON. Remove the formatting to optimize the file.
            RuntimeAchievementList achievementCollection = new RuntimeAchievementList();
            achievementCollection.AchievementList = achievementList;
            string jsonString = JsonUtility.ToJson(achievementCollection, true);
            File.WriteAllText(path + "/Achievements.json", jsonString);

            // Make a back-up of the existing JSON data (you know, just in case...)
            if (File.Exists(path + "/Achievements.json"))
            {
                File.Copy(path + "/Achievements.json", path + "Achievements-Backup.json", true);
            }
        }
        #endregion
    }

    [System.Serializable]
    public class RuntimeAchievementList
    {
        public List<Achievement> AchievementList;
    }

}


