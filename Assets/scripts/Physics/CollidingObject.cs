#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System;
using System.Collections.Generic;

/// <summary>
/// This class handles water, collects colliding objects, and collects grounded objects
/// </summary>
public class CollidingObject:PhysicsObject {
	[HideInInspector]public List<Water> inwaters=new List<Water>();
	[HideInInspector]public List<Collision2D> collidingWith=new List<Collision2D>();
	[HideInInspector]public List<Collision2D> groundedTo=new List<Collision2D>();
	public List<CollidingObject> neighbors=new List<CollidingObject>();
	public float curSpeed=0;
	public float maxSpeed=4;
	public float accel=4;
	public float accelMult=1;
	public CollidingObject attachedTo;
	protected bool attaching=false;

	public override void PhysicsUpdate() {
		// handle water
		float cumArea=0;
		float cumDensity=0;
		float cumThickness=0;
		Vector3 cumVelocity=Vector3.zero;
		for (int i=0; i<inwaters.Count; ++i) {
			// find approximate contact point (can be improved, TODO)
			BoxCollider2D coll = inwaters[i].GetComponent<BoxCollider2D>();
			Vector3 dir = (coll.transform.position-transform.position).normalized;
			float distance = -collRadius;
			float diff = 2*collRadius;
			for (int j=0; j<20; ++j) {
				diff /= 2;
				distance += diff;
				if (coll.OverlapPoint(transform.position + distance*dir))
					distance -= diff;
			}
			// calculate area, cumulate variables (using https://www.mathsisfun.com/geometry/circle-sector-segment.html)
			if (distance<collRadius) {
				float angle = Mathf.Acos(distance/collRadius);
				float area = angle*collRadius*collRadius - distance*distance*Mathf.Tan(angle);
				cumArea += area;
				cumDensity += area*inwaters[i].densityRatio;
				cumThickness += area*inwaters[i].thicknessRatio;
				cumVelocity += area*inwaters[i].thicknessRatio*inwaters[i].flow;
			}
		}
		// adjust results
		float totalArea = Mathf.PI*collRadius*collRadius;
		cumThickness /= Mathf.Max(totalArea, cumArea);
		cumDensity /= Mathf.Max(totalArea, cumArea);
		cumVelocity /= Mathf.Max(totalArea, cumArea);
		if (cumThickness>0)
			cumVelocity /= cumThickness;
		// update velocity
		velocity += Time.fixedDeltaTime*(1-cumDensity)*Physics.gravity;
		float f = Time.fixedDeltaTime*cumThickness;//(1-1/(Time.fixedDeltaTime*cumThickness+1));
		velocity = f*cumVelocity + (1-f)*velocity;

		// handle acceleration
		// if attached to another gear
		if (attachedTo!=null) {
			Vector3 diff = transform.position-attachedTo.transform.position;
			transform.position = attachedTo.transform.position + diff.normalized*(attachedTo.collRadius+this.collRadius);
			curSpeed += Accel();
			curAngularVelocity = -curSpeed*180/(Mathf.PI*(collRadius+collRadius*collRadius/attachedTo.collRadius));
//print("rotating "+transform.position+" around "+attachedTo.transform.position
	//+" (attachedTo: "+attachedTo.curAngularVelocity+"; curSpeed: "+curSpeed+"; attachedTo: "+attachedTo.curAngularVelocity
	//+"; result: "+Time.fixedDeltaTime*(attachedTo.curAngularVelocity - curSpeed*180/Mathf.PI/diff.magnitude)+")");
			transform.RotateAround(attachedTo.transform.position, Vector3.forward,
				Time.fixedDeltaTime*(attachedTo.curAngularVelocity - curSpeed*180/Mathf.PI/diff.magnitude));
			velocity = Vector3.zero;
		}
		// if not moving
		else if (!isMovable) {
			curSpeed += Accel();
//print("oldSpeed: "+curAngularVelocity+"; newSpeed: "+CurSpeedToAngularVelocity());
			curAngularVelocity = CurSpeedToAngularVelocity();
		}
		// if grounded and not attached
		else if (attachedTo==null && groundedTo.Count>0) {
			// helper vectors
			Vector3 speedDir = Vector3.right;
			Vector3 projVel = Vector3.Project(velocity, speedDir);
			Vector3 defaultSpeed = velocity-projVel;
			// calculate curSpeed, in the direction of speedDir
			curSpeed = Vector3.Dot(projVel, speedDir);
			curSpeed += Accel();
			velocity = defaultSpeed + curSpeed*speedDir;
			curAngularVelocity = CurSpeedToAngularVelocity();
		}
		
		// update physics
		base.PhysicsUpdate();
	}

	public virtual void OnTriggerEnter2D(Collider2D coll) {
		// add neighboring CollidingObjects
		CollidingObject collObj = coll.GetComponent<CollidingObject>();
		if (collObj!=null && !neighbors.Contains(collObj))
			neighbors.Add(collObj);

		// handle entering water
		Water water = coll.gameObject.GetComponent<Water>();
		if (water!=null)
			inwaters.Add(water);

		// handle attaching
		if (collObj!=null && (attachedTo==null || coll!=attachedTo) && attaching) {
			PhysicsUpdate();
			attachedTo = collObj;
			isMovable = false;
			print("attaching to: "+attachedTo.name);
		}
	}
	public virtual void OnTriggerExit2D(Collider2D coll) {
		// add neighboring CollidingObjects
		CollidingObject collObj = coll.GetComponent<CollidingObject>();
		if (collObj!=null && neighbors.Contains(collObj))
			neighbors.Remove(collObj);

		// handle exiting water
		Water water = coll.gameObject.GetComponent<Water>();
		if (water!=null)
			inwaters.Remove(water);

		// handle detaching
		if (collObj!=null && collObj==attachedTo) {
			print("detatching; attachedTo: "+attachedTo.name);
			isMovable = true;
			velocity = attachedTo.GetVelAtPoint(transform.position);
			velocity = velocity.normalized*(velocity.magnitude-curSpeed);
			attachedTo = null;
			print("vel: "+velocity.magnitude);
		}
	}

	//public virtual void OnCollisionEnter2D(Collision2D coll) {}
	public virtual void OnCollisionStay2D(Collision2D coll) {
		//OnCollisionEnter2D(coll);
		// add to collidingWith, try adding to groundedTo
		collidingWith.Add(coll);
		if (transform.position.y-coll.contacts[0].point.y>0)
			groundedTo.Add(coll);
	}

	// helper functions
	public float Accel() { return Time.fixedDeltaTime*accel*(accelMult-curSpeed/maxSpeed); }
	public float CurSpeedToAngularVelocity() { return curSpeed*180/Mathf.PI/collRadius; }
	public float AngularVelocityToCurSpeed() { return curAngularVelocity*collRadius*Mathf.PI/180; }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CollidingObject))]
public class CollidingObjectEditor:Editor {
	public enum MassType { mass, density, momentOfInertia }
	public static MassType massType;
	public override void OnInspectorGUI() {
		CollidingObject obj = (CollidingObject)target;
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
