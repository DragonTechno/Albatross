using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeanTowards : MonoBehaviour {

    public float leanDistance;
    public float leanRange;
    public GameObject target;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if ((target.transform.position - transform.parent.transform.position).magnitude < leanRange)
        {
            transform.position = (Vector2)transform.parent.transform.position + ((Vector2)target.transform.position - (Vector2)transform.parent.transform.position).normalized * leanDistance;
        }
        else
        {
            transform.position = transform.parent.transform.position;
        }
	}
}
