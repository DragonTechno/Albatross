using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindChunk : MonoBehaviour
{
    public static List<CullChunk> chunks;
    public static int chunkOffset = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(chunks == null)
        {
            CullChunk[] foundChunks = FindObjectsOfType<CullChunk>();
            chunks = new List<CullChunk>();
            foreach(CullChunk chunk in foundChunks)
            {
                chunks.Add(chunk);
            }
        }
        ParentToNearestChunk();
        InvokeRepeating("ParentToNearestChunk", (float)chunkOffset/10f + 10f, 10f);
        chunkOffset += 1;
        chunkOffset %= 100;
    }

    void ParentToNearestChunk()
    {
        CullChunk nearestChunk = FindObjectOfType<CullChunk>();
        float distance = 9999f;
        foreach(CullChunk chunk in chunks)
        {
            if((chunk.transform.position - transform.position).magnitude < distance)
            {
                distance = (chunk.transform.position - transform.position).magnitude;
                nearestChunk = chunk;
            }
        }
        transform.parent = nearestChunk.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
