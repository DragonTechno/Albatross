using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    public int maxHealth;
    internal int currentHealth;

    // Use this for initialization
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void takeDamage(int damage)
    {
        currentHealth -= damage;
        print("Current Health: " + currentHealth.ToString());
        if (gameObject.tag == "Plane")
        {
            PlaneFlight thisPlane = GetComponent<PlaneFlight>();
            StartCoroutine(thisPlane.Invincible(.3f));
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
