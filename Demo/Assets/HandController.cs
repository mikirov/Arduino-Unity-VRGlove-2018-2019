using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.Serialization;

public class HandController : MonoBehaviour
{
//	private bool configured = false;


    struct JointPosition
    {
        public Transform position;
        public Dictionary<int, Quaternion> bentReading;
        public Dictionary<int, Quaternion> normalReading;
    }


    [SerializeField] private List<JointPosition> jointPositions;

    private List<int> readings;

    private void Awake()
    {
        readings = new List<int>();
    }

    public void SetTransforms()
    {
        for (int i = 0; i < jointPositions.Count; i++)
        {
            JointPosition jointPosition = jointPositions[i];

            int bentRotationReading = jointPosition.bentReading.Keys.First();
            int normalRotationReading = jointPosition.normalReading.Keys.First();

            float rotationProportion = Remap(readings[i], bentRotationReading, normalRotationReading, 0, 1);
            jointPosition.position.rotation = Quaternion.Lerp(
                jointPosition.bentReading.Values.First(),
                jointPosition.normalReading.Values.First(),
                rotationProportion);
        }
    }

    public static float Remap(float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        var fromAbs = from - fromMin;
        var fromMaxAbs = fromMax - fromMin;

        var normal = fromAbs / fromMaxAbs;

        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;

        return to;
    }

    public void SetReadings(List<int> values)
    {
        readings = values;
    }
}