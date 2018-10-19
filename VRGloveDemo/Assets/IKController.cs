using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour {


    Animator animator;

    [SerializeField]
    Transform indexFinger;

	void Start () {
        animator = GetComponent<Animator>();	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnAnimatorIK(int layerIndex)
    {
        //animator.SetIKPosition()
    }
}
