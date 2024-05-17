using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TrackerPro
{

    public class StatElementUI : MonoBehaviour
    {
        public TextMeshProUGUI statTitle;
        public TextMeshProUGUI statDescription;
        public TextMeshProUGUI valueText;


        public void SetStatValues(Stat stat)
        {

            statTitle.text = stat.name;
            statDescription.text = stat.description;
            switch (stat.typeOfStat)
            {
                case Stat.TypeOfStat.Numerical:
                    valueText.text = stat.value.ToString();
                    break;
                case Stat.TypeOfStat.Percentage:
                    Stat stat1 = StatMaster.GetStat(stat.linkedStat1);
                    Stat stat2 = StatMaster.GetStat(stat.linkedStat2);
                    valueText.text = string.Format("{0}%", ((float)stat2.value / (float)stat1.value) * 100);
                    break;
                case Stat.TypeOfStat.TimeSpan:
                    int seconds = int.Parse(stat.timeSpan);
                    switch (stat.howToDisplayTimeStat)
                    {
                        case Stat.HowToDisplayTimeStat.JustSeconds:
                            valueText.text = stat.timeSpan + "s";
                            break;
                        case Stat.HowToDisplayTimeStat.JustMinutes:
                            
                            valueText.text = stat.ConvertTimeSpanSecondsToTimeStringMinutes(seconds);
                            break;
                        case Stat.HowToDisplayTimeStat.JustHours:
                            valueText.text = stat.ConvertTimeSpanSecondsToTimeStringHours(seconds);
                            break;
                        case Stat.HowToDisplayTimeStat.JustDays:
                            valueText.text = stat.ConvertTimeSpanSecondsToTimeStringDays(seconds);
                            break;
                        case Stat.HowToDisplayTimeStat.All:
                            valueText.text = stat.ConvertTimeSpanSecondsToTimeStringDaysHoursMinutesSeconds(seconds);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            
        }
                
    }
}
