using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

namespace TrackerPro
{
    public class TrackerProEditor : EditorWindow
    {
        
        private Vector2 scrollStart;

        //Achievement Tracking Variables
        private TrackerProEditorHandler editorHandler;
        private List<bool> showAchievementDetails = new List<bool>();
        private List<Achievement> tempAchievementList = new List<Achievement>();

        //Stat Tracking Variables
        private List<bool> showStatDetails = new List<bool>();
        private List<Stat> tempStatList = new List<Stat>();

        private enum TabType
        { 
            AchievementManager,
            StatManager
        }

        //private enum AchievementTypeOptions
        //{
        //Numerical,
        //Boolean
        
        //}

        //private AchievementTypeOptions achievementType;

        private TabType currentTab = TabType.AchievementManager;

        [MenuItem("Tools/Cruxis Creations/Tracker Pro/Tracker Pro Manager")]
        static void InitializeEditor()
        {
            TrackerProEditor editor1 = (TrackerProEditor)EditorWindow.GetWindow(typeof(TrackerProEditor));
            editor1.maxSize = new Vector2(800, 900);
            editor1.minSize = new Vector2(625, 800);
            editor1.maximized = true;
            editor1.titleContent = new GUIContent("Tracker Pro Manager");
            editor1.Show();
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Achievement Manager"))
            {
                currentTab = TabType.AchievementManager;
            }

            if(GUILayout.Button("Stat Manager"))
            {
                currentTab = TabType.StatManager;
            }
            GUILayout.EndHorizontal();

            switch(currentTab)
            {
                case TabType.AchievementManager:
                    DrawAchievementManager();
                    break;
                case TabType.StatManager:
                    DrawStatManager();
                    break;
            
            
            }
        }

