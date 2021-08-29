using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CsvReader : MonoBehaviour
{
    [SerializeField] private string filePath = @"D:\Box\UnityProjects\GameChanger\Data\8_24_21\example_data_scripts\thighR_FE_1.csv";
    [SerializeField] private List<string> headers;
    

    private void OnEnable()
    {
        Read();
    }

    private void Read()
    {
        StreamReader reader = null;

        if (File.Exists(filePath))
        {
            reader = new StreamReader(File.OpenRead(filePath));
            List<string> workingList = new List<string>();
            headers = new List<string>();
            int i = 0;
            while (!reader.EndOfStream && i < 20)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                foreach (var item in values)
                {
                    if (item.Equals(""))
                    {
                        print("Empty!");
                        break;
                    }
                    
                    workingList.Add(item);
                    print(item);
                    i++;
                }
                
                foreach (var coloumn1 in workingList)
                {
                    print(coloumn1);
                }
                i++;
            }
        }
    }
}
