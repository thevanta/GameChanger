using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CsvReader : MonoBehaviour
{
    [SerializeField] private string filePath = @"D:\Box\UnityProjects\GameChanger\Data\8_24_21\example_data_scripts\thighR_FE_1.csv";
    [SerializeField] private List<string> headers;
    
    [SerializeField] public List<float> PositionX;
    [SerializeField] public List<float> PositionY;
    [SerializeField] public List<float> PositionZ;
    [SerializeField] public List<float> QuaternionW;
    [SerializeField] public List<float> QuaternionI;
    [SerializeField] public List<float> QuaternionJ;
    [SerializeField] public List<float> QuaternionK;

    public const int POSITION_X = 6;
    public const int POSITION_Y = 7;
    public const int POSITION_Z = 8;
    public const int QUATERNION_W = 9;
    public const int QUATERNION_I = 10;
    public const int QUATERNION_J = 11;
    public const int QUATERNION_K = 12;
    

    [ContextMenu("Read")]
    private void Read()
    {
        StreamReader reader = null;

        if (File.Exists(filePath))
        {
            reader = new StreamReader(File.OpenRead(filePath));
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
                        case POSITION_X: PositionX.Add(float.Parse(values[i]));
                            break;
                        case POSITION_Y: PositionY.Add(float.Parse(values[i]));
                            break;
                        case POSITION_Z: PositionZ.Add(float.Parse(values[i]));
                            break;
                        case QUATERNION_W: QuaternionW.Add(float.Parse(values[i]));
                            break;
                        case QUATERNION_I: QuaternionI.Add(float.Parse(values[i]));
                            break;
                        case QUATERNION_J: QuaternionJ.Add(float.Parse(values[i]));
                            break;
                        case QUATERNION_K: QuaternionK.Add(float.Parse(values[i]));
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
