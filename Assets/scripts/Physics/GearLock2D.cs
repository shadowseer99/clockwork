using UnityEngine;
using System.Collections;
using System;

public class GearLock2D : MonoBehaviour {
	private bool isActive=false;
	private Vector3 startPos;
	private Gear gear;
	public float minDistanceToTrigger=1;
	public float transitionTime=1;
	private float curTime=0;
	public bool moveX=true;
	public bool moveY=true;
	public bool moveZ=false;

	public void FixedUpdate () {
		if (isActive && curTime<transitionTime) {
			curTime += Time.fixedDeltaTime;
			gear.transform.position = Vector3.Lerp(startPos, transform.position, curTime/transitionTime);
			if (!moveX) gear.transform.position = new Vector3(startPos.x, gear.transform.position.y, gear.transform.position.z);
			if (!moveY) gear.transform.position = new Vector3(gear.transform.position.x, startPos.y, gear.transform.position.z);
			if (!moveZ) gear.transform.position = new Vector3(gear.transform.position.x, gear.transform.position.y, startPos.z);
		}
	}

	public void OnTriggerStay2D(Collider2D coll) {
		Gear gear = coll.GetComponent<Gear>();
		if (gear!=null && gear.isMovable && !isActive && (Vector3.Distance(transform.position, gear.transform.position)<minDistanceToTrigger || minDistanceToTrigger==0)) {
			print("moving now");
			gear.isMovable = false;
			gear.Start();
			this.gear = gear;
			startPos = gear.transform.position;
			isActive = true;
		}
	}
}
