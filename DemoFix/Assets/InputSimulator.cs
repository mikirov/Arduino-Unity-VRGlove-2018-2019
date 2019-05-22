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
    [Range(0, 512)]
    private int minBound = 0;

    [SerializeField]
    [Range(512, 1024)]
    private int maxBound = 1024;

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
        /*
        foreach(FingerJoint joint in fingerJoints)
        {
            joint.SetBounds(minBound, maxBound);
        }
        */
    }


    public override List<int> GetValues()
    {
        return trimmerInputs;
    }


    public override Quaternion GetMPUValues()
    {
        throw new System.NotImplementedException();
    }
}
