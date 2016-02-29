using UnityEngine;
using System.Collections;

public class MusicObject : MonoBehaviour {
	static bool exists;

	// Use this for initialization
	void Awake()
    {
		if (exists)
			Destroy(gameObject);
		else
			DontDestroyOnLoad(this.gameObject);
		exists = true;
    }
}
