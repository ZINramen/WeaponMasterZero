using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrackerPro
{

    public class TrackerProEvents : MonoBehaviour
    {
        /// <summary>
        /// Action for when achievement progress is updated
        /// </summary>
        public static System.Action<Achievement> AchievementValueChanged;

        /// <summary>
        /// Action for when an achievement was completed and granted
        /// </summary>
        public static System.Action<Achievement> AchievementGranted;


        /// <summary>
        /// Action for when stat value is updated
        /// </summary>
        public static System.Action<Stat> StatValueChanged;


        /// <summary>
        /// Handler called when achievement values are changed
        /// </summary>
        public static void OnAchievementValueChanged(Achievement achievement)
        {
            if (AchievementValueChanged != null)
            {
                AchievementValueChanged.Invoke(achievement);
            }
        }

        /// <summary>
        /// Handler called when achievement has been completed
        /// </summary>
        /// <param name="achievement"></param>
        public static void OnAchievementGranted(Achievement achievement)
        {
            if (AchievementGranted != null)
            {
                AchievementGranted.Invoke(achievement);
            }
        }


        /// <summary>
        /// Handler called when achievement values are changed
        /// </summary>
        public static void OnStatValueChanged(Stat stat)
        {
            if (StatValueChanged != null)
            {
                StatValueChanged.Invoke(stat);
            }
        }
    }
}
