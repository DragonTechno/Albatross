using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeanTowards : MonoBehaviour {

    public float leanDistance;
    public float leanRange;
    public GameObject target;
    public float turnSpeed;
    public float leanSpeed;
    public Vector2 offset = new Vector2();

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        Vector2 distance = new Vector2();
        if(target)
        {
            distance = target.transform.position + (Quaternion.AngleAxis(target.transform.rotation.eulerAngles.z, Vector3.forward) * offset) - transform.position;
        }
        if (target && (target.transform.position - transform.parent.transform.position).magnitude < leanRange)
        {
            transform.position = Vector3.MoveTowards(transform.position,transform.position + (Vector3)distance.normalized * leanDistance,leanSpeed);
            Quaternion rotateTowards = Quaternion.AngleAxis(Mathf.Clamp(Vector2.SignedAngle(transform.right,distance),-turnSpeed,turnSpeed), Vector3.forward);
            transform.right = rotateTowards * transform.right;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position,transform.parent.position,leanSpeed);
        }
	}
}
