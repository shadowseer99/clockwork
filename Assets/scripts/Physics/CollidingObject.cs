﻿#if UNITY_EDITOR
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
	// interaction variables
	[HideInInspector]public List<Water> inwaters=new List<Water>();
	[HideInInspector]public List<Collision2D> collidingWith=new List<Collision2D>();
	[HideInInspector]public List<Collision2D> groundedTo=new List<Collision2D>();
	[HideInInspector]public List<CollidingObject> neighbors=new List<CollidingObject>();
	[HideInInspector]public CollidingObject attachedTo;
	[HideInInspector]protected bool attaching=false;

	// physics variables
	public float curSpeed=0;
	public float maxSpeed=4;
	//public float additionalFriction=0;
	public float accel=4;
	//public float power=4;
	public float accelMult=1;
	public bool airControl=true;
	public GameObject pegObject;
	public int numPegs=0;
	public float pegOffset=0;
	public float origAngle;
	protected Collision2D lastGroundedTo=null;
	protected float timeSinceGrounded=0;

	// sounds
	public AudioCustom _move;
	public AudioCustom _collHit;
	public AudioCustom _hitSurface;
	private AudioSource move;
	private AudioSource collHit;
	private AudioSource hitSurface;

	public override void Start() {
		// handle negative acceleration
		base.Start();
		if (accel<0) {
			accelMult = -1;
			accel = Mathf.Abs(accel);
		}
		if (Application.isPlaying) {
			move = gameObject.AddComponent<AudioSource>();
			collHit = gameObject.AddComponent<AudioSource>();
			hitSurface = gameObject.AddComponent<AudioSource>();
			move.clip = _move.clip;
			collHit.clip = _collHit.clip;
			hitSurface.clip = _hitSurface.clip;
			move.loop = true;
			collHit.loop = false;
			hitSurface.loop = false;
			move.volume = _move.volume;

			Vector3 v = transform.TransformDirection(Vector3.right);
			origAngle = Mathf.Atan2(v.y, v.x);
		}
	}

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
		velocity += Time.fixedDeltaTime*(density-cumDensity)*Physics.gravity;
		float f = Time.fixedDeltaTime*cumThickness;//(1-1/(Time.fixedDeltaTime*cumThickness+1));
		velocity = f*cumVelocity + (1-f)*velocity;

		// handle acceleration
		// if attached to another gear
		if (attachedTo!=null) {
			Vector3 diff = transform.position-attachedTo.transform.position;
			transform.position = 0.02f*transform.position
				+ 0.98f * (attachedTo.transform.position + diff.normalized*(attachedTo.collRadius+this.collRadius));
			curSpeed += Accel();
			curAngularVelocity = CurSpeedToAngularVelocity();
			transform.RotateAround(attachedTo.transform.position, Vector3.forward,
				Time.fixedDeltaTime*(attachedTo.curAngularVelocity - curSpeed*180/Mathf.PI/diff.magnitude));
			velocity = Vector3.zero;
		}
		// if not moving
		else if (!isMovable) {
			curSpeed += Accel();
			curAngularVelocity = CurSpeedToAngularVelocity();
		}
		// if grounded and not attached
		else {
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

		// misc
		if (groundedTo.Count>0) {
			lastGroundedTo = groundedTo[0];
			timeSinceGrounded = 0;
		} else {
			timeSinceGrounded += Time.fixedDeltaTime;
		}
		collidingWith.Clear();
		groundedTo.Clear();
		if ((isMovable && Mathf.Abs(curSpeed)>0.5f && timeSinceGrounded<0.2f) && !move.isPlaying) move.Play();
		if ((!isMovable || Mathf.Abs(curSpeed)<0.5f || timeSinceGrounded>0.2f) && move.isPlaying) move.Stop();
		if (move.isPlaying) move.volume = Mathf.Pow(Mathf.Min(1, Mathf.Abs(curSpeed/maxSpeed)), 2);
	}

	public virtual void OnTriggerEnter2D(Collider2D coll) {
		// add neighboring CollidingObjects
		CollidingObject collObj = coll.GetComponent<CollidingObject>();
		if (collObj!=null && !neighbors.Contains(collObj)) {
			neighbors.Add(collObj);

			// align pegs
			this.GetPegOffset(collObj);
			collObj.GetPegOffset(this);
		}

		// handle entering water
		Water water = coll.gameObject.GetComponent<Water>();
		if (water!=null) {
			water.splash.Play();
			inwaters.Add(water);
		}

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
			Vector3 velocity2 = GetVelAtPoint(attachedTo.transform.position);
			velocity -= accelMult*curSpeed*velocity2.normalized;
			attachedTo = null;
			print("vel: "+velocity.magnitude);
		}
	}

	public virtual void OnCollisionStay2D(Collision2D coll) {
		// add to collidingWith, try adding to groundedTo
		collidingWith.Add(coll);
		if (transform.position.y-coll.contacts[0].point.y>0)
			groundedTo.Add(coll);
	}

	public void OnCollisionEnter2D(Collision2D coll) {
		float vol = Mathf.Pow(Mathf.Min(Mathf.Abs(Vector3.Dot(coll.contacts[0].normal, lastVelocity))/maxSpeed, 1), 3);
		CollidingObject obj = coll.gameObject.GetComponent<CollidingObject>();
		if (coll.gameObject.tag!="Bouncy")
			velocity -= 0.95f*Vector3.Project(velocity, coll.contacts[0].normal);
		if ((obj!=null && (obj.isMovable || this.isMovable)) && (!collHit.isPlaying || vol>collHit.volume)) {
			collHit.Play();
			collHit.volume = vol*_collHit.volume;
		}
		if (obj==null && (!hitSurface.isPlaying || vol>hitSurface.volume)) {
			hitSurface.volume = vol*_hitSurface.volume;
			hitSurface.Play();
		}
	}

	// helper functions
	public float Accel() {
		float mult = 0;
		if (airControl) mult = 0.1f;
		if (inwaters.Count>0) mult = 0.5f;
		if (groundedTo.Count>0 || attachedTo!=null || !isMovable) mult = 1;
		float result = Time.fixedDeltaTime*mult*accel*(accelMult-curSpeed/maxSpeed);
		if (groundedTo.Count>0 && groundedTo[0].collider.sharedMaterial!=null && groundedTo[0].collider.sharedMaterial.name=="Ramp" && result*curSpeed<0 && accelMult==0) result *= 0.1f;
		if (result*curSpeed<0 && accelMult*curSpeed>0) result = 0;
		return result;
	}
	public float CurSpeedToAngularVelocity() {
		if (attachedTo!=null)
			return -curSpeed*180/(Mathf.PI*(collRadius+collRadius*collRadius/attachedTo.collRadius));
		else
			return -curSpeed*180/Mathf.PI/collRadius;
	}
	public float AngularVelocityToCurSpeed() {
		if (attachedTo!=null)
			return -curAngularVelocity*(Mathf.PI*(collRadius+collRadius*collRadius/attachedTo.collRadius))/180;
		else
			return -curAngularVelocity*collRadius*Mathf.PI/180;
	}
	public float GetPegOffset(CollidingObject obj) {
		Vector3 diff = obj.transform.position - transform.position;
		Vector3 v1 = transform.TransformDirection(Vector3.right);
		float diffAngle = Mathf.Atan2(diff.y, diff.x)*180/Mathf.PI;
		float angle = Mathf.Atan2(v1.y, v1.x)*180/Mathf.PI;
		float cumAngle = angle+diffAngle - this.pegOffset - origAngle + 720;
		float pegAngle = 360f/numPegs;
		float pegOffset = (cumAngle%pegAngle)/pegAngle - 0.5f;
		if (name=="Player Gear" || name=="gear04") print(name+": dangle="+diffAngle+", angle="+angle+", cumAngle="+cumAngle+", final="+pegOffset);
		return pegOffset;
			/*Vector3 diff = coll.transform.position - transform.position;
			Vector3 v1 = transform.TransformDirection(Vector3.right);
			Vector3 v2 = coll.transform.TransformDirection(Vector3.right);
			float diffAngle = Mathf.Atan2(diff.y, diff.x)*180/Mathf.PI;
			float angle1 = Mathf.Atan2(v1.y, v1.x)*180/Mathf.PI + diffAngle;
			float angle2 = Mathf.Atan2(v2.y, v2.x)*180/Mathf.PI - diffAngle;
			float pegAngle1 = angle1%(360/numPegs);
			float pegAngle2 = angle2%(360/collObj.numPegs);
			if (pegAngle1<0) pegAngle1 += 360f/numPegs;
			if (pegAngle2<0) pegAngle2 += 360f/collObj.numPegs;
			float test = angle1%15;
			print(name+": "+angle1+", "+angle2+", "+diffAngle);
			print("self peg angle: "+ (pegAngle1));
			print("other peg angle: "+(pegAngle2));*/

	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(CollidingObject))][CanEditMultipleObjects]
public class CollidingObjectEditor:PhysicsObjectEditor {}
#endif
