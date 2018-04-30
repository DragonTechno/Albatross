using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoMovement : MonoBehaviour {

	public float speed;
    float cSpeed;
	Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
        cSpeed = speed;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		int xspeed = 0;
		int yspeed = 0;
		if (Input.GetKey (KeyCode.LeftArrow))
		{
			xspeed -= 1;
		}
		if (Input.GetKey (KeyCode.RightArrow))
		{
			xspeed += 1;
		}
		if (Input.GetKey (KeyCode.UpArrow))
		{
			yspeed += 1;
		}
		if (Input.GetKey (KeyCode.DownArrow))
		{
			yspeed -= 1;
		}
		rb.velocity = Vector2.ClampMagnitude(new Vector2 (xspeed*cSpeed,yspeed*cSpeed),cSpeed);
	}

    public void MovementToggle(bool on)
    {
        if(on)
        {
            cSpeed = speed;
        }
        else
        {
            cSpeed = 0;
        }
    }
}
