using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision.collider.gameObject.name);
        print(Physics2D.GetIgnoreCollision(collision.collider, GetComponent<Collider2D>()));
    }
}
