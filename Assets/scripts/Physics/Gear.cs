#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gear:CollidingObject {
	public bool isGolden=false;
	private float goldenRotation=0;
	
	public AudioCustom _insert;
	public AudioCustom _goldenRotating;
	private AudioSource insert;
	private AudioSource goldenRotating;

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
	}

	public override void Update() {
		base.Update();
		if (Application.isPlaying) insert.volume = _insert.volume*SoundProfile.effects;
	}

	public override void PhysicsUpdate() {
		// handle goldenGear
		if (isGolden) {
			if (Application.isPlaying) {
				if (!goldenRotating.isPlaying) goldenRotating.Play();
				goldenRotating.volume = _goldenRotating.volume*SoundProfile.effects*Mathf.Abs(AngularVelocityToCurSpeed()/maxSpeed);
				goldenRotating.pitch = 1+Mathf.Abs(goldenRotation)/360;
			}
			goldenRotation += Time.fixedDeltaTime*curAngularVelocity;
			if (Mathf.Abs(goldenRotation) > 360) {
				GameObject.FindObjectOfType<Pause>().EndLevel();
				return;
			}
		}

		base.PhysicsUpdate();
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(Gear))][CanEditMultipleObjects]
public class GearEditor:CollidingObjectEditor {}
#endif