using UnityEngine;
using System.Collections;

public class PlaySoundOnClick : MonoBehaviour {
	public AudioCustom _sound;
	private AudioSource sound;

	public void Start() {
		sound = new GameObject("sound player").AddComponent<AudioSource>();
		sound.clip = _sound.clip;
		sound.volume = _sound.volume;
	}

	public void Play() {
		sound.Play();
	}
}
