using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LandInTown : MonoBehaviour {

	public string townName;
	public float townHeight;
	Collider2D thisCollider; 
	ContactFilter2D playerFilter;
	PlaneManagement planeMan;

	// Use this for initialization
	void Start () {
		playerFilter = new ContactFilter2D();
		playerFilter.SetLayerMask(LayerMask.GetMask("Player"));
		thisCollider = GetComponent<Collider2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		Collider2D[] playerOverlap = new Collider2D[5];
		thisCollider.OverlapCollider (playerFilter, playerOverlap);
		foreach (Collider2D playerObject in playerOverlap)
		{
			if (playerObject)
			{
				GameObject other = playerObject.gameObject;
				if (other.tag == "Plane" && Mathf.Abs(other.transform.position.z) <= townHeight)
				{
					print (other.transform.position);
					planeMan = FindObjectOfType<PlaneManagement> ();
					planeMan.landingPosition = other.transform.position + 1*Vector3.back;
					planeMan.planeRotation = other.transform.rotation;
					planeMan.map = false;
					SceneManager.LoadSceneAsync (townName);
				}
			}
		}
	}
}
