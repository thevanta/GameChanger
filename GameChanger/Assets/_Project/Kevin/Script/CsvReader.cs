using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CsvReader : MonoBehaviour
{
    [SerializeField] private string filePath = @"D:\Box\UnityProjects\GameChanger\Data\8_24_21\example_data_scripts\thighR_FE_1.csv";
    [SerializeField] private List<string> headers;
    [SerializeField] private List<float> positionX;
    [SerializeField] private List<float> positionY;
    [SerializeField] private List<float> positionZ;

    private const int POSITION_X = 6;
    private const int POSITION_Y = 7;
    private const int POSITION_Z = 8;
    

    private void OnEnable()
    {
        Read();
    }

    [ContextMenu("Read")]
    private void Read()
    {
        StreamReader reader = null;

        if (File.Exists(filePath))
        {
            reader = new StreamReader(File.OpenRead(filePath));
            List<string> workingList = new List<string>();
            headers = new List<string>();

            var line = reader.ReadLine();
            var values = line.Split(',');
            foreach (var item in values)
            {
                headers.Add(item);
            }
            
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                values = line.Split(',');

                for (var i = 0; i < values.Length; i++)
                {
                    switch (i)
                    {
                        case POSITION_X: positionX.Add(float.Parse(values[i]));
                            break;
                        case POSITION_Y: positionY.Add(float.Parse(values[i]));
                            break;
                        case POSITION_Z: positionZ.Add(float.Parse(values[i]));
                            break;
                        default:
                            break;
                    }
                }
                
                foreach (var value in workingList)
                {
                    //print(value);
                }
            }
        }
    }
}
