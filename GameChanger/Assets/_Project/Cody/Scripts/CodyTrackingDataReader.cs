using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodyTrackingDataReader : MonoBehaviour
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
            Quat quat = new Quat(quaternionW[_currentIndex], quaternionI[_currentIndex], quaternionJ[_currentIndex], quaternionK[_currentIndex]);
            Quat qx = new Quat (Mathf.Cos (0.01f / 2), 0, 0, Mathf.Sin (0.01f / 2));
            Quat qy = new Quat (Mathf.Cos (0.01f / 2), 0, Mathf.Sin (0.01f / 2), 0);
            Quat qz = new Quat (Mathf.Cos (0.01f / 2), Mathf.Sin (0.01f / 2), 0, 0);
            
            quat = Quat.Multiply (qx, quat);
            quat = Quat.Multiply (qy, quat);
            quat = Quat.Multiply (qz, quat);

            _currentRotation = quat.ToUnityQuaternion();

                
            _currentPosition = new Vector3(
                positionX[_currentIndex], 
                positionY[_currentIndex], 
                positionZ[_currentIndex]);
            /*_currentRotation = new Quaternion(
                quaternionI[_currentIndex], 
                quaternionJ[_currentIndex],
                quaternionK[_currentIndex], 
                quaternionW[_currentIndex]);*/
            
            thisTransform.localPosition = _currentPosition;
            transform.localRotation = _currentRotation;
            
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
    
    public class Quat{
        // Represents w + xi + yj + zk
        public float w, x, y, z;
        public Quat(float w, float x, float y, float z){
            this.w = w;
            this.x = x;
            this.y = y;
            this.z = z;
        }
 
        public float Norm(){
            return Mathf.Sqrt (w * w + x * x + y * y + z * z);
        }
 
        public Quat Normalize(){
            float m = Norm ();
            return new Quat (w / m, x / m, y / m, z / m);
        }
 
        // Returns a*b
        public static Quat Multiply(Quat a, Quat b){
            float w = a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z;
            float x = a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y;
            float y = a.w * b.y + a.y * b.w - a.x * b.z + a.z * b.x;
            float z = a.w * b.z + a.z * b.w + a.x * b.y - a.y * b.x;
            return new Quat (w,x,y,z).Normalize();
        }
 
        public Quaternion ToUnityQuaternion(){
            return new Quaternion (w, x, y, z);
        }
    }
}
