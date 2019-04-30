using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSimulator : MonoBehaviour {

    [SerializeField]
    [Range(0, 1024)]
    private List<int> trimmerInputs;

    [SerializeField]
    private List<FingerJoint> fingerJoints;

    private void Awake()
    {
        if (fingerJoints.Count != 14)
        {
            Debug.LogError("14 finger joints needed!");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach(FingerJoint joint in fingerJoints)
        {
            joint.SetBounds(512, 800);
        }
    }

    private void Update()
    {
        
        for(int i = 0; i < 14; i++)
        {
            FingerJoint joint = fingerJoints[i];
            joint.SetRotation(trimmerInputs[i]);
        }
    }
}