        private void DrawAchievementManager()
        {
            EditorGUILayout.LabelField("Achievement Tracking Tools", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Undo", "Undo unsaved changes"), GUILayout.Width(Screen.width / 4 - 4.5f), GUILayout.Height(35)))
            {
                editorHandler = new TrackerProEditorHandler();
                tempAchievementList = new List<Achievement>(editorHandler.LoadAchievementFile());
            }
            if (GUILayout.Button(new GUIContent("0 all values", "Reset all achievement values to 0"), GUILayout.Width(Screen.width / 4 - 4.5f), GUILayout.Height(35)))
            {
                ResetAllStats();
            }
            if (GUILayout.Button(new GUIContent("Generate IDs", "Automatically generate IDs for ALL achievements, starting from 0"), GUILayout.Width(Screen.width / 4 - 4.5f), GUILayout.Height(35)))
            {
                GenerateAchievementIDs();
            }
            if (GUILayout.Button(new GUIContent("Close all", "Close all open achievement editors"), GUILayout.Width(Screen.width / 4 - 4.5f), GUILayout.Height(35)))
            {
                for (int i = 0; i < showAchievementDetails.Count; i++)
                {
                    showAchievementDetails[i] = false;
                }
            }
            
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            DrawAchievementsArea();
            DrawAchievementControlArea();
        }


        void DrawAchievementsArea()
        {
            EditorGUILayout.LabelField('\u2630' + " Achievements", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            scrollStart = EditorGUILayout.BeginScrollView(scrollStart);
            for (int i = 0; i < tempAchievementList.Count; i++)
            {
                Achievement achievement = tempAchievementList[i];
                
                // Grab the icon based on the saved asset path (so we can generate an instanceID for the editor)
                //achievement.icon = (Sprite)AssetDatabase.LoadAssetAtPath(achievement.iconPath, typeof(Sprite));
                showAchievementDetails.Add(false);
                showAchievementDetails[i] = EditorGUILayout.Foldout(showAchievementDetails[i], achievement.title);
                if (showAchievementDetails[i])
                {

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(new GUIContent("Icon", "Icon used to represent the achievement in the UI"));
                    if (achievement.iconPath != "")
                        achievement.icon = (Sprite)EditorGUILayout.ObjectField("", AssetDatabase.LoadAssetAtPath<Sprite>(achievement.iconPath), typeof(Sprite), true);
                    else
                        achievement.icon = (Sprite)EditorGUILayout.ObjectField("", achievement.icon, typeof(Sprite), true);
                    achievement.iconPath = AssetDatabase.GetAssetPath(achievement.icon);
                    EditorGUILayout.EndHorizontal();   

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(new GUIContent("ID", "Unique identifier used to reference specific achievements (must be unique!)"));
                    achievement.id = EditorGUILayout.IntField(achievement.id);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(new GUIContent("Name", "Achievement's title displayed in the UI"));
                    achievement.title = EditorGUILayout.TextField(achievement.title);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(new GUIContent("Description", "Summary text displayed below the achievevement title"));
                    achievement.description = EditorGUILayout.TextArea(achievement.description);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(new GUIContent("Type of Achievement", "The Type of Achievement"));
                    achievement.typeOfAchievement = (Achievement.TypeOfAchievement)EditorGUILayout.EnumPopup(achievement.typeOfAchievement);
                    EditorGUILayout.EndHorizontal();

                    if(achievement.typeOfAchievement == Achievement.TypeOfAchievement.Numerical)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(new GUIContent("Needed Value", "The needed incremental amount to complete achievement"));
                        achievement.neededValue = EditorGUILayout.IntField(achievement.neededValue);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(new GUIContent("Current Value", "The amount of progress currently made"));
                        achievement.value = EditorGUILayout.IntSlider(achievement.value, 0, achievement.neededValue);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(new GUIContent("Display as Percentage", "If checked, the achievement values will display as {value}%"));
                        achievement.displayAsPercentage = EditorGUILayout.Toggle(achievement.displayAsPercentage);
                        EditorGUILayout.EndHorizontal();
                    }
                    
                    if(achievement.typeOfAchievement == Achievement.TypeOfAchievement.Boolean)
                    {
                        EditorGUILayout.HelpBox("If using Boolean type of Achievement Completion, remember to give the achievement manually yourself using AchievementMaster.GiveAchievement(string NameOfAchievement)", MessageType.Warning);
                    }
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(new GUIContent("Reward Value", "The amount of achievement points to grant when completed"));
                    achievement.points = EditorGUILayout.IntField(achievement.points);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(new GUIContent("Completed?", "Whether or not the achievement is completed"));
                    achievement.completed = EditorGUILayout.Toggle(achievement.completed);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(new GUIContent("Secret Achievement?", "Secretive achievement details are hidden in the UI until completed"));
                    achievement.secret = EditorGUILayout.Toggle(achievement.secret);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(new GUIContent("^", "Move up list"), GUILayout.Width(30)) && i > 0)
                    {
                        int desiredIndex = i - 1;
                        var item = achievement;
                        tempAchievementList.RemoveAt(i);
                        tempAchievementList.Insert(desiredIndex, item);
                        showAchievementDetails[desiredIndex] = true;
                        showAchievementDetails[i] = false;
                    }
                    else if (GUILayout.Button(new GUIContent("v", "Move down list"), GUILayout.Width(30)) && i < tempAchievementList.Count - 1)
                    {
                        int desiredIndex = i + 1;
                        var item = achievement;
                        tempAchievementList.RemoveAt(i);
                        tempAchievementList.Insert(desiredIndex, item);
                        showAchievementDetails[desiredIndex] = true;
                        showAchievementDetails[i] = false;
                    }
                    GUI.color = new Color32(255, 145, 165, 255);
                    if (GUILayout.Button(new GUIContent("-", "Remove achievement"), GUILayout.Width(20)))
                    {
                        tempAchievementList.RemoveAt(i);
                        showAchievementDetails.RemoveAt(i);
                        ++i;

                    }
                    GUI.color = Color.white;
                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndVertical();

                }
            }
            EditorGUILayout.EndScrollView();
            GUI.color = new Color32(229, 243, 255, 255);
            EditorGUILayout.EndVertical();
        }

