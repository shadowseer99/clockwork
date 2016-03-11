using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ButtonSoundPlayer : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
	public AudioCustom _click;
	public AudioCustom _mouseEnter;
	public AudioCustom _mouseExit;
	private static AudioSource click;
	private static AudioSource mouseEnter;
	private static AudioSource mouseExit;

	public void Start() {
		if (click==null) click = new GameObject("Button Click").AddComponent<AudioSource>();
		if (mouseEnter==null) mouseEnter = new GameObject("Button Enter").AddComponent<AudioSource>();
		if (mouseExit==null) mouseExit = new GameObject("Button Exit").AddComponent<AudioSource>();
		if (_click.clip!=null) click.clip = _click.clip;
		if (_mouseEnter.clip!=null) mouseEnter.clip = _mouseEnter.clip;
		if (_mouseExit.clip!=null) mouseExit.clip = _mouseExit.clip;
	}

	public void OnPointerClick(PointerEventData data) {
        click.volume = _click.volume*SoundProfile.effects;
        click.Play();
	}

	public void OnPointerEnter(PointerEventData data) {
		mouseEnter.Play();
		mouseEnter.volume = _mouseEnter.volume*SoundProfile.effects;
	}

	public void OnPointerExit(PointerEventData data) {
		mouseExit.Play();
		mouseExit.volume = _mouseExit.volume*SoundProfile.effects;
	}
}
