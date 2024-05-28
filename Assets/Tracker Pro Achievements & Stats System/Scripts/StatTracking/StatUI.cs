using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TrackerPro
{
    public class StatUI : MonoBehaviour
    {
        public Transform parentListObject;
        private List<StatElementUI> statPrefabList = new List<StatElementUI>();

        // Start is called before the first frame update
        void Start()
        {

            BuildStatUIList();
            TrackerProEvents.StatValueChanged += RefreshUIValues;
        }

        public void BuildStatUIList()
        {
            #region oldCode
            //for (int i = 0; i < StatMaster.statMaster.Count; i++)
            //{
            //    if (StatMaster.statMaster[i].showStatInUI == false)
            //    {
            //        //Instantiate the UI Element
            //        statPrefabList.Add(Instantiate(StatMaster.instance.TrackerProSettings.statElementUI));

            //        //Set it's parent to the parentListObject
            //        statPrefabList[i].transform.SetParent(parentListObject, false);
            //    }

            //}
            #endregion

            for (int i = 0; i < StatMaster.statMaster.Count; i++)
            {
        
                    // Instantiate the UI Element
                var newStatPrefab = Instantiate(StatMaster.instance.TrackerProSettings.statElementUI);

                    // Set its parent to the parentListObject
                newStatPrefab.transform.SetParent(parentListObject, false);

                    // Add the newStatPrefab to the statPrefabList
                statPrefabList.Add(newStatPrefab);
                
                //Means its a hidden stat
                if (StatMaster.statMaster[i].showStatInUI == true)
                {
                    //Then we set the gameobject to false.
                    newStatPrefab.gameObject.SetActive(false);
                }
            }

            //Now we populate the data

            for (int i = 0; i < statPrefabList.Count; i++)
            {
                statPrefabList[i].SetStatValues(StatMaster.statMaster[i]);
            }
        }

        public void RefreshUIValues(Stat stat)
        {
            for (int i = 0; i < StatMaster.statMaster.Count; i++)
            {
                if (StatMaster.statMaster[i].id == stat.id)
                {
                    statPrefabList[i].SetStatValues(stat);
                }
            }
        }
    }
}