        void DrawAchievementControlArea()
        {

            EditorGUILayout.BeginHorizontal();
            GUI.color = new Color32(196, 218, 255, 255);
            if (GUILayout.Button(new GUIContent("+ Achievement", "Create a new achievement"), GUILayout.Height(40)))
            {
                tempAchievementList.Add(new Achievement());
                //showAchievementDetails.Add(false);
            }
            GUI.color = new Color32(196, 255, 203, 255);
            if (GUILayout.Button(new GUIContent("Save Changes", "Save achievement data to file"), GUILayout.Height(40)))
            {
                editorHandler.SaveAchievementsFromEditor(tempAchievementList);
                AchievementMaster.achievementMaster = tempAchievementList;
                AssetDatabase.Refresh();
            }
            EditorGUILayout.EndHorizontal();


        }

        #region AchievementTools
        void GenerateAchievementIDs()
        {
            for (int i = 0; i < tempAchievementList.Count; i++)
            {
                tempAchievementList[i].id = i;
            }
        }

        void ResetAllAchievementStats()
        {
            // Set value to 0 and completed to false for each achievement
            tempAchievementList.ForEach(x =>
            {
                x.value = 0;
                x.completed = false;
            });
        }

        #endregion

        #region StatTools
        void GenerateStatIDs()
        {
            for (int i = 0; i < tempStatList.Count; i++)
            {
                tempStatList[i].id = i;
            }
        }

        void ResetAllStats()
        {
            // Set value to 0 and track stat for each stat
            tempStatList.ForEach(x =>
            {
                x.value = 0;
                x.trackStat = true;
            });
        }

        #endregion

