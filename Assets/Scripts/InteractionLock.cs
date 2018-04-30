﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionLock : MonoBehaviour {

    internal Interactable connectedObject;
    internal bool locked;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnMouseDown()
    {
        if (connectedObject)
        {
            connectedObject.ProgressText();
        }
    }

    public void SetLock(Interactable newFocus)
    {
        locked = true;
        connectedObject = newFocus;
    }

    public void Unlock()
    {
        Invoke("LockDelay",.2f);
        connectedObject = null;
    }

    public void LockDelay()
    {
        locked = false;
    }
}