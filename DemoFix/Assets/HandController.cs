using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.Serialization;

public class HandController : MonoBehaviour
{
    [SerializeField]
    private List<FingerJoint> fingerJoints;
    
    private Quaternion mpuCalibration;

    private void Awake()
    {
        float x = PlayerPrefs.GetFloat("mpu-x", 0);
        float y = PlayerPrefs.GetFloat("mpu-y", 0);
        float z = PlayerPrefs.GetFloat("mpu-z", 0);
        float w = PlayerPrefs.GetFloat("mpu-w", 0);

        mpuCalibration = new Quaternion(x, y, z, w);
    }

    private void Update()
    {
        SetReadings(FindObjectOfType<BaseInputController>().GetValues());
        Quaternion mpuRotation = FindObjectOfType<BaseInputController>().GetMPUValues();

        transform.rotation = mpuRotation * Quaternion.Inverse(mpuCalibration);
    }

    public int GetFingerJointCount()
    {
        return fingerJoints.Count;
    }

    public bool FingerHasPositiveRotationAt(int index)
    {
        return GetFingerAt(index).HasPositiveRotation();
    }


    public FingerJoint GetFingerAt(int index)
    {
        return fingerJoints[index];
    }


    public void SetCalibrationRotation(Quaternion calibrationQuat)
    {
        mpuCalibration = calibrationQuat;

        PlayerPrefs.SetFloat("mpu-x", calibrationQuat.x);
        PlayerPrefs.SetFloat("mpu-y", calibrationQuat.y);
        PlayerPrefs.SetFloat("mpu-z", calibrationQuat.z);
        PlayerPrefs.SetFloat("mpu-w", calibrationQuat.w);
    }


    public void SetBounds(int[] lowerValues, int[] highValues)
    {
        for(int i = 0; i < fingerJoints.Count; i++)
        {
            fingerJoints[i].SetBounds(lowerValues[i], highValues[i]);
        }
    }


    private void SetReadings(int[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            fingerJoints[i].SetRotation(values[i]);
        }
    }


    private void SetReadings(List<int> values)
    {
        SetReadings(values.ToArray());
    }
}