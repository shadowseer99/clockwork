using UnityEngine;
using System.Collections;
using UnityEditor;
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
	[HideInInspector]public List<CollidingObject> neighbors=new List<CollidingObject>();
	public float curSpeed=0;
	public float maxSpeed=4;
	public float accel=4;
	[HideInInspector]public float accelMult=1;
	public CollidingObject attachedTo;
	protected bool attaching=false;

	public override void PhysicsUpdate() {
		// handle water
		// use https://www.mathsisfun.com/geometry/circle-sector-segment.html for better water
		/*velocity += Time.fixedDeltaTime*(1-inwater.densityRatio)*Physics.gravity;
		float f = (1-1/(Time.fixedDeltaTime*inwater.thicknessRatio+1));
		velocity = f*inwater.flow + (1-f)*velocity;*/
		
		// update physics
		base.PhysicsUpdate();

		// if attached to another gear
		if (attachedTo!=null) {
			Vector3 diff = transform.position-attachedTo.transform.position;
			transform.position = attachedTo.transform.position + diff.normalized*(attachedTo.collRadius+this.collRadius);
			float xrate = CrossPlatformInputManager.GetAxisRaw("Horizontal");
			curSpeed += Time.fixedDeltaTime*accel*(xrate-curSpeed/maxSpeed);
			curAngularVelocity = -curSpeed*180/(Mathf.PI*(collRadius+collRadius*collRadius/attachedTo.collRadius));
			transform.RotateAround(attachedTo.transform.position, Vector3.forward,
				Time.fixedDeltaTime*(attachedTo.curAngularVelocity - curSpeed*180/Mathf.PI/diff.magnitude));
			velocity = Vector3.zero;
		}
		
		// if grounded and not attached
		if (attachedTo==null && groundedTo.Count>0) {
			// helper vectors
			Vector3 speedDir = Vector3.right;
			Vector3 projVel = Vector3.Project(velocity, speedDir);
			Vector3 defaultSpeed = velocity-projVel;
			// calculate curSpeed, in the direction of speedDir
			curSpeed = Vector3.Dot(projVel, speedDir);
			curSpeed += Time.fixedDeltaTime*accel*(accelMult-curSpeed/maxSpeed);
			velocity = defaultSpeed + curSpeed*speedDir;
			curAngularVelocity = -curSpeed*180/Mathf.PI/collRadius+(attachedTo!=null?attachedTo.curAngularVelocity:0);
		}
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

	public virtual void OnCollisionEnter2D(Collision2D coll) {
		// add to collidingWith, try adding to groundedTo
		collidingWith.Add(coll);
		if (transform.position.y-coll.contacts[0].point.y>0)
			groundedTo.Add(coll);

		Gear other = coll.gameObject.GetComponent<Gear>();
		if (other!=null && other.isMovable && this.isMovable) {
			throw new NotImplementedException("impulse isn't implemented");
			// average out velocities
			/*print("this.vel: "+velocity+"; other.vel: "+other.velocity+"; impulse: "+coll.contacts);
			Vector3 totalVel = other.mass*other.velocity+rigidbody.mass*velocity-rigidbody.mass*coll.impulse;
			float totalMass = other.mass+rigidbody.mass;
			Vector3 finalVel = totalVel/totalMass;
			//print("finalVel: "+finalVel);
			other.velocity = 2*finalVel;
			velocity = 2*finalVel;*/
		}
	}

	void OnCollisionStay2D(Collision2D coll) {
		OnCollisionEnter2D(coll);
	}
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
