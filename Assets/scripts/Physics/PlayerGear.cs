#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System;

public class PlayerGear:CollidingObject {
	public AudioClip _move;
	public AudioClip _stickToGear;
	public AudioClip _stickToGearEmpty;
	public AudioClip _letGoOfGear;
	public AudioClip _letGoOfGearEmpty;
	public AudioClip _hitSurface;
	private AudioSource move;
	private AudioSource stickToGear;
	private AudioSource stickToGearEmpty;
	private AudioSource letGoOfGear;
	private AudioSource letGoOfGearEmpty;
	bool wasAttaching=false;
	bool wasAttached=false;

	public override void Start() {
		base.Start();
		
		if (Application.isPlaying) {
		print("starting");
			// initialize AudioSources
			move = gameObject.AddComponent<AudioSource>();
			stickToGear = gameObject.AddComponent<AudioSource>();
			stickToGearEmpty = gameObject.AddComponent<AudioSource>();
			letGoOfGear = gameObject.AddComponent<AudioSource>();
			letGoOfGearEmpty = gameObject.AddComponent<AudioSource>();

			move.clip = _move;
			stickToGear.clip = _stickToGear;
			stickToGearEmpty.clip = _stickToGearEmpty;
			letGoOfGear.clip = _letGoOfGear;
			letGoOfGearEmpty.clip = _letGoOfGearEmpty;

			move.loop = true;
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

		// set direction of accel
		accelMult = CrossPlatformInputManager.GetAxisRaw("Horizontal");

		// handle sounds
		/*source.clip = move;
		source.loop = true;
		if (!source.isPlaying) source.Play();*/
		if (!wasAttached && attaching && attachedTo!=null && !stickToGear.isPlaying) stickToGear.Play();
		if (!wasAttaching && attaching && attachedTo==null && !stickToGearEmpty.isPlaying) stickToGearEmpty.Play();
		if (!attaching && wasAttaching && wasAttached!=null && !letGoOfGear.isPlaying) letGoOfGear.Play();
		if (!attaching && wasAttaching && wasAttached==null && !letGoOfGearEmpty.isPlaying) letGoOfGearEmpty.Play();
		if ((accelMult!=0 && groundedTo.Count!=0) && !move.isPlaying) move.Play();
		if ((accelMult==0 && groundedTo.Count==0) && move.isPlaying) move.Stop();
		wasAttached = attachedTo!=null;
		wasAttaching = attaching;
		
		
		base.PhysicsUpdate();
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerGear))]
public class PlayerGearEditor:PhysicsObjectEditor { }
#endif