        private void DrawStatManager()
        {
            EditorGUILayout.LabelField("Stat Tracker Tools", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Undo", "Undo unsaved changes"), GUILayout.Width(Screen.width / 4 - 4.5f), GUILayout.Height(35)))
            {
                editorHandler = new TrackerProEditorHandler();
                tempStatList = new List<Stat>(editorHandler.LoadStatsFile());
            }
            if (GUILayout.Button(new GUIContent("0 all values", "Reset all stat values to 0"), GUILayout.Width(Screen.width / 4 - 4.5f), GUILayout.Height(35)))
            {
                ResetAllStats();
            }
            if (GUILayout.Button(new GUIContent("Generate IDs", "Automatically generate IDs for ALL stats, starting from 0"), GUILayout.Width(Screen.width / 4 - 4.5f), GUILayout.Height(35)))
            {
                GenerateStatIDs();
            }
            if (GUILayout.Button(new GUIContent("Close all", "Close all open stat drop downs"), GUILayout.Width(Screen.width / 4 - 4.5f), GUILayout.Height(35)))
            {
                for (int i = 0; i < showStatDetails.Count; i++)
                {
                    showStatDetails[i] = false;
                }
            }
            
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            DrawStatsArea();
            DrawStatsControlArea();
        }

        private void DrawStatsArea()
        {
            EditorGUILayout.LabelField('\u2630' + " Stats", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            scrollStart = EditorGUILayout.BeginScrollView(scrollStart);
            for (int i = 0; i < tempStatList.Count; i++)
            {
                Stat stat = tempStatList[i];
                // Grab the icon based on the saved asset path (so we can generate an instanceID for the editor)
                //achievement.icon = (Sprite)AssetDatabase.LoadAssetAtPath(achievement.iconPath, typeof(Sprite));
                showStatDetails.Add(false);
                showStatDetails[i] = EditorGUILayout.Foldout(showStatDetails[i], stat.name);
                if (showStatDetails[i])
                {

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(new GUIContent("ID", "Unique identifier used to reference specific stats (must be unique!)"));
                    stat.id = EditorGUILayout.IntField(stat.id);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(new GUIContent("Name", "Name of the Stat"));
                    stat.name = EditorGUILayout.TextField(stat.name);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(new GUIContent("Description", "Summary text for the Stat"));
                    stat.description = EditorGUILayout.TextArea(stat.description);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(new GUIContent("Should we track the stat?", "Set whether the stat is trackable or not - can be used to pause tracking!"));
                    stat.trackStat = EditorGUILayout.Toggle(stat.trackStat);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(new GUIContent("Type of Stat", "The Type of Stat"));
                    stat.typeOfStat = (Stat.TypeOfStat)EditorGUILayout.EnumPopup(stat.typeOfStat);
                    EditorGUILayout.EndHorizontal();

                    if(stat.typeOfStat == Stat.TypeOfStat.Numerical)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(new GUIContent("Current Value", "The Current Value of the Stat"));
                        stat.value = EditorGUILayout.FloatField(stat.value);
                        EditorGUILayout.EndHorizontal();
                    }

                    if(stat.typeOfStat == Stat.TypeOfStat.Percentage)
                    {
                        EditorGUILayout.HelpBox("If using Percentage Type of Stat, The higher value needs to be in Linked Stat 1. E.g. If you want to find the percentage of number of bullets hit vs number of bullets fired, then the number fired would be linked stat 1!", MessageType.Warning);
                        
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(new GUIContent("Linked Stat 1", "Name of the First Linked Stat Used for Percentage Stats"));
                        stat.linkedStat1 = EditorGUILayout.TextField(stat.linkedStat1);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(new GUIContent("Linked Stat 2", "Name of the Second Linked Stat Used for Percentage Stats"));
                        stat.linkedStat2 = EditorGUILayout.TextField(stat.linkedStat2);
                        EditorGUILayout.EndHorizontal();
                    }
                    
                    if(stat.typeOfStat == Stat.TypeOfStat.TimeSpan)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(new GUIContent("Type of TimeSpan", "The Type of Timespan to save e.g. Seconds, Minutes, Hours, Days or All"));
                        stat.howToDisplayTimeStat = (Stat.HowToDisplayTimeStat)EditorGUILayout.EnumPopup(stat.howToDisplayTimeStat);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(new GUIContent("Timespan value in Seconds", "The Timespan value in Seconds - will be converted to chosen type for UI"));
                        EditorGUILayout.LabelField(stat.timeSpan);
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(new GUIContent("Hidden Stat", "Set whether the stat is hidden or not - used typically for percentage stats"));
                    stat.showStatInUI = EditorGUILayout.Toggle(stat.showStatInUI);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(new GUIContent("^", "Move up list"), GUILayout.Width(30)) && i > 0)
                    {
                        int desiredIndex = i - 1;
                        var item = stat;
                        tempStatList.RemoveAt(i);
                        tempStatList.Insert(desiredIndex, item);
                        showStatDetails[desiredIndex] = true;
                        showStatDetails[i] = false;
                    }
                    else if (GUILayout.Button(new GUIContent("v", "Move down list"), GUILayout.Width(30)) && i < tempStatList.Count - 1)
                    {
                        int desiredIndex = i + 1;
                        var item = stat;
                        tempStatList.RemoveAt(i);
                        tempStatList.Insert(desiredIndex, item);
                        showStatDetails[desiredIndex] = true;
                        showStatDetails[i] = false;
                    }
                    GUI.color = new Color32(255, 145, 165, 255);
                    if (GUILayout.Button(new GUIContent("-", "Remove achievement"), GUILayout.Width(20)))
                    {
                        tempStatList.RemoveAt(i);
                        showStatDetails.RemoveAt(i);
                        ++i;

                    }
                    GUI.color = Color.white;
                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndVertical();

                }
            }
            EditorGUILayout.EndScrollView();
            GUI.color = new Color32(229, 243, 255, 255);
            EditorGUILayout.EndVertical();
        }

        private void DrawStatsControlArea()
        {
            EditorGUILayout.BeginHorizontal();
            GUI.color = new Color32(196, 218, 255, 255);
            if (GUILayout.Button(new GUIContent("+ New Stat", "Create a new stat to track"), GUILayout.Height(40)))
            {
                tempStatList.Add(new Stat());       
            }
            GUI.color = new Color32(196, 255, 203, 255);
            if (GUILayout.Button(new GUIContent("Save Changes", "Save stat data to file"), GUILayout.Height(40)))
            {
                editorHandler.SaveStatsFromEditor(tempStatList);
                StatMaster.statMaster = tempStatList;
                AssetDatabase.Refresh();
            }
            EditorGUILayout.EndHorizontal();
        }

        void Awake()
        {
            //Load Stat and Achievement files and add them to the master in-game lists
            editorHandler = new TrackerProEditorHandler();
            tempAchievementList = editorHandler.LoadAchievementFile();
            tempStatList = editorHandler.LoadStatsFile();
            AchievementMaster.achievementMaster = tempAchievementList;
            StatMaster.statMaster = tempStatList;
        }
    }
}

