using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

public class CsvReader : MonoBehaviour
{
    public string filename;
    public List<List<string>> lines = new List<List<string>>();
    public Transform medalContent;
    private void Start()
    {
        StreamReader sr = new StreamReader(Application.dataPath + "/Hyun/Data/" + filename);
        bool eof = false;
        
        while (!eof) // 끝 도달까지 계속 진행
        {
            string data = sr.ReadLine();
            if(data == null)
            {
                eof = true;
                break;
            }
            var values = data.Split(',');
            List<string> valueList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                valueList.Add(values[i].ToString());
            }
            lines.Add(valueList);
        }

        if (medalContent)
        {
            int i = 1;
            foreach (var item in medalContent.GetComponentsInChildren<Achievement>())
            {
                item.medal_Code = i;
                if (item.save.medalList.Find(x => x == item.medal_Code) == 0)
                {
                    item.gameObject.SetActive(false);
                }
                i++;
            }
        }
    }
}
