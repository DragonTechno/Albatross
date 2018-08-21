using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour {

    public GameObject healthBar;
    GameObject healthMask;
    Health playerHealth;

	// Use this for initialization
	void Start () {
        healthMask = healthBar.GetComponentInChildren<SpriteMask>().gameObject;
        playerHealth = GetComponent<Health>();
	}
	
	// Update is called once per frame
	void Update () {
        float maskWidth = Mathf.Clamp((float)playerHealth.currentHealth / playerHealth.maxHealth,0,1);
        healthMask.transform.localScale = new Vector2(maskWidth, healthMask.transform.localScale.y);
	}
}
