#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System;

public class PlayerGear:CollidingObject {
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
		
		base.PhysicsUpdate();
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerGear))]
public class PlayerGearEditor:PhysicsObjectEditor { }
#endif
