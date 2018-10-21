using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FABRIK algorithm based on:
//https://www.academia.edu/9165835/FABRIK_A_fast_iterative_solver_for_the_Inverse_Kinematics_problem

public class FABRIKSolver : MonoBehaviour {


    [SerializeField]
    List<Transform> jointPositions;


    private List<float> jointDistances = new List<float>();

    [SerializeField]
    Transform endPosition;

    [SerializeField]
    float positionTolerance = 0.25f;

	void Start () {
		if(jointPositions.Count > 0)
        {
            for (int i = 0; i < jointPositions.Count - 1; i++) {
                float distance = Vector3.Distance(jointPositions[i].position, jointPositions[i + 1].position);
                jointDistances.Add(distance);
            }

        }

        else
        {
            Debug.Log("No joint positions given");
        }
	}
	
	
	void Update () {
        Solve();
	}

    private void Solve()
    {
        float distanceFromStartToTarget = Vector3.Distance(jointPositions[0].position, endPosition.position);


        float jointDistanceSum = 0;
        foreach(float distance in jointDistances)
        {
            jointDistanceSum += distance;
        }
        
        if(distanceFromStartToTarget > jointDistanceSum) // if the popsition is out of reach
        {
            for(int i = 0; i < jointPositions.Count - 1; i++)
            {
                Vector3 currentPosition = jointPositions[i].position;
                float distanceFromCurrentToTarget = Math.Abs(Vector3.Distance(endPosition.position, currentPosition));
                float lambda = jointDistances[i] / distanceFromCurrentToTarget;

                jointPositions[i + 1].position = (1 - lambda) * jointPositions[i].position + lambda * endPosition.position;
            }
        }
        else // the poosition is reachable
        {

            float delta = Math.Abs(Vector3.Distance(jointPositions[jointPositions.Count - 1].position, endPosition.position)); // difference between position of end effector and target
            while (delta > positionTolerance)
            {

                //Stage 1
                Vector3 rootInitialPosition = jointPositions[0].position;

                jointPositions[jointPositions.Count - 1] = endPosition;
                for(int i = jointPositions.Count - 2; i >= 0; i--)
                {
                    float distanceToPrevious = Math.Abs(Vector3.Distance(jointPositions[i + 1].position, jointPositions[i].position));
                    float lambda = jointDistances[i] / distanceToPrevious;

                    jointPositions[i].position = (1 - lambda) * jointPositions[i + 1].position + (lambda * jointPositions[i].position);    
                }
                //Stage 2
                jointPositions[0].position = rootInitialPosition;
                for(int i = 0; i < jointPositions.Count - 1; i++)
                {
                    float distanceToNext = Math.Abs(Vector3.Distance(jointPositions[i + 1].position, jointPositions[i].position));
                    float lambda = jointDistances[i] / distanceToNext;

                    jointPositions[i + 1].position = (1 - lambda) * jointPositions[i].position + (lambda * jointPositions[i + 1].position);
                }

                delta = Math.Abs(Vector3.Distance(jointPositions[jointPositions.Count - 1].position, endPosition.position));
            }
        }



    }
}
