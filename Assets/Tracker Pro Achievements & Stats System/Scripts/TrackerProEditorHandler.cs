using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace TrackerPro
{
    [System.Serializable]
    public class TrackerProEditorHandler : MonoBehaviour
    {
        public List<Achievement> LoadAchievementFile(bool demo = false)
        {
            string loadedJSON = "";
            // If no Achievement.json file, load the demo achievement data.
            if (File.Exists(Application.dataPath + "/TrackerPro/Resources/JSON/Achievements.json"))
            {
                loadedJSON = File.ReadAllText(Application.dataPath + "/TrackerPro/Resources/JSON/Achievements.json");
            }
            else
            {
                loadedJSON = File.ReadAllText(Application.dataPath + "/TrackerPro/Resources/JSON/Demo-Achievements.json");

            }
            EditorAchievementList achievements = JsonUtility.FromJson<EditorAchievementList>(loadedJSON);
            // After loading the data, save it to make sure icon instanceIDs are fine
            // If no Achievements.json exists, this will create one using the demo data
            SaveAchievementsFromEditor(achievements.AchievementList);
            return new List<Achievement>(achievements.AchievementList);
        }

        /// <summary>
        /// Save JSON modifications to the Resources folder. This should only be done in the editor.
        /// </summary>
        public void SaveAchievementsFromEditor(List<Achievement> achievementList)
        {
            // Grab the absolute path to the JSON data in Resources
            string path = Application.dataPath + "/TrackerPro/Resources/JSON/";

            // Write out the serialized data to the Achievements.json file, overwriting it if it exists (it's okay, we have backup!)
            //File.WriteAllText(path + "Achievements.json", Newtonsoft.Json.JsonConvert.SerializeObject(achievementList, Newtonsoft.Json.Formatting.Indented));

            // Take the data in our achievement list and convert it into
            // Human readable JSON. Remove the formatting to optimize the file.
            EditorAchievementList achievementCollection = new EditorAchievementList();
            achievementCollection.AchievementList = achievementList;
            string jsonString = JsonUtility.ToJson(achievementCollection, true);
            File.WriteAllText(path + "/Achievements.json", jsonString);

            // Make a back-up of the existing JSON data (you know, just in case...)
            if (File.Exists(path + "/Achievements.json"))
            {
                File.Copy(path + "/Achievements.json", path + "Achievements-Backup.json", true);
            }
        }

        public List<Stat> LoadStatsFile(bool demo = false)
        {
            string loadedJSON = "";
            // If no Achievement.json file, load the demo achievement data.
            if (File.Exists(Application.dataPath + "/TrackerPro/Resources/JSON/Stats.json"))
            {
                loadedJSON = File.ReadAllText(Application.dataPath + "/TrackerPro/Resources/JSON/Stats.json");
            }
            else
            {
                loadedJSON = File.ReadAllText(Application.dataPath + "/TrackerPro/Resources/JSON/Demo-Stats.json");

            }
            EditorStatList stats = JsonUtility.FromJson<EditorStatList>(loadedJSON);
            // After loading the data, save it to make sure icon instanceIDs are fine
            // If no Achievements.json exists, this will create one using the demo data
            SaveStatsFromEditor(stats.StatList);
            return new List<Stat>(stats.StatList);
        }

        /// <summary>
        /// Save JSON modifications to the Resources folder. This should only be done in the editor.
        /// </summary>
        public void SaveStatsFromEditor(List<Stat> statList)
        {
            // Grab the absolute path to the JSON data in Resources
            string path = Application.dataPath + "/TrackerPro/Resources/JSON/";

            // Write out the serialized data to the Achievements.json file, overwriting it if it exists (it's okay, we have backup!)
            //File.WriteAllText(path + "Achievements.json", Newtonsoft.Json.JsonConvert.SerializeObject(achievementList, Newtonsoft.Json.Formatting.Indented));

            // Take the data in our achievement list and convert it into
            // Human readable JSON. Remove the formatting to optimize the file.
            EditorStatList statCollection = new EditorStatList();
            statCollection.StatList = statList;
            string jsonString = JsonUtility.ToJson(statCollection, true);
            File.WriteAllText(path + "/Stats.json", jsonString);

            // Make a back-up of the existing JSON data (you know, just in case...)
            if (File.Exists(path + "/Stats.json"))
            {
                File.Copy(path + "/Stats.json", path + "Stats-Backup.json", true);
            }
        }
    }

    [System.Serializable]
    public class EditorAchievementList
    {
        public List<Achievement> AchievementList;
    }

    [System.Serializable]
    public class EditorStatList
    {
        public List<Stat> StatList;
    }
}
