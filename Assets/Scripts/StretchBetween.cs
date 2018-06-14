using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchBetween : MonoBehaviour {

    public GameObject object1;
    public GameObject object2;
    Vector2 midpoint;
    Vector2 point1;
    Vector2 point2;

	// Use this for initialization
	void Start () {
        point1 = object1.transform.position;
        point2 = object2.transform.position;
        Vector2 midlength = point1 - point2;
        transform.localScale = new Vector2((midlength).magnitude, transform.localScale.y);
        transform.right = midlength;
        transform.position = (Vector3) (point2 + midlength * .5f) + Vector3.forward*transform.position.z;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        point1 = object1.transform.position;
        point2 = object2.transform.position;
        Vector2 midlength = point1 - point2;
        transform.localScale = new Vector2((midlength).magnitude, transform.localScale.y);
        transform.right = midlength;
        transform.position = (Vector3)(point2 + midlength * .5f) + Vector3.forward * transform.position.z;
    }
}
