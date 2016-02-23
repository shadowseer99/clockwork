#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[DisallowMultipleComponent]
[RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D))]
[ExecuteInEditMode]
public class PhysicsObject:MonoBehaviour {
	// core variables
	[HideInInspector]
	public float mass {
		get { return (rigidbody==null?0:rigidbody.mass); }
		set { if (rigidbody!=null) rigidbody.mass = value; } }
	public float density {
		get { return mass/area; }
		set { mass=value*area; } }
	public float momentOfInertia {
		get { return momentOfInertiaMult*mass*collRadius*collRadius; }
		set { mass=value/momentOfInertiaMult/collRadius/collRadius; } }
	public bool isMovable=true;
	public Vector3 velocity {
		get { return (rigidbody==null?Vector2.zero:rigidbody.velocity); }
		set { if (rigidbody!=null) rigidbody.velocity = (isMovable?value:Vector3.zero); } }
	protected Vector3 lastVelocity { get; private set; }
	public Vector3 momentum {
		get { return velocity*mass; }
		set { velocity = value/mass; } }
	public bool isRotatable=true;
	public float curAngularVelocity;
	public float angularMomentum {
		get { return momentOfInertia*curAngularVelocity; }
		set { curAngularVelocity = value/momentOfInertia; } }
	// helper variables
	public float collRadius          { get; private set; }
	public float trigRadius          { get; private set; }
	public float area                { get; private set; }
	public float momentOfInertiaMult { get; private set; }
	public CircleCollider2D coll       { get; private set; }
	public CircleCollider2D trig       { get; private set; }
	new public Rigidbody2D rigidbody   { get; private set; }

	// physics manager variables
	[HideInInspector] public PhysicsManager.GearSet gearSet;
	[HideInInspector] public uint visited;

	public virtual void Start() {
		if (Application.isPlaying)
			PhysicsManager.physicsManager.Start();
		HandleColliders();
	}

	public virtual void Update() {
#if UNITY_EDITOR
		while (UnityEditorInternal.ComponentUtility.MoveComponentUp(this)) { }
		if (!Application.isPlaying)
			HandleColliders();
#endif
	}

	//public virtual void FixedUpdate() { PhysicsUpdate(); }

	public virtual void PhysicsUpdate() {}

	public virtual void Move() {
		if (isRotatable)
			transform.Rotate(Time.fixedDeltaTime*curAngularVelocity*Vector3.forward, Space.World);
		if (!isMovable)
			velocity = Vector3.zero;
		lastVelocity = velocity;
	}

	/// <summary>Returns the speed of an object rotating around this object at the given point.</summary>
	public Vector3 GetVelAtPoint(Vector3 point) {
		// use the cross product, multiply by angularSpeed in radians
		Vector3 diff = point-transform.position;
		Vector3 result = Vector3.Cross(Vector3.forward, diff);
		// extra logic to prevent returning Vector3.zero
		return result*(Mathf.Abs(curAngularVelocity)>0.001f?curAngularVelocity:(float)1e-3)*Mathf.PI/180;
	}

	/// <summary>Resets radius/area, based off of the collider values</summary>
	private void HandleColliders() {
		// adjust rigidbody
		rigidbody = GetComponent<Rigidbody2D>();
		rigidbody.isKinematic = !isMovable;
		rigidbody.gravityScale = 0;
		gameObject.layer = LayerMask.NameToLayer(isMovable?"Default":"Static Gear");
		rigidbody.constraints = (isMovable?RigidbodyConstraints2D.FreezeRotation:RigidbodyConstraints2D.FreezeAll);

		// find coolider, set radius/area/momentOfInertiaMult
		Transform t = transform;
		collRadius = area = 0;
		momentOfInertiaMult = 0.01f;
		CircleCollider2D[] colls = GetComponents<CircleCollider2D>();
		for (int i=0; i<colls.Length; ++i) {
			// find constants
			float radius = colls[i].radius*Mathf.Sqrt(
				t.TransformVector(t.InverseTransformVector(Vector3.up).normalized).magnitude
				*t.TransformVector(t.InverseTransformVector(Vector3.right).normalized).magnitude);
			momentOfInertiaMult = 0.5f;
			// store results as trigger OR collider
			if (colls[i].isTrigger) {
				this.trig = colls[i];
				this.trigRadius = radius;
			} else {
				this.coll = colls[i];
				this.collRadius = radius;
				area = Mathf.PI*collRadius*collRadius;
			}
		}
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(PhysicsObject))][CanEditMultipleObjects]
public class PhysicsObjectEditor:Editor {
	public enum MassType { mass, density, momentOfInertia }
	public static MassType massType;
	public override void OnInspectorGUI() {
		PhysicsObject obj = (PhysicsObject)target;
		EditorGUILayout.BeginHorizontal();
		massType = (MassType)EditorGUILayout.EnumPopup(massType);
		if (massType==MassType.mass)
			obj.mass = EditorGUILayout.FloatField(obj.mass);
		else if (massType==MassType.density)
			obj.density = EditorGUILayout.FloatField(obj.density);
		else if (massType==MassType.momentOfInertia)
			obj.momentOfInertia = EditorGUILayout.FloatField(obj.momentOfInertia);
		EditorGUILayout.EndHorizontal();
		base.OnInspectorGUI();
	}

	/*protected void TrySetPrefabMod() {
		PropertyModification[] mods = PrefabUtility.GetPropertyModifications(target);
		if (mods!=null) {

		}
	}*/
}
#endif
