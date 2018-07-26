using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaneManagement : MonoBehaviour {

	public GameObject plane;
	public float strata;
    public float shift;
    internal Vector3 landingPosition;
	internal Quaternion planeRotation;
	public bool fighting;
	public bool map;
	int scenesLoaded;
    int enemiesFighting;

	void Awake()
	{
		if (FindObjectsOfType<PlaneManagement> ().Length > 1)
		{
			Destroy (gameObject);
		}
	}

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	// Use this for initialization
	void Start () {
        fighting = false;
		DontDestroyOnLoad (gameObject);
		landingPosition = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
        if(enemiesFighting <= 0)
        {
            fighting = false;
        }
        else
        {
            fighting = true;
        }
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (map == true)
		{
			plane = FindObjectOfType<PlaneFlight> ().gameObject;
			landingPosition = (Vector3)landingPosition + Vector3.back*shift;
            plane.transform.rotation = planeRotation;
		}		
	}

	public void RequestFight ()
	{
        enemiesFighting += 1;
        fighting = true;
	}

    public void LeaveFight()
    {
        enemiesFighting -= 1;
    }
}
