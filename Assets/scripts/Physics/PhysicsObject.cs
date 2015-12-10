#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
[ExecuteInEditMode]
public class PhysicsObject:MonoBehaviour {
	// core variables
	[HideInInspector]
	public float mass=1;
	public float density {
		get { return mass/area; }
		set { mass=value*area; } }
	public float momentOfInertia {
		get { return momentOfInertiaMult*mass*radius*radius; }
		set { mass=value/momentOfInertiaMult/radius/radius; } }
	public bool isMovable=true;
	public Vector3 velocity {
		get { return rigidbody.velocity; }
		set { rigidbody.velocity = value; } }
	public Vector3 momentum {
		get { return velocity*mass; }
		set { velocity = value/mass; } }
	public bool isRotatable=true;
	public float curAngularVelocity;
	public float angularMomentum {
		get { return momentOfInertia*curAngularVelocity; }
		set { curAngularVelocity = value/momentOfInertia; } }
	// helper variables
	protected float radius;
	protected float area;
	protected float momentOfInertiaMult;
	protected Collider coll;
	protected Rigidbody rigidbody;

	// other variables
	protected bool rotateAround=false;
	protected Vector3 center;
	[HideInInspector] public uint id;
	[HideInInspector] public uint visited;
	[HideInInspector] public uint setId;
	[HideInInspector] public bool speed;

	public virtual void Start() {
		HandleColliders();
	}

	public virtual void Update() {
#if UNITY_EDITOR
		while (UnityEditorInternal.ComponentUtility.MoveComponentUp(this)) { }
		if (!Application.isPlaying) {
			HandleColliders();
		}
#endif
	}

	public virtual void FixedUpdate() { PhysicsUpdate(); }

	public virtual void PhysicsUpdate(int percolate=int.MaxValue) {
		/*if (isMovable)
			transform.position += Time.fixedDeltaTime*velocity;*/
		if (isRotatable)
			transform.Rotate(Time.fixedDeltaTime*curAngularVelocity*Vector3.forward, Space.World);
	}

	public void AddForceAtPoint(Vector3 point, Vector3 force) {
		print("AddForceAtPoint("+point+", "+force+")");
		print("radius of gyration: "+Mathf.Sqrt(momentOfInertia/area));
		force *= Time.fixedDeltaTime;
		Vector3 diff = point-transform.position;
		float newMomentOfInertia=diff.sqrMagnitude*mass;
		float totalMomentOfInertia=(momentOfInertia+newMomentOfInertia);
		print("newMoment: "+newMomentOfInertia+"; totalMoment: "+totalMomentOfInertia);
		float cross = Vector3.Dot(Vector3.Cross(diff, force), Vector3.forward)*180/Mathf.PI;
		float ratio = newMomentOfInertia/totalMomentOfInertia;
		print("ratio: "+ratio+"; cross: "+cross+"; isMovable: "+isMovable+"; isRotatable: "+isRotatable);
		if (isMovable) {
			if (isRotatable) {
				velocity += force*(1-ratio);
			} else {
				velocity += force;
			}
		}
		if (isRotatable) {
			if (isMovable) {
				angularMomentum += cross*ratio;
			} else {
				angularMomentum += cross;
			}
		}
	}

	/// <summary>Resets radius/area, based off of the collider values</summary>
	private void HandleColliders() {
		// adjust rigidbody
		rigidbody = GetComponent<Rigidbody>();
		rigidbody.isKinematic = !isMovable;
		rigidbody.useGravity = false;
		gameObject.layer = LayerMask.NameToLayer(isMovable?"Default":"Static Gear");
		rigidbody.constraints = (isMovable?RigidbodyConstraints.FreezeRotation|RigidbodyConstraints.FreezePositionZ:RigidbodyConstraints.FreezeAll);

		// find coolider, set radius/area/momentOfInertiaMult
		Transform t = transform;
		radius = area = momentOfInertiaMult = -1;
		Collider[] colls = GetComponents<Collider>();
		for (int i=0; i<colls.Length; ++i) {
			if (!colls[i].isTrigger) {
				Type type = colls[i].GetType();
				if (type==typeof(SphereCollider)) {
					SphereCollider coll = (SphereCollider)colls[i];
					radius = coll.radius*Mathf.Sqrt(
						t.TransformVector(t.InverseTransformVector(Vector3.up).normalized).magnitude
						*t.TransformVector(t.InverseTransformVector(Vector3.right).normalized).magnitude);
					area = Mathf.PI*radius*radius;
					momentOfInertiaMult = 0.5f;
				} else if (type==typeof(BoxCollider)) {
					throw new Exception("An Incompatible Collider has been attached to "+gameObject.name);
					//BoxCollider coll = (BoxCollider)colls[i];
					//radius = coll.size.magnitude*GetTransformMagnitude(transform)/Mathf.Sqrt(3);
					//area = radius*radius;
				} else {
					throw new Exception("An Incompatible Collider has been attached to "+gameObject.name);
				}
			}
		}
		//print("radius: "+radius);
		//print("area: "+area);
		//print("momentOfInertiaMult: "+momentOfInertiaMult);
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(PhysicsObject))]
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
}
#endif
