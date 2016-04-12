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
	public float airControl=0.1f;
	public GameObject pegObject;
	public int numPegs=0;
	public float pegOffset=0;
	public float origAngle;
	protected Collision2D lastGroundedTo=null;
	protected float timeSinceGrounded=0;
	[HideInInspector]public float waterThickness=0;
	[HideInInspector]public float waterDensity=0;
	[HideInInspector]public Vector3 waterVelocity=Vector3.zero;

	// sounds
	public AudioCustom _move;
	public AudioCustom _collHit;
	public AudioCustom _hitSurface;
	private AudioSource move;
	private AudioSource collHit;
	private AudioSource hitSurface;

	public override void Start() {
		base.Start();
		
		// handle negative acceleration, 0 maxSpeed
		maxSpeed = Mathf.Max(Mathf.Abs(maxSpeed), 0.01f);
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

			Vector3 v = transform.TransformDirection(Vector3.right);
			origAngle = Mathf.Atan2(v.y, v.x);
			if (accel<0.001f) accelMult = 0;
		}
	}

	public override void PhysicsUpdate() {
		// handle water
		float cumArea=0;
		waterDensity=0;
		waterThickness=0;
		waterVelocity=Vector3.zero;
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
				waterDensity += area*inwaters[i].densityRatio;
				waterThickness += area*inwaters[i].thicknessRatio;
				waterVelocity += area*inwaters[i].thicknessRatio*inwaters[i].flow;
			}
		}
		// adjust results
		float totalArea = Mathf.PI*collRadius*collRadius;
		waterThickness /= Mathf.Max(totalArea, cumArea);
		waterDensity /= Mathf.Max(totalArea, cumArea);
		waterVelocity /= Mathf.Max(totalArea, cumArea);
		if (waterThickness>0)
			waterVelocity /= waterThickness;
		// update velocity
		velocity += Time.fixedDeltaTime*(density-waterDensity)*Physics.gravity;
		float f = Time.fixedDeltaTime*waterThickness;//(1-1/(Time.fixedDeltaTime*waterThickness+1));
		velocity = f*waterVelocity + (1-f)*velocity;

		// update curSpeed, accel, etc.
		if (attachedTo!=null) PhysicsUpdateAttached();
		else if (!isMovable) PhysicsUpdateNotMoving();
		else PhysicsUpdateMoving();
		
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
		if (move.isPlaying) move.volume = Mathf.Pow(Mathf.Min(1, Mathf.Abs(curSpeed/maxSpeed)), 2)*_move.volume;
	}

	public virtual void PhysicsUpdateAttached() {
		Vector3 diff = transform.position-attachedTo.transform.position;
		transform.position = 0.02f*transform.position
			+ 0.98f * (attachedTo.transform.position + diff.normalized*(attachedTo.collRadius+this.collRadius));
		curSpeed += Accel();
		curAngularVelocity = CurSpeedToAngularVelocity();
		transform.RotateAround(attachedTo.transform.position, Vector3.forward,
			Time.fixedDeltaTime*(attachedTo.curAngularVelocity - curSpeed*180/Mathf.PI/diff.magnitude));
		velocity = attachedTo.velocity;
	}

	public virtual void PhysicsUpdateNotMoving() {
		curSpeed += Accel();
		curAngularVelocity = CurSpeedToAngularVelocity();
		velocity = Vector3.zero;
	}

	public virtual void PhysicsUpdateMoving(bool overrideCurspeed=true) {
		// find average ground velocity, average angle
		Vector3 avgGroundVel = Vector3.zero;
		Vector3 normal = Vector3.zero;
		int groundCount=0, normCount=0;
		for (int i=0; i<groundedTo.Count; ++i) {
			if (groundedTo[i].rigidbody!=null) {
				avgGroundVel += (Vector3)groundedTo[i].rigidbody.velocity;
				++groundCount;
			}
			for (int j=0; j<groundedTo[i].contacts.Length; ++j)
				normal += (Vector3)groundedTo[i].contacts[j].normal;
			normCount += groundedTo[i].contacts.Length;
		}
		normal /= Mathf.Max(1, normCount);
		avgGroundVel /= Mathf.Max(1, groundCount);

		// helper vectors
		Vector3 speedDir = Vector3.right;// Vector3.Cross(normal, Vector3.forward);
		Vector3 projVel = Vector3.Project(velocity, speedDir);
		Vector3 defaultSpeed = velocity/*-avgGroundVel*/-projVel;
		// calculate curSpeed, in the direction of speedDir
		if (overrideCurspeed) curSpeed = Vector3.Dot(projVel, speedDir) + Accel();
		//if (name=="PCgear") print("speedDir: "+100*speedDir+", projVel: "+100*projVel+", defaultSpeed: "+100*defaultSpeed+", curSpeed: "+curSpeed);
		velocity = defaultSpeed /*+ avgGroundVel*/ + curSpeed*speedDir;
		curAngularVelocity = CurSpeedToAngularVelocity();
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
			if (!inwaters.Contains(water)) {
				water.splash.Play();
				inwaters.Add(water);
			}
		}

		// handle attaching
		if (collObj!=null && (attachedTo==null || coll!=attachedTo) && attaching) {
			PhysicsUpdate();
			attachedTo = collObj;
			if (this is PlayerGear && collObj is Gear)
				(collObj as Gear).hasPlayerGear = true;
			isMovable = false;
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
			//print("detatching from "+attachedTo.name);
			isMovable = true;
			Vector3 velocity = attachedTo.GetVelAtPoint(transform.position);
			Vector3 velocity2 = -accelMult*curSpeed*GetVelAtPoint(attachedTo.transform.position).normalized;
			this.velocity = velocity + velocity2;
			attachedTo = null;
			if (this is PlayerGear && collObj is Gear)
				(collObj as Gear).hasPlayerGear = false;
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
		// find a multiplication constant
		float mult = 0;
		if (airControl!=0) mult = airControl;
		if (inwaters.Count>0) mult = 0.5f + 0.5f*mult;
		if (groundedTo.Count>0 || attachedTo!=null || !isMovable) mult = 1;
		float result = Time.fixedDeltaTime*mult*accel*(accelMult-curSpeed/maxSpeed);
		// don't slow down if accelMult==0 AND in the air OR on a Ramp
		if (accelMult==0 && result*curSpeed<0 && (groundedTo.Count==0 || (groundedTo[0].collider.sharedMaterial!=null && groundedTo[0].collider.sharedMaterial.name=="Ramp"))) result = 0;
		// don't slow down if moving fast...
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
