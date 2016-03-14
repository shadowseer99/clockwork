using UnityEngine;
using System.Collections;

public class MusicObject : MonoBehaviour {
	static bool exists;
    public AudioSource defaultmusic;
    public AudioSource momentummusic;
    public AudioSource watermusic;
    public AudioSource buttonmusic;
    public AudioSource xtrememusic;
    public int momstart;
    public int momend;
    public int waterstart;
    public int waterend;
    public int buttstart;
    public int buttend;
    public int xtrstart;
    public int xtrend;
    private int playing=0;
    
    void Start()
    {
        
    }
    void OnLevelWasLoaded(int level)
    {
        if (level <= momend && level >= momstart && playing != 1)
        {
            defaultmusic.Stop();
            momentummusic.Play();
            watermusic.Stop();
            buttonmusic.Stop();
            xtrememusic.Stop();
            playing = 1;
        }
        else if (level <= waterend && level >= waterstart && playing != 2)
        {
            defaultmusic.Stop();
            momentummusic.Stop();
            watermusic.Play();
            buttonmusic.Stop();
            xtrememusic.Stop();
            playing = 2;
        }
        else if (level <= buttend && level >= buttstart && playing != 3)
        {
            defaultmusic.Stop();
            momentummusic.Stop();
            watermusic.Stop();
            buttonmusic.Play();
            xtrememusic.Stop();
            playing = 3;
        }
        else if (level <= xtrend && level >= xtrstart && playing != 4)
        {
            defaultmusic.Stop();
            momentummusic.Stop();
            watermusic.Stop();
            buttonmusic.Stop();
            xtrememusic.Play();
            playing = 4;
        }
        else if (level == 0)
        {
            defaultmusic.Play();
            momentummusic.Stop();
            watermusic.Stop();
            buttonmusic.Stop();
            xtrememusic.Stop();
            playing = 0;
        }
    }
    void Update()
    {
        
        defaultmusic.volume = SoundProfile.music;
        momentummusic.volume = SoundProfile.music;
        watermusic.volume = SoundProfile.music;
        buttonmusic.volume = SoundProfile.music;
        xtrememusic.volume = SoundProfile.music;

        AudioListener.volume = SoundProfile.master;
        transform.position = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
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
