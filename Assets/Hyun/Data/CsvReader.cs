using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using UnityEngine;

public class CsvReader : MonoBehaviour
{
    [Tooltip("csv 파일 경로 Assets 폴더가 시작점")]
    public string filename;
    public List<List<string>> lines = new List<List<string>>();

    private void Start()
    {
        StreamReader sr = new StreamReader(Application.dataPath + filename);
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
    }
}
