#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Transparent))]
public class Button2D : MonoBehaviour {
	public enum ButtonType { moveObject, toggleObject, customAction }
	[HideInInspector] public ButtonType buttonType;
	[HideInInspector] public float duration=1;
	[HideInInspector] public float delay=0;
	[HideInInspector] public bool revert=true;
	[HideInInspector] public bool repeat=true;
	[HideInInspector] public bool allAtOnce=false;
	[HideInInspector] public Transform target;
	[HideInInspector] public Transform endPos;
	[HideInInspector] public GameObject toggleObject;
	public UnityEngine.UI.Button.ButtonClickedEvent customAction;
	public UnityEngine.UI.Button.ButtonClickedEvent undoCustomAction;
	public bool everyFrame=false;

	[HideInInspector] public int state=0;
	[HideInInspector] public float axis=0;
	[HideInInspector] public float delay2=0;
	private Transform startPos;
	private bool canGoForwards=true;
	private bool canGoBackwards=true;

	// handle transforms
	public void Start() {
		if (buttonType!=ButtonType.moveObject) {
			target = null;
		} if (buttonType!=ButtonType.toggleObject) {
			toggleObject = null;
		} if (buttonType!=ButtonType.customAction) {
			customAction.RemoveAllListeners();
			undoCustomAction.RemoveAllListeners();
		}
		if (target==null)
			target = new GameObject("Target").transform;
		if (endPos==null)
			endPos = target;

		startPos = new GameObject("Start Pos").transform;
		startPos.position = target.position;
		startPos.localScale = target.localScale;
		startPos.rotation = target.rotation;
		if (duration==60)
			duration = 3600;
	}

	public void Update() {
		switch (state) {
		// waiting to undo
		case -2:
			delay2 -= Time.deltaTime;
			if (delay2<0) {
				SetState(-1);
				Update();
			}
			break;
		// undo changes
		case -1:
			axis = Mathf.Max(axis-Time.deltaTime, 0);
			if (everyFrame)
				undoCustomAction.Invoke();
			target.position = Vector3.Lerp(startPos.position, endPos.position, axis/duration);
			target.localScale = Vector3.Lerp(startPos.localScale, endPos.localScale, axis/duration);
			target.rotation = Quaternion.Lerp(startPos.rotation, endPos.rotation, axis/duration);

			// detect for transition completion
			if (axis<=0)
				SetState(0);
			break;
		// apply changes
		case 1:
			axis = Mathf.Min(axis+Time.deltaTime, duration);
			axis += Time.deltaTime;
			if (everyFrame)
				customAction.Invoke();
			target.position = Vector3.Lerp(startPos.position, endPos.position, axis/duration);
			target.localScale = Vector3.Lerp(startPos.localScale, endPos.localScale, axis/duration);
			target.rotation = Quaternion.Lerp(startPos.rotation, endPos.rotation, axis/duration);

			// detect for transition completion
			if (axis>=duration)
				SetState(0);
			break;
		// waiting to apply changes
		case 2:
			delay2 -= Time.deltaTime;
			if (delay2<0) {
				SetState(1);
				Update();
			}
			break;
		}
	}

	public void OnTriggerEnter2D(Collider2D coll) {
		// go forwards, if possible
		if (coll.GetComponent<PlayerGear>())
			SetState(2);
	}

	public void OnTriggerExit2D(Collider2D coll) {
		// go backwards, if possible
		if (coll.GetComponent<PlayerGear>())
			SetState(-2);
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
		case -2:
			// go backwards if possible
			if (canGoBackwards && axis>0 && revert) {
				state = -2;
				delay2 = delay;
			}
			// stop going forwards if still going forwards
			if (state>0)
				SetState(0);
			break;
		case -1:
			state = -1;
			undoCustomAction.Invoke();
			if (toggleObject)
				toggleObject.SetActive(!toggleObject.activeSelf);
			break;
		case 0:
			// stop if possible
			if (!allAtOnce || (delay2<0 && (axis<=0 || axis>=duration)))
				state = 0;
			break;
		case 1:
			state = 1;
			customAction.Invoke();
			if (toggleObject)
				toggleObject.SetActive(!toggleObject.activeSelf);
			break;
		case 2:
			// go forwards if possible
			if (canGoForwards && axis<duration) {
				state = 2;
				delay2 = delay;
			}
			// stop going backwards if still going backwards
			if (state==-1)
				SetState(0);
			break;
		}
	}

	public void TestFunctionTrue() { /*print("TestFunctionTrue();");*/ }
	public void TestFunctionFalse() { /*print("TestFunctionFalse();");*/ }
}


#if UNITY_EDITOR
[CustomEditor(typeof(Button2D))]
public class Button2DEditor:Editor {
	public override void OnInspectorGUI() {
		// initialize vars
		SerializedProperty duration = serializedObject.FindProperty("duration");
		SerializedProperty delay = serializedObject.FindProperty("delay");
		SerializedProperty revert = serializedObject.FindProperty("revert");
		SerializedProperty repeat = serializedObject.FindProperty("repeat");
		SerializedProperty allAtOnce = serializedObject.FindProperty("allAtOnce");
		SerializedProperty target = serializedObject.FindProperty("target");
		SerializedProperty endPos = serializedObject.FindProperty("endPos");
		SerializedProperty toggleObject = serializedObject.FindProperty("toggleObject");
		Button2D obj = (Button2D)this.target;

		// handle type
		obj.buttonType = (Button2D.ButtonType)EditorGUILayout.EnumPopup("Button Type", obj.buttonType);
		if (obj.buttonType==Button2D.ButtonType.moveObject) {
			target.objectReferenceValue = (Transform)EditorGUILayout.ObjectField("Target", target.objectReferenceValue, typeof(Transform), true);
			endPos.objectReferenceValue = (Transform)EditorGUILayout.ObjectField("End Position", endPos.objectReferenceValue, typeof(Transform), true);
		} else if (obj.buttonType==Button2D.ButtonType.toggleObject) {
			toggleObject.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("Toggle Object", toggleObject.objectReferenceValue, typeof(GameObject), true);
		} else if (obj.buttonType==Button2D.ButtonType.customAction) {
			base.OnInspectorGUI();
		}

		// generic variables
		duration.floatValue = EditorGUILayout.Slider("Duration", duration.floatValue, 0, 60);
		delay.floatValue = EditorGUILayout.Slider("Delay", delay.floatValue, 0, 60);
		revert.boolValue = EditorGUILayout.Toggle("Revert", revert.boolValue);
		repeat.boolValue = EditorGUILayout.Toggle("Repeat", repeat.boolValue);
		allAtOnce.boolValue = EditorGUILayout.Toggle("All At Once", allAtOnce.boolValue);
		serializedObject.ApplyModifiedProperties();
		//obj.axis = EditorGUILayout.Slider("Axis", obj.axis, 0, 60);
		//obj.delay2 = EditorGUILayout.Slider("Delay2", obj.delay2, 0, 60);
		//obj.state = EditorGUILayout.IntField("State", obj.state);
	}
}
#endif
