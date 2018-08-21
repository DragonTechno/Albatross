using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour {

    public float attraction;
    public float pushback;
    public GameObject target;
    Rigidbody2D rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector2 difference = target.transform.position - transform.position;
        rb.velocity += Mathf.Sqrt(difference.magnitude-pushback)*difference.normalized*attraction;
	}
}
