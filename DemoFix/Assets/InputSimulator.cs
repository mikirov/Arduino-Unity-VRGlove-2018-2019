using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSimulator : BaseInputController {

    [SerializeField]
    [Range(0, 1024)]
    private List<int> trimmerInputs;

    [SerializeField]
    private List<FingerJoint> fingerJoints;

    [SerializeField]
    private Quaternion rotation;

    [SerializeField]
    [Range(0, 512)]
    private int minBound = 0;

    [SerializeField]
    [Range(512, 1024)]
    private int maxBound = 1024;

    private void Start()
    {
        int fingerJointCount = FindObjectOfType<HandController>().GetFingerJointCount();
        if (fingerJoints.Count < fingerJointCount)
        {
            Debug.LogError(fingerJointCount + " finger joints needed!");
            Destroy(gameObject);
        }
    }


    public override List<int> GetValues()
    {
        return trimmerInputs;
    }


    public override Quaternion GetMPUValues()
    {
        return rotation;
    }
}
