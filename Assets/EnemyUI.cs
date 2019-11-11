using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    public Slider healthBar;
    public Vector3 offset;
    Health enemyHealth;
    

    // Use this for initialization
    void Start()
    {
        enemyHealth = GetComponentInParent<Health>();
        healthBar.transform.SetParent(GameObject.Find("World Canvas").transform, true);
        healthBar.transform.position = transform.position + offset;
        healthBar.transform.rotation = Quaternion.identity;
        healthBar.maxValue = enemyHealth.maxHealth;
        healthBar.value = enemyHealth.currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyHealth.currentHealth != 0 && enemyHealth.currentHealth != enemyHealth.maxHealth)
        {
            healthBar.gameObject.SetActive(true);
            healthBar.maxValue = enemyHealth.maxHealth;
            healthBar.value = enemyHealth.currentHealth;
            healthBar.transform.position = transform.position + offset;
        }
        else
        {
            healthBar.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        //print("Destroyed!");
        if(healthBar)
        {
            Destroy(healthBar.gameObject);
            //print("Healthbar destroyed");
        }
    }
}
