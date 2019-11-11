using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseSortingOrder : MonoBehaviour
{
    static int orderOffset = 0;

    // Start is called before the first frame update
    void Start()
    {
        orderOffset += 20;

        foreach(Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.sortingOrder += orderOffset;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
