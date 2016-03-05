using UnityEngine;
using System.Collections;

public class MusicObject : MonoBehaviour {
	static bool exists;
    private AudioSource music;
    private GameObject cam;
    private AudioListener ear;
    void Start()
    {
        music = gameObject.GetComponent<AudioSource>();
    }
    void Update()
    {
        music.volume = SoundProfile.music;
        AudioListener.volume = SoundProfile.master;
    }
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
