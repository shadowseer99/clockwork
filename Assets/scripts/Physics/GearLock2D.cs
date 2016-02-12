using UnityEngine;
using System.Collections;

public class GearLock2D : MonoBehaviour {
	private bool isActive=false;
	private Vector3 startPos;
	private EnviroGear gear;
	public float minDistanceToTrigger=1;
	public float transitionTime=1;
	private float curTime=0;

	void FixedUpdate () {
		if (isActive && curTime<transitionTime) {
			curTime += Time.fixedDeltaTime;
			gear.transform.position = Vector3.Lerp(startPos, transform.position, curTime/transitionTime);
		}
	}

	void OnTriggerStay2D(Collider2D coll) {
		EnviroGear gear = coll.GetComponent<EnviroGear>();
		if (gear!=null && gear.isMovable && !isActive && Vector3.Distance(transform.position, gear.transform.position)<minDistanceToTrigger) {
			gear.isMovable = false;
			gear.Start();
			this.gear = gear;
			startPos = gear.transform.position;
			isActive = true;
		}
	}
}
