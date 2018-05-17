using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaveTown : Interactable {

	public string mapName;
	public float townHeight;

    override public void InteractionAction()
    {
        PlaneManagement planeMan = FindObjectOfType<PlaneManagement>();
        planeMan.map = true;
        print("Map changed");
        print(planeMan.map);
        SceneManager.LoadSceneAsync(mapName);
    }

 /*   void OnTriggerEnter2D(Collider2D coll)
	{
		print ("collision");
		if(coll.gameObject.tag == "Player")
		{
            PlaneManagement planeMan = FindObjectOfType<PlaneManagement>();
            planeMan.map = true;
			print ("Map changed");
            print(planeMan.map);
			SceneManager.LoadSceneAsync (mapName);
		}
	}*/

    void SwitchMap()
    {
        PlaneManagement planeMan = FindObjectOfType<PlaneManagement>();
        planeMan.map = true;
        print("Map changed");
        print(planeMan.map);
        SceneManager.LoadSceneAsync(mapName);
    }
}
