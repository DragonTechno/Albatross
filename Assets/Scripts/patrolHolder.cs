using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class patrolHolder : MonoBehaviour {

    internal Transform[] patrol;

    private void OnEnable()
    {
        Transform[] tempPatrol = GetComponentsInChildren<Transform>();
        patrol = new Transform[tempPatrol.Length-1];
        Array.Copy(tempPatrol, 1, patrol, 0, patrol.Length);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
