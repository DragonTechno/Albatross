using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHop : MonoBehaviour {
    public float hopDelay;
    public float hopDuration;
    public bool shootDuring;
    turnFollow tf;
    bool moving;
    bool waiting;
    bool startMoving;
    float originalSpeed;

	// Use this for initialization
	void Start () {
        tf = GetComponent<turnFollow>();
        originalSpeed = tf.speed;
        tf.speed = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if(startMoving)
        {
            StartCoroutine("Hop");
            startMoving = false;
            moving = true;
        }
        else if(!moving && !waiting)
        {
            waiting = true;
            StartCoroutine("Wait");
        }
	}

    IEnumerator Hop()
    {
        float originalRate = 0;
        if(!shootDuring)
        {
            originalRate = GetComponent<BasicEnemyFire>().fireRate;
            GetComponent<BasicEnemyFire>().fireRate = 0;
        }
        tf.speed = originalSpeed;
        yield return new WaitForSeconds(hopDuration);
        tf.speed = 0;
        print(tf.speed);
        moving = false;
        if (!shootDuring)
        {
            GetComponent<BasicEnemyFire>().fireRate = originalRate;
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(hopDelay);
        waiting = false;
        startMoving = true;
    }
}
