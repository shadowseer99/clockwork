using UnityEngine;
using System.Collections;

public class MusicObject : MonoBehaviour {

	// Use this for initialization
	void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
