using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    public Slider healthBar;
    public Slider boostBar;
    Health playerHealth;
    PlaneFlight plane;

	// Use this for initialization
	void Start () {
        playerHealth = GetComponent<Health>();
        plane = GetComponent<PlaneFlight>();
        healthBar.maxValue = playerHealth.maxHealth;
        healthBar.value = playerHealth.currentHealth;
        boostBar.maxValue = plane.maxBoost;
        boostBar.value = plane.boost;
	}
	
	// Update is called once per frame
	void Update () {
        healthBar.maxValue = playerHealth.maxHealth;
        healthBar.value = playerHealth.currentHealth;
        boostBar.maxValue = plane.maxBoost;
        boostBar.value = plane.boost;
    }
}
