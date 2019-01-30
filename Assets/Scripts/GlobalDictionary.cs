using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDictionary : MonoBehaviour {

    public static GlobalDictionary instance = null;
    internal List<Quest> Quests;
    internal List<Item> Items;

    [SerializeField]
    public StringBooleanDictionary BooleanDictionary;

    public StringIntDictionary IntDictionary;

    //Awake is always called before any Start functions
    void Awake()
    {
        Quests = new List<Quest>();
        Items = new List<Item>();

        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    private void Reset()
    {
        IntDictionary = new StringIntDictionary() { };
        BooleanDictionary = new StringBooleanDictionary { };

    }
}
