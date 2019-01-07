using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour {

    public GameObject healthBar;
    public GameObject boostBar;
    GameObject healthMask;
    GameObject boostMask;
    Health playerHealth;
    PlaneFlight plane;

	// Use this for initialization
	void Start () {
        healthMask = healthBar.GetComponentInChildren<SpriteMask>().gameObject;
        boostMask = boostBar.GetComponentInChildren<SpriteMask>().gameObject;
        playerHealth = GetComponent<Health>();
        plane = GetComponent<PlaneFlight>();
	}
	
	// Update is called once per frame
	void Update () {
        float healthMaskWidth = Mathf.Clamp((float)playerHealth.currentHealth / playerHealth.maxHealth,0,1);
        healthMask.transform.localScale = new Vector2(healthMaskWidth, healthMask.transform.localScale.y);
        float boostMaskWidth = Mathf.Clamp(plane.boost / plane.maxBoost, 0, 1);
        boostMask.transform.localScale = new Vector2(boostMaskWidth, boostMask.transform.localScale.y);
    }
}
