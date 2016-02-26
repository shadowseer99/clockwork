#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System;

public class PlayerGear:CollidingObject {
	public AudioCustom _stickToGear;
	public AudioCustom _stickToGearEmpty;
	public AudioCustom _letGoOfGear;
	public AudioCustom _letGoOfGearEmpty;
	private AudioSource stickToGear;
	private AudioSource stickToGearEmpty;
	private AudioSource letGoOfGear;
	private AudioSource letGoOfGearEmpty;
	private bool wasAttaching=false;
	private CollidingObject wasAttached;
	public Animator attachingSystem;
	public Animator attachedSystem;
	private Renderer attachingSystemRenderer;

	public override void Start() {
		base.Start();
		attachingSystemRenderer = attachingSystem.GetComponent<Renderer>();
		
		if (Application.isPlaying) {
			// initialize AudioSources
			stickToGear = gameObject.AddComponent<AudioSource>();
			stickToGearEmpty = gameObject.AddComponent<AudioSource>();
			letGoOfGear = gameObject.AddComponent<AudioSource>();
			letGoOfGearEmpty = gameObject.AddComponent<AudioSource>();

			stickToGear.clip = _stickToGear.clip;
			stickToGearEmpty.clip = _stickToGearEmpty.clip;
			letGoOfGear.clip = _letGoOfGear.clip;
			letGoOfGearEmpty.clip = _letGoOfGearEmpty.clip;

			stickToGear.volume = _stickToGear.volume;
			stickToGearEmpty.volume = _stickToGearEmpty.volume;
			letGoOfGear.volume = _letGoOfGear.volume;
			letGoOfGearEmpty.volume = _letGoOfGearEmpty.volume;

			stickToGear.loop = false;
			stickToGearEmpty.loop = false;
			letGoOfGear.loop = false;
			letGoOfGearEmpty.loop = false;
		}
	}

	public override void PhysicsUpdate() {
        // handle attaching
#if UNITY_IPHONE || UNITY_ANDROID
        attaching = Mobile.engage;
        accelMult = 0;
        if (Mobile.left)
            accelMult -= 1;
        if (Mobile.right)
            accelMult += 1;
#else
        attaching = Input.GetKey(KeyCode.LeftShift);
        accelMult = CrossPlatformInputManager.GetAxisRaw("Horizontal");
#endif



        trig.enabled = attaching;
		if (attaching==false && attachedTo!=null) {
			// do this to detach but remember neighbors
			CollidingObject temp = attachedTo;
			OnTriggerExit2D(temp.trig);
			OnTriggerEnter2D(temp.trig);
		}
		if (attaching && attachedTo==null && !wasAttaching) attachingSystem.SetBool("RunForwards", true);
		attachingSystemRenderer.enabled = !attachingSystem.GetCurrentAnimatorStateInfo(0).IsName("Off");
		if (attaching && attachedTo!=null && !wasAttaching) attachedSystem.SetBool("RunForwards", true);
		if (!attaching && wasAttaching) attachedSystem.SetBool("RunBackwards", true);

		// handle sounds
		if (!wasAttached && attaching && attachedTo!=null && !stickToGear.isPlaying) stickToGear.Play();
		if (!wasAttaching && attaching && attachedTo==null && !stickToGearEmpty.isPlaying) stickToGearEmpty.Play();
		if (!attaching && wasAttaching && wasAttached==null && !letGoOfGear.isPlaying) letGoOfGear.Play();
		if (!attaching && wasAttaching && wasAttached!=null && !letGoOfGearEmpty.isPlaying) letGoOfGearEmpty.Play();
		wasAttached = attachedTo;
		wasAttaching = attaching;

		base.PhysicsUpdate();
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerGear))][CanEditMultipleObjects]
public class PlayerGearEditor:PhysicsObjectEditor { }
#endif
