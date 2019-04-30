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

    private bool configured = false;

    private void Start()
    {
        foreach(FingerJoint joint in fingerJoints)
        {
            joint.SetBounds(0, 1024);
        }
    }

    public void SetReadings(List<int> values)
    {
        for(int i = 0; i < values.Count; i++)
        {
            fingerJoints[i].SetRotation(values[i]);
        }
    }
}