using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{

    internal string title;
    internal Sprite image;
    internal string description;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override bool Equals(object o)
    {
        if (o is Item)
            return Equals((Item)o);
        else
            return base.Equals(o);
    }

    public bool Equals(Item i)
    {
        if (title == i.title)
            return true;
        else
            return false;
    }
    public static bool operator ==(Item i1, Item i2)
    {
        if (i1.title == i2.title)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool operator !=(Item i1, Item i2)
    {
        if (i1.title == i2.title)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    // this implementation is not necessary
    // Only the override is
    public override int GetHashCode()
    {
        int hash = 17;
        // Suitable nullity checks etc, of course :)
        hash = hash * 23 + title.GetHashCode();
        hash = hash * 23 + title.GetHashCode();
        return hash;
    }
}
