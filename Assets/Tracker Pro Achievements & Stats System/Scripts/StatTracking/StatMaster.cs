using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace TrackerPro
{
    public class StatMaster : MonoBehaviour
    {
        public Settings TrackerProSettings;

        public static StatMaster instance;
        public static List<Stat> statMaster = new List<Stat>();
        private Dictionary<int, Coroutine> activeCoroutines = new Dictionary<int, Coroutine>();

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
            statMaster = LoadStats();
            ActivateAllStatTimeSpanRoutines();
        }

        public void ActivateAllStatTimeSpanRoutines()
        {
            foreach (Stat stat in statMaster)
            {
                if (stat.typeOfStat == Stat.TypeOfStat.TimeSpan && stat.trackStat == true)
                {
                    if (!activeCoroutines.ContainsKey(stat.id))
                    {
                        Coroutine coroutine = StartCoroutine(TrackStatTimespan(stat.id));
                        activeCoroutines[stat.id] = coroutine;
                    }
                }
            }
        }

        public void EnableSingleStatTimeSpanRoutine(string NameOfStat)
        {
            Stat stat = GetStat(NameOfStat);

            if(stat.typeOfStat == Stat.TypeOfStat.TimeSpan && stat.trackStat == false)
            {
                stat.trackStat = true;
                if(!activeCoroutines.ContainsKey(stat.id))
                {
                    Coroutine coroutine = StartCoroutine(TrackStatTimespan(stat.id));
                    activeCoroutines[stat.id] = coroutine;
                }
            }
        }

        public void EnableSingleStatTimeSpanRoutine(int id)
        {
            Stat stat = GetStat(id);

            if (stat.typeOfStat == Stat.TypeOfStat.TimeSpan && stat.trackStat == false)
            {
                stat.trackStat = true;
                if (!activeCoroutines.ContainsKey(stat.id))
                {
                    Coroutine coroutine = StartCoroutine(TrackStatTimespan(stat.id));
                    activeCoroutines[stat.id] = coroutine;
                }
            }
        }

        public void StopAllStatTimeSpanRoutines()
        {
            foreach (var key in activeCoroutines)
            {
                StopCoroutine(key.Value);
            }
            activeCoroutines.Clear();
        }

        public void StopSingleStatTimeSpanRoutine(int id)
        {
            if (activeCoroutines.TryGetValue(id, out Coroutine coroutine))
            {
                StopCoroutine(coroutine);
                activeCoroutines.Remove(id);
            }
        }
        public void StopSingleStatTimeSpanRoutine(string NameOfStat)
        {
            Stat stat = GetStat(NameOfStat);

            if (activeCoroutines.TryGetValue(stat.id, out Coroutine coroutine))
            {
                StopCoroutine(coroutine);
                activeCoroutines.Remove(stat.id);
            }
        }


        public IEnumerator TrackStatTimespan(int idOfStat)
        {
            Stat stat = GetStat(idOfStat);

            if(stat == null)
            {
                yield break;
            }

            int seconds = 0;
            int autoSaveCounter = 0;

            while(stat.trackStat == true)
            {
                seconds++;
                autoSaveCounter++;

                if (int.TryParse(stat.timeSpan, out int currentTimeSpanInSeconds))
                {
                    stat.timeSpan = (currentTimeSpanInSeconds + 1).ToString();
                }
                else
                {         
                    stat.timeSpan = "1";
                }
                TrackerProEvents.OnStatValueChanged(stat);
                if (autoSaveCounter >= 10)
                {
                    if(TrackerProSettings.autosave)
                    {
                        SaveStats(statMaster);
                    }
                    autoSaveCounter = 0;
                }
                yield return new WaitForSeconds(1f);
            }
        }

        /// <summary>
        /// Returns the Stat based on the int id
        /// </summary>
        /// <param name="id">ID of the Stat.</param>
        public static Stat GetStat(int id)
        {
            foreach (Stat stat in statMaster)
            {
                if (stat.id == id)
                {
                    return stat;
                }
            }

            Debug.LogWarning("Stat: " + id.ToString() + "doesn't exist! Check the Tracker Pro Manager");
            return null;
        }

        /// <summary>
        /// Returns the Stat based on the string name
        /// </summary>
        /// <param name="nameOfStat">ID of the Achievement.</param>
        public static Stat GetStat(string nameOfStat)
        {
            foreach (Stat stat in statMaster)
            {
                if (stat.name == nameOfStat)
                {
                    return stat;
                }
            }

            Debug.LogWarning("Stat: " + nameOfStat + "doesn't exist! Check the Tracker Pro Manager");
            return null;
        }

        public static float ReturnStatCurrentValue(string statName)
        {
            return GetStat(statName).value;
        }

        public static float ReturnStatCurrentValue(int statId)
        {
            return GetStat(statId).value;
        }

        public static void IncrementStat(string statName, float amount)
        {
            Stat stat = GetStat(statName);

            if (stat.typeOfStat != Stat.TypeOfStat.Numerical) return;
            if(stat.trackStat)
            {
                stat.value += amount;
            } 
        }

        public static void IncrementStat(int statId, float amount)
        {
            Stat stat = GetStat(statId);

            if (stat.typeOfStat != Stat.TypeOfStat.Numerical) return;
            if (stat.trackStat)
            {
                stat.value += amount;
            }
        }


        public static void DecreaseStat(string statName, float amount)
        {
            Stat stat = GetStat(statName);

            if (stat.typeOfStat != Stat.TypeOfStat.Numerical) return;
            if (stat.trackStat)
            {
                stat.value -= amount;
            }
        }

        public static void DecreaseStat(int statId, float amount)
        {
            Stat stat = GetStat(statId);

            if (stat.typeOfStat != Stat.TypeOfStat.Numerical) return;
            if (stat.trackStat)
            {
                stat.value -= amount;
            }
        }

        public static void SetStatToValue(string statName, float amount)
        {
            Stat stat = GetStat(statName);

            if (stat.typeOfStat != Stat.TypeOfStat.Numerical) return;
            stat.value = amount;
        }

        public static void SetStatToValue(int statId, float amount)
        {
            Stat stat = GetStat(statId);

            if (stat.typeOfStat != Stat.TypeOfStat.Numerical) return;
            stat.value = amount;
        }

        public static void ResetAllStats()
        {
            foreach(Stat stat in statMaster)
            {
                stat.value = 0;
            }
        }

        #region SaveLoadStats
        public static List<Stat> LoadStats(bool demo = false)
        {
            string loadedJSON = "";
            
            if (File.Exists(Application.dataPath + "/TrackerPro/Resources/JSON/Stats.json"))
            {
                loadedJSON = File.ReadAllText(Application.dataPath + "/TrackerPro/Resources/JSON/Stats.json");
            }
            else if (File.Exists(Application.dataPath + "/TrackerPro/Resources/JSON/DemoFolder/Demo-Stats.json"))
            {
                loadedJSON = File.ReadAllText(Application.dataPath + "/TrackerPro/Resources/JSON/DemoFolder/Demo-Stats.json");

            }
            RuntimeStatList stats = JsonUtility.FromJson<RuntimeStatList>(loadedJSON);

            return new List<Stat>(stats.StatList);
        }

        public static void SaveStats(List<Stat> statList)
        {
            // Grab the absolute path to the JSON data in Resources
            string path = Application.dataPath + "/TrackerPro/Resources/JSON";

            RuntimeStatList statCollection = new RuntimeStatList();
            statCollection.StatList = statList;
            string jsonString = JsonUtility.ToJson(statCollection, true);
            File.WriteAllText(path + "/Stats.json", jsonString);

            // Make a back-up of the existing JSON data (you know, just in case...)
            if (File.Exists(path + "/Stats.json"))
            {
                File.Copy(path + "/Stats.json", path + "Stats-Backup.json", true);
            }
        }
        #endregion

    }

    [System.Serializable]
    public class RuntimeStatList
    {
        public List<Stat> StatList;
    }



    [System.Serializable]
    public class Stat
    {
        public int id;
        public string name;
        public string description;
        public float value;
        public bool trackStat = true;
        public bool showStatInUI = false;
        public string linkedStat1;
        public string linkedStat2;
        public string timeSpan;
        public enum TypeOfStat
        {
            Numerical = 1,
            Percentage = 2, 
            TimeSpan = 4
        }

        public TypeOfStat typeOfStat;

        public enum HowToDisplayTimeStat
        {
        JustSeconds = 1,
        JustMinutes = 2,
        JustHours = 4,
        JustDays = 8,
        All = 16}

        public HowToDisplayTimeStat howToDisplayTimeStat;

        public string typeOfTimeStatString
        {
            get { return howToDisplayTimeStat.ToString(); }
            set { howToDisplayTimeStat = (HowToDisplayTimeStat)Enum.Parse(typeof(HowToDisplayTimeStat), value); }
        }

        public string typeOfStatString
        {
            get { return typeOfStat.ToString(); }
            set { typeOfStat = (TypeOfStat)Enum.Parse(typeof(TypeOfStat), value); }
        }

        public Stat()
        {
            this.id = StatMaster.statMaster.Count + 1;
            this.name = "Name of the Stat";
            this.description = "Description of the Stat";
            this.value = 0;
            this.trackStat = true;
            this.typeOfStat = TypeOfStat.Numerical;
            this.showStatInUI = false;
            this.linkedStat1 = "";
            this.linkedStat2 = "";
            this.timeSpan = "1";
            this.howToDisplayTimeStat = HowToDisplayTimeStat.JustHours;
        }
        public Stat(Stat stat)
        {
            this.id = stat.id;
            this.name = stat.name;
            this.description = stat.description;
            this.value = stat.value;
            this.trackStat = stat.trackStat;
            this.typeOfStat = stat.typeOfStat;
            this.showStatInUI = stat.showStatInUI;
            this.linkedStat1 = stat.linkedStat1;
            this.linkedStat2 = stat.linkedStat2;
            this.timeSpan = stat.timeSpan;
            this.howToDisplayTimeStat = stat.howToDisplayTimeStat;
        }

        public string ConvertTimeSpanSecondsToTimeStringHours(int seconds)
        {
            int hours = 0;

            hours = (int)(seconds / (60 * 60));
            return $"{hours}h";
        }

        public string ConvertTimeSpanSecondsToTimeStringMinutes(int seconds)
        {
            int minutes = 0;
            minutes = (int)(seconds / 60);
            return $"{minutes}m";
        }

        public string ConvertTimeSpanSecondsToTimeStringDays(int seconds)
        {
            int days = 0;
            days = (int)(seconds / (60 * 60 * 24));
            return $"{days}d";

        }

        public string ConvertTimeSpanSecondsToTimeStringDaysHoursMinutesSeconds(int seconds)
        {
            string fullTime = "";

            int days = 0;
            int hours = 0;
            int minutes = 0;
            int newSeconds = 0;

            newSeconds = (int)(seconds % 60);
            minutes = (int)(seconds / 60) % 60;
            hours = (int)(seconds / (60 * 60)) % 24;
            days = (int)(seconds / (60 * 60 * 24));

            fullTime = $"{days}d {hours}d {minutes}m {newSeconds}s";
            return fullTime;
        }
    }
}


