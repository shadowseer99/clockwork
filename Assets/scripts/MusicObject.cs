using UnityEngine;
using System.Collections;

public class MusicObject : MonoBehaviour {
	static bool exists;
    public AudioClip defaultmusic;
    public AudioClip watermusic;
    public int waterstart;
    public int waterend;
    private int playing=0;
    private AudioSource music;
    void Start()
    {
        music = gameObject.GetComponent<AudioSource>();
    }
    void Update()
    {
        if(Application.loadedLevel<=waterend&&Application.loadedLevel>=waterstart&&playing!=2)
        {
            music.Stop();
            music.clip = watermusic;
            playing = 2;
            music.Play();
        }
        else if(playing!=0)
        {
            music.Stop();
            music.clip = defaultmusic;
            playing = 0;
            music.Play();
        }
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
