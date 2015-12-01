using UnityEngine;
using System.Collections;
using System;

public class Button : MonoBehaviour {
	
	[Range(0.01f, 60)]
	public float duration=1;
	[Range(0, 60)]
	public float delay=0;
	public bool revert=true;
	public bool repeat=true;
	public Transform target;
	public Transform endPos;
	public GameObject toggleObject;
	public UnityEngine.UI.Button.ButtonClickedEvent customAction;
	public UnityEngine.UI.Button.ButtonClickedEvent undoCustomAction;
	public bool everyFrame=false;
	public bool onResume=false;

	private int state=0;
	private float axis=0;
	private Transform startPos;
	private float totalDur;
	private bool canGoForwards=true;
	private bool canGoBackwards=true;

	// handle transforms
	public void Start() {
		if (target==null)
			target = new GameObject("Target").transform;
		if (endPos==null)
			endPos = target;

		startPos = new GameObject("Start Pos").transform;
		startPos.position = target.position;
		startPos.localScale = target.localScale;
		startPos.rotation = target.rotation;
		if (duration==0)
			duration = 3600;
		totalDur = duration+delay;
		if (delay!=0)
			throw new NotImplementedException("Delay hasn't yet been implemented");
		if (onResume)
			throw new NotImplementedException("On Resume hasn't yet been implemented");
	}

	public void Update() {
		switch (state) {
		case -1:
			// undo changes
			if (everyFrame)
				undoCustomAction.Invoke();
			target.position = Vector3.Lerp(startPos.position, endPos.position, axis/duration);
			target.localScale = Vector3.Lerp(startPos.localScale, endPos.localScale, axis/duration);
			target.rotation = Quaternion.Lerp(startPos.rotation, endPos.rotation, axis/duration);

			// detect for transition completion
			axis -= Time.deltaTime;
			if (axis<0)
				SetState(0);
			break;
		case 1:
			// apply changes
			if (everyFrame)
				customAction.Invoke();
			target.position = Vector3.Lerp(startPos.position, endPos.position, axis/duration);
			target.localScale = Vector3.Lerp(startPos.localScale, endPos.localScale, axis/duration);
			target.rotation = Quaternion.Lerp(startPos.rotation, endPos.rotation, axis/duration);

			// detect for transition completion
			axis += Time.deltaTime;
			if (axis>duration)
				SetState(0);
			break;
		}
	}

	public void OnTriggerEnter(Collider coll) {
		// go forwards, if possible
		if (coll.GetComponent<GearGuyCtrl1>()!=null)
			SetState(1);
	}

	public void OnTriggerExit(Collider coll) {
		// go backwards, if possible
		if (coll.GetComponent<GearGuyCtrl1>()!=null)
			SetState(-1);
	}

	private void SetState(int newState) {
		if (newState==state)
			return;

		// make changes based off of the old state
		switch (state) {
		case -1:
			if (!repeat)
				canGoBackwards = false;
			break;
		case 1:
			if (!repeat)
				canGoForwards = false;
			break;
		}

		// adjust to the new state
		switch (newState) {
		case -1:
			// go backwards if possible
			if (canGoBackwards && axis>0 && revert) {
				undoCustomAction.Invoke();
				state = -1;
				if (toggleObject)
					toggleObject.SetActive(!toggleObject.activeSelf);
			}
			// stop going forwards if still going forwards
			if (state==1)
				state = 0;
			break;
		case 0:
			state = 0;
			break;
		case 1:
			// go forwards if possible
			if (canGoForwards && axis<duration) {
				customAction.Invoke();
				state = 1;
				if (toggleObject)
					toggleObject.SetActive(!toggleObject.activeSelf);
			}
			// stop going backwards if still going backwards
			if (state==-1)
				state = 0;
			break;
		}
	}

	public void TestFunctionTrue() { /*print("TestFunctionTrue();");*/ }
	public void TestFunctionFalse() { /*print("TestFunctionFalse();");*/ }
}
