using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : ScriptableObject {

    internal string title;
    internal string description;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override bool Equals(object o)
    {
        if (o is Quest)
            return Equals((Quest)o);
        else
            return base.Equals(o);
    }

    public bool Equals(Quest q)
    {
        if (title == q.title)
            return true;
        else
            return false;
    }
    public static bool operator ==(Quest q1, Quest q2)
    {
        if (q1.title == q2.title)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool operator !=(Quest q1, Quest q2)
    {
        if (q1.title == q2.title)
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
