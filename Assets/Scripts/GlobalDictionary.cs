using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDictionary : MonoBehaviour {

    [SerializeField]
    public StringBooleanDictionary BooleanDictionary;

    public StringIntDictionary IntDictionary;

    private void Reset()
    {
        IntDictionary = new StringIntDictionary() { };
        BooleanDictionary = new StringBooleanDictionary { };
    }
}
