using UnityEngine;
using System.Collections;

public class PlaySoundOnClick : MonoBehaviour {
	public AudioCustom _sound;
	private AudioSource sound;

	public void Start() {
		sound = new GameObject("sound player").AddComponent<AudioSource>();
		sound.clip = _sound.clip;
		sound.volume = _sound.volume * SoundProfile.effects;
	}

	public void Play() {
        sound.volume = _sound.volume*SoundProfile.effects;
        sound.Play();
	}
}
