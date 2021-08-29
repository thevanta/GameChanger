using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingDataReader : MonoBehaviour
{
    [SerializeField] private Transform thisTransform;
    [SerializeField] private CsvReader csvReader;
    [SerializeField] private List<float> positionX;
    [SerializeField] private List<float> positionY;
    [SerializeField] private List<float> positionZ;
    [SerializeField] private List<float> quaternionW;
    [SerializeField] private List<float> quaternionI;
    [SerializeField] private List<float> quaternionJ;
    [SerializeField] private List<float> quaternionK;

    private int _currentIndex;
    private Vector3 _currentPosition;
    private Quaternion _currentRotation;
    
    //Future implementation
    private enum Tracker
    {
        LeftThigh,
        RightThigh,
    }

    private void FixedUpdate()
    {
        if (_currentIndex < positionX.Count)
        {
            _currentPosition = new Vector3(
                positionX[_currentIndex], 
                positionY[_currentIndex], 
                positionZ[_currentIndex]);
            _currentRotation = new Quaternion(
                quaternionI[_currentIndex], 
                quaternionJ[_currentIndex],
                quaternionK[_currentIndex], 
                quaternionW[_currentIndex]);
            
            thisTransform.localPosition = _currentPosition;
            //Rotations are currently incorrect
            //transform.localRotation = _currentRotation;
            
            _currentIndex++;
        }
    }

    [ContextMenu("PopulateDependencies")]
    private void PopulateDependencies()
    {
        thisTransform = transform;
        csvReader = FindObjectOfType<CsvReader>();
        GetTrackingData();
    }

    [ContextMenu("GetTrackingData")]
    private void GetTrackingData()
    {
        positionX = new List<float>();
        positionY = new List<float>();
        positionZ = new List<float>();
        quaternionW = new List<float>();
        quaternionI = new List<float>();
        quaternionJ = new List<float>();
        quaternionK = new List<float>();
        
        positionX = csvReader.PositionX;
        positionY = csvReader.PositionY;
        positionZ = csvReader.PositionZ;

        quaternionI = csvReader.QuaternionI;
        quaternionJ = csvReader.QuaternionJ;
        quaternionK = csvReader.QuaternionK;
        quaternionW = csvReader.QuaternionW;
        
        _currentIndex = 0;
    }
}
