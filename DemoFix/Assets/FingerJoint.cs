using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerJoint : MonoBehaviour
{
    [SerializeField]
    private Vector3 minRotation;

    [SerializeField]
    private Vector3 maxRotation;

    [SerializeField]
    private bool hasPositiveRotation = true;

    private int minTrimmerValue;
    private int maxTrimmerValue;

    private void Awake()
    {
        minTrimmerValue = PlayerPrefs.GetInt(name + "-min", 0);
        maxTrimmerValue = PlayerPrefs.GetInt(name + "-max", 1024);
    }

    public void SetBounds(int min, int max)
    {
        if(min > max)
        {
            Debug.LogError("Finger joint " + name + " has bounds set out of range!", gameObject);
            return;
        }
        minTrimmerValue = min;
        maxTrimmerValue = max;
        
        PlayerPrefs.SetInt(name + "-min", min);
        PlayerPrefs.SetInt(name + "-max", max);
    }

    public void SetRotation(float trimmerValue)
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        float lerpVal = 0;
        if(trimmerValue >= minTrimmerValue)
        {
            lerpVal = (trimmerValue - minTrimmerValue) / (maxTrimmerValue - minTrimmerValue);
        }


        if(float.IsInfinity(lerpVal) || float.IsNaN(lerpVal))
        {
            Debug.LogError(lerpVal + " for " + name + 
                " with min " + minTrimmerValue + " and max: " + minTrimmerValue + 
                " is infinity!");
            return;
        }

        if (hasPositiveRotation) {
            rotation = Vector3.Lerp(minRotation, maxRotation, lerpVal);
        }
        else
        {
            rotation = Vector3.Lerp(maxRotation, minRotation, lerpVal);
        }

        transform.localEulerAngles = rotation;
    }

    public bool HasPositiveRotation()
    {
        return hasPositiveRotation;
    }
}
