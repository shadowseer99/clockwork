#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gear:CollidingObject {
	public bool isGolden=false;
	private float goldenRotation=0;
	private float soundCounter=0;
	
	public AudioCustom _insert;
	public AudioCustom _goldenRotating;
	public AudioCustom _goldenRotated;
	private AudioSource insert;
	private AudioSource goldenRotating;
	private AudioSource goldenRotated;

	public override void Start() {
		base.Start();
		if (!Application.isPlaying) return;

		// initialize sounds
		insert = gameObject.AddComponent<AudioSource>();
		insert.clip = _insert.clip;
		insert.loop = false;
		goldenRotating = gameObject.AddComponent<AudioSource>();
		goldenRotating.clip = _goldenRotating.clip;
		goldenRotating.loop = false;
		goldenRotated = gameObject.AddComponent<AudioSource>();
		goldenRotated.clip = _goldenRotated.clip;
		goldenRotated.loop = false;
	}

	public override void Update() {
		base.Update();
		if (Application.isPlaying) insert.volume = _insert.volume*SoundProfile.effects;
	}

	public override void PhysicsUpdate() {
		// handle goldenGear
		if (isGolden) {
			if (Application.isPlaying) {
				//if (!goldenRotating.isPlaying) goldenRotating.Play();
				if (Mathf.Floor(Mathf.Abs(goldenRotation)/40) != Mathf.Floor(Mathf.Abs(goldenRotation+Time.fixedDeltaTime*curAngularVelocity)/40))
					goldenRotating.PlayDelayed(0.1f);
				goldenRotating.volume = _goldenRotating.volume*SoundProfile.effects*Mathf.Abs(AngularVelocityToCurSpeed()/maxSpeed);
				goldenRotating.pitch = SemitonesToPitch(Mathf.Abs(goldenRotation)*24/360-12);
				//print("pitch: "+goldenRotating.pitch);
			}
			goldenRotation += Time.fixedDeltaTime*curAngularVelocity;
			if (Mathf.Abs(goldenRotation) > 360) {
				goldenRotated.volume = _goldenRotated.volume*SoundProfile.effects;
				goldenRotated.Play();
				GameObject.FindObjectOfType<Pause>().EndLevel();
				return;
			}
		}

		base.PhysicsUpdate();
	}

	private float SemitonesToPitch(float semitones) {
		return Mathf.Pow(2, semitones/12);
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(Gear))][CanEditMultipleObjects]
public class GearEditor:CollidingObjectEditor {}
#endif