using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    public int maxHealth;
    public int damageSourcesPerFrame;
    public float invincibleTime;
    internal int frameDamage = 0;
    internal int currentHealth;
    internal List<GameObject> damageSources;
    public bool invincible = false;
    int mainLayer;

    // Use this for initialization
    void Start()
    {
        mainLayer = gameObject.layer;
        currentHealth = maxHealth;
        invincible = false;
        damageSources = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void takeDamage(int damage, GameObject damageSource)
    {
        if (!invincible && damageSources.Count < damageSourcesPerFrame && !damageSources.Contains(damageSource))
        {
            damageSources.Add(damageSource);
            frameDamage += damage;
        }
    }

    void LateUpdate()
    {
        if(damageSources.Count > 0)
        {
            print(damageSources.Count);
            currentHealth -= frameDamage;
            frameDamage = 0;
            damageSources = new List<GameObject>();
            StartCoroutine(Invincible(invincibleTime));
        }
    }

    public void StartInvincibility(float duration)
    {
        StartCoroutine(Invincible(duration));
    }

    public IEnumerator Invincible(float duration)
    {
        gameObject.layer = 11;
        print("Invincible");
        invincible = true;
        yield return new WaitForSecondsRealtime(duration);
        gameObject.layer = mainLayer;
        invincible = false;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
