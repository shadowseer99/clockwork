#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System;

public class PlayerGear:CollidingObject {
	public AudioClip _stickToGear;
	public AudioClip _stickToGearEmpty;
	public AudioClip _letGoOfGear;
	public AudioClip _letGoOfGearEmpty;
	public AudioCustom test;
	private AudioSource stickToGear;
	private AudioSource stickToGearEmpty;
	private AudioSource letGoOfGear;
	private AudioSource letGoOfGearEmpty;
	bool wasAttaching=false;
	bool wasAttached=false;
	public GameObject attachingSystem;
	public GameObject attachedSystem;
	private float timeSinceChange;

	public override void Start() {
		base.Start();
		
		if (Application.isPlaying) {
			// initialize AudioSources
			stickToGear = gameObject.AddComponent<AudioSource>();
			stickToGearEmpty = gameObject.AddComponent<AudioSource>();
			letGoOfGear = gameObject.AddComponent<AudioSource>();
			letGoOfGearEmpty = gameObject.AddComponent<AudioSource>();

			stickToGear.clip = _stickToGear;
			stickToGearEmpty.clip = _stickToGearEmpty;
			letGoOfGear.clip = _letGoOfGear;
			letGoOfGearEmpty.clip = _letGoOfGearEmpty;

			stickToGear.loop = false;
			stickToGearEmpty.loop = false;
			letGoOfGear.loop = false;
			letGoOfGearEmpty.loop = false;
		}
	}

	public override void PhysicsUpdate() {
		// handle attaching
		attaching = Input.GetKey(KeyCode.LeftShift);
		trig.enabled = attaching;
		if (attaching==false && attachedTo!=null) {
			// do this to detach but remember neighbors
			CollidingObject temp = attachedTo;
			OnTriggerExit2D(temp.trig);
			OnTriggerEnter2D(temp.trig);
		}
		attachingSystem.SetActive(attaching && attachedTo==null && timeSinceChange<0.3f);
		attachedSystem.SetActive(attaching && attachedTo!=null && timeSinceChange<0.3f);
		timeSinceChange = (attaching==wasAttaching && (attachedTo!=null)==wasAttached?timeSinceChange+Time.fixedDeltaTime:0);

		// set direction of accel
		accelMult = CrossPlatformInputManager.GetAxisRaw("Horizontal");

		// handle sounds
		/*source.clip = move;
		source.loop = true;
		if (!source.isPlaying) source.Play();*/
		if (!wasAttached && attaching && attachedTo!=null && !stickToGear.isPlaying) stickToGear.Play();
		if (!wasAttaching && attaching && attachedTo==null && !stickToGearEmpty.isPlaying) stickToGearEmpty.Play();
		if (!attaching && wasAttaching && !wasAttached && !letGoOfGear.isPlaying) letGoOfGear.Play();
		if (!attaching && wasAttaching && wasAttached && !letGoOfGearEmpty.isPlaying) letGoOfGearEmpty.Play();
		wasAttached = attachedTo!=null;
		wasAttaching = attaching;

		base.PhysicsUpdate();
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerGear))][CanEditMultipleObjects]
public class PlayerGearEditor:PhysicsObjectEditor { }
#endif
