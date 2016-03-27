using UnityEngine;
using System.Collections;

public class persistant : MonoBehaviour {
    static bool exists;
    void Awake()
    {
        if (exists)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(this.gameObject);
        exists = true;
    }
}
