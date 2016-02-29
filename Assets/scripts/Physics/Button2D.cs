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
	[HideInInspector]public bool dontMoveTrigger;
	[HideInInspector]public AudioCustom _buttonOn;
	[HideInInspector]public AudioCustom _buttonOff;

	[HideInInspector] public int state=0;
	[HideInInspector] public float axis=0;
	[HideInInspector] public float delay2=0;
	private Collider2D[] colls;
	private Transform startPos;
	private bool canGoForwards=true;
	private bool canGoBackwards=true;
	private PlayerGear player;
	private AudioSource buttonOn;
	private AudioSource buttonOff;

	// handle transforms
	public void Start() {
		// handle audio
		if (buttonOn==null && _buttonOn!=null) {
			buttonOn = gameObject.AddComponent<AudioSource>();
			buttonOn.clip = _buttonOn.clip;
			buttonOn.volume = _buttonOn.volume;
		}
		if (buttonOff==null && _buttonOff!=null) {
			buttonOff = gameObject.AddComponent<AudioSource>();
			buttonOff.clip = _buttonOff.clip;
			buttonOff.volume = _buttonOff.volume;
		}
		if (buttonType!=ButtonType.moveObject) target = null;
		if (buttonType!=ButtonType.toggleObject) toggleObject = null;
		if (target==null) target = new GameObject("Target").transform;
		if (endPos==null) endPos = target;
		if (buttonType!=ButtonType.customAction) {
			customAction.RemoveAllListeners();
			undoCustomAction.RemoveAllListeners();
		}

		startPos = new GameObject("Start Pos").transform;
		startPos.position = target.position;
		startPos.localScale = target.localScale;
		startPos.rotation = target.rotation;

		if (isParentObjOrSameObj(transform, endPos)) {
			print("fixing button parent-child issue");
			Transform newEndPos = new GameObject("End Position").transform;
			newEndPos.position = endPos.position;
			newEndPos.localScale = endPos.localScale;
			newEndPos.rotation = endPos.rotation;
			endPos = newEndPos;
		}

		if (duration==60)
			duration = 3600;
		colls = GetComponents<Collider2D>();
		dontMoveTrigger = dontMoveTrigger && target==transform;
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
			if (everyFrame && buttonType==ButtonType.customAction) undoCustomAction.Invoke();
			target.position = Vector3.Lerp(startPos.position, endPos.position, axis/duration);
			target.localScale = Vector3.Lerp(startPos.localScale, endPos.localScale, axis/duration);
			target.rotation = Quaternion.Lerp(startPos.rotation, endPos.rotation, axis/duration);
			for (int i=0; i<colls.Length&&dontMoveTrigger; ++i)
				if (colls[i].isTrigger)
					colls[i].offset = transform.InverseTransformPoint(startPos.position);

			// detect for transition completion
			if (axis<=0)
				SetState(0);
			break;
		// apply changes
		case 1:
			axis = Mathf.Min(axis+Time.deltaTime, duration);
			if (everyFrame && buttonType==ButtonType.customAction) customAction.Invoke();
			target.position = Vector3.Lerp(startPos.position, endPos.position, axis/duration);
			target.localScale = Vector3.Lerp(startPos.localScale, endPos.localScale, axis/duration);
			target.rotation = Quaternion.Lerp(startPos.rotation, endPos.rotation, axis/duration);
			for (int i=0; i<colls.Length&&dontMoveTrigger; ++i)
				if (colls[i].isTrigger)
					colls[i].offset = transform.InverseTransformPoint(startPos.position);

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
		PlayerGear player = coll.GetComponent<PlayerGear>();
		if (player) {
			SetState(2);
			this.player = player;
		}
	}

	public void OnTriggerExit2D(Collider2D coll) {
		// go backwards, if possible
		PlayerGear player = coll.GetComponent<PlayerGear>();
		if (player) {
			SetState(-2);
			this.player = player;
		}
	}

	private void SetState(int newState) {
		if (newState==state)
			return;

		// make changes based off of the old state
		if (state==-1 && !repeat) canGoBackwards = false;
		if (state==1 && !repeat) canGoForwards = false;

		// adjust to the new state
		if (newState==-2) {
			// go backwards if possible
			if (canGoBackwards && axis>0 && revert && !(allAtOnce&&state>0)) {
				state = -2;
				delay2 = delay;
				if (buttonOn!=null) buttonOn.Play();
			}
			// stop going forwards if still going forwards
			if (state>0)
				SetState(0);
		} else if (newState==-1) {
			state = -1;
			if (buttonType==ButtonType.customAction) undoCustomAction.Invoke();
			if (toggleObject) toggleObject.SetActive(!toggleObject.activeSelf);
		} else if (newState==0) {
			// stop if possible, handle allAtOnce special cases
			if (allAtOnce  && canGoForwards && axis==0 && player.coll.IsTouching(colls[0])) {
				state = 2;
				if (buttonOn!=null) buttonOn.Play();
			} else if (allAtOnce && canGoBackwards && revert && axis==duration && !player.coll.IsTouching(colls[0])) {
				state = -2;
				if (buttonOff!=null) buttonOff.Play();
			} else if (!allAtOnce || (delay2<0 && (axis<=0 || axis>=duration))) {
				state = 0;
			}
		} else if (newState==1) {
			state = 1;
			if (buttonType==ButtonType.customAction) customAction.Invoke();
			if (toggleObject) toggleObject.SetActive(!toggleObject.activeSelf);
		} else if (newState==2) {
			// go forwards if possible
			if (canGoForwards && axis<duration && !(allAtOnce&&state<0)) {
				state = 2;
				delay2 = delay;
				if (buttonOff!=null) buttonOff.Play();
			}
			// stop going backwards if still going backwards
			if (state==-1)
				SetState(0);
		}
	}

	public bool isParentObjOrSameObj(Transform parent, Transform child) {
		return child==parent || child.parent!=null && (child.parent==parent || isParentObjOrSameObj(parent, child.parent));
	}
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
		SerializedProperty dontMoveTrigger = serializedObject.FindProperty("dontMoveTrigger");
		SerializedProperty buttonOn = serializedObject.FindProperty("_buttonOn");
		SerializedProperty buttonOff = serializedObject.FindProperty("_buttonOff");
		Button2D obj = (Button2D)this.target;

		// handle type
		obj.buttonType = (Button2D.ButtonType)EditorGUILayout.EnumPopup("Button Type", obj.buttonType);
		if (obj.buttonType==Button2D.ButtonType.moveObject) {
			target.objectReferenceValue = (Transform)EditorGUILayout.ObjectField("Target", target.objectReferenceValue, typeof(Transform), true);
			endPos.objectReferenceValue = (Transform)EditorGUILayout.ObjectField("End Position", endPos.objectReferenceValue, typeof(Transform), true);
			if (target.objectReferenceValue==obj.transform)
				dontMoveTrigger.boolValue = EditorGUILayout.Toggle("Don't Move Trigger", dontMoveTrigger.boolValue);
		} else if (obj.buttonType==Button2D.ButtonType.toggleObject) {
			toggleObject.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("Toggle Object", toggleObject.objectReferenceValue, typeof(GameObject), true);
		} else if (obj.buttonType==Button2D.ButtonType.customAction) {
			base.OnInspectorGUI();
		}
		EditorGUILayout.PropertyField(buttonOn);
		EditorGUILayout.PropertyField(buttonOff);

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
