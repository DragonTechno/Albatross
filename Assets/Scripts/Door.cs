using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable {

    public GameObject partner;

    public override void InteractionAction()
    {
        player.transform.position = partner.transform.GetChild(0).transform.position;
        Lock.Unlock();
    }
}
