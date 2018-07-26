using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerateToMouse : MonoBehaviour {
    public float speed;
    public float slowShift;
    public float zeroDistance;
    Rigidbody2D rb;
    Camera cam;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
	}

    void FixedUpdate()
    {
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 difference = (Vector2)(mouseWorldPos - transform.position);
        if (difference.magnitude > zeroDistance)
        {
            rb.velocity = (difference.normalized * Mathf.Sqrt(difference.magnitude) * speed);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
}
