using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : MonoBehaviour
{
    public GameObject[] eyes;
    public float fireRateIncrease;
    public float turnRateIncrease;

    int deadEyes;

    // Start is called before the first frame update
    void Start()
    {
        deadEyes = 0;
        foreach (GameObject eye in eyes)
        {
            if (eye)
            {
                eye.transform.right = Quaternion.AngleAxis(Random.Range(0,360), Vector3.forward)*Vector2.up;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        int newDeadEyes = -deadEyes;
        foreach(GameObject eye in eyes)
        {
            if(eye == null)
            {
                ++newDeadEyes;
            }
        }
        if (newDeadEyes > 0)
        {
            foreach (GameObject eye in eyes)
            {
                if (eye)
                {
                    eye.GetComponent<BasicEnemyFire>().fireRate += fireRateIncrease * newDeadEyes;
                    eye.GetComponent<BasicEnemyFire>().delay -= 2*fireRateIncrease * newDeadEyes;
                    eye.GetComponent<LeanTowards>().turnSpeed += turnRateIncrease * newDeadEyes;
                }
            }
        }
        deadEyes += newDeadEyes;
        if(deadEyes == 10)
        {
            GetComponentInChildren<turnFollow>().forceOutOfFight = true;
            GetComponent<Animator>().Play("SpiderDeath");
            Destroy(gameObject, 4.13f);
        }
    }
}
