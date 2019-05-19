using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerJoint : MonoBehaviour
{
    [SerializeField]
    private Vector3 minRotation;

    [SerializeField]
    private Vector3 maxRotation;

    private int minTrimmerValue;
    private int maxTrimmerValue;

    private void Awake()
    {
        minTrimmerValue = PlayerPrefs.GetInt(name + "-min", 0);
        maxTrimmerValue = PlayerPrefs.GetInt(name + "-max", 1024);
    }

    public void SetBounds(int min, int max)
    {
        minTrimmerValue = min;
        maxTrimmerValue = max;
        
        PlayerPrefs.SetInt(name + "-min", min);
        PlayerPrefs.SetInt(name + "-max", max);
    }

    public void SetRotation(float trimmerValue)
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        float lerpVal = 0;
        if (trimmerValue >= minTrimmerValue)
        {
            lerpVal = (trimmerValue - minTrimmerValue) / (maxTrimmerValue - minTrimmerValue);
        }

        rotation = Vector3.Lerp(
            minRotation,
            maxRotation,
            lerpVal
        );

        transform.localEulerAngles = rotation;
    }
}
