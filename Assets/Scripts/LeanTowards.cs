using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeanTowards : MonoBehaviour {

    public float leanDistance;
    public float leanRange;
    public GameObject target;
    public float turnSpeed;
    public float leanSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (target && (target.transform.position - transform.parent.transform.position).magnitude < leanRange)
        {
            transform.position = Vector3.MoveTowards(transform.position,transform.position + (Vector3)((Vector2)target.transform.position - (Vector2)transform.parent.position).normalized * leanDistance,leanSpeed);
            Quaternion rotateTowards = Quaternion.AngleAxis(Mathf.Clamp(Vector2.SignedAngle(transform.right,target.transform.position-transform.position),-turnSpeed,turnSpeed), Vector3.forward);
            transform.right = rotateTowards * transform.right;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position,transform.parent.position,leanSpeed);
        }
	}
}
