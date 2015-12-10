using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System;

public class PlayerGear:CollidingObject {
	public float maxSpeed = 4f;
	public bool adjustForSize=false;
	public float acceleration=16f;
	public bool m_AirControl = false;
	public GameObject stickyAura;
	public float curSpeed=0;

	private bool engaged;
	public Gear attachedTo;
	private bool facingRight = true;

	public override void Start() {
		base.Start();

		// adjust vars
		stickyAura.transform.localScale = Vector3.zero;
		if (adjustForSize) {
			maxSpeed *= radius;
			acceleration *= radius;
		}
	}

	public override void PhysicsUpdate(int percolate=int.MaxValue) {
		// read inputs, adjust vars
		bool crouch = Input.GetKey(KeyCode.LeftShift);
		float xrate = CrossPlatformInputManager.GetAxisRaw("Horizontal");
		float y = CrossPlatformInputManager.GetAxisRaw("Vertical");
		engage(crouch);

		// adjust speed
		Vector3 speedDir = (attachedTo==null
			?Vector3.right
			:attachedTo.GetVelAtPoint(transform.position).normalized);
		Vector3 defaultSpeed = (attachedTo==null
			?velocity-Vector3.Project(velocity, speedDir)
			:attachedTo.GetVelAtPoint(transform.position));
		//curSpeed = Vector3.Dot(Vector3.Project(velocity, speedDir), speedDir);
		//curSpeed += Time.fixedDeltaTime*acceleration*(xrate-curSpeed/maxSpeed);
		velocity = defaultSpeed + curSpeed*speedDir;
		Vector3 friction = Vector3.zero;
		curAngularVelocity = -curSpeed*180/Mathf.PI/radius+(attachedTo!=null?attachedTo.curAngularVelocity:0);

		// move
		base.PhysicsUpdate(percolate-1);
		
		// move
		if (attachedTo!=null) {
			/*base.PhysicsUpdate(percolate-1);
			float mult = (numGroundedTo>0?1:0.1f);
			for (int i=0; i<numGroundedTo; ++i)
				if (groundedTo[i].gameObject.GetComponent<Collider>().material.bounciness>0.01f)
					mult = 0.1f;
			velocity -= Time.fixedDeltaTime*Vector3.right*(acceleration*velocity.x/maxSpeed)*mult;
			velocity += Time.fixedDeltaTime*Vector3.right*xrate*acceleration*mult;
			transform.Rotate(Vector3.back, Time.fixedDeltaTime*velocity.x*180/Mathf.PI/radius);*/
		} else {
			/*// rotate this, move this around the center
			float lastRotVel = rotVel;
			rotVel += Time.fixedDeltaTime*xrate*acceleration/(transform.position-transform.parent.position).magnitude;
			rotVel -= Time.fixedDeltaTime*(acceleration*rotVel/maxSpeed);
			//print(rotVel);
			Vector3 pos = transform.position;
			transform.Rotate(Vector3.back * Time.fixedDeltaTime*rotVel*180/Mathf.PI);
			Vector3 temp=transform.position;
			transform.RotateAround(transform.parent.position, Vector3.back, Time.fixedDeltaTime*(rotVel)*180/Mathf.PI);
			//print("moved "+(transform.position-pos).magnitude/Time.fixedDeltaTime);
			//print("speed: "+(transform.position-temp).magnitude/Time.fixedDeltaTime+", expected speed: "+rotVel+"ratio: "+(transform.position-temp).magnitude/Time.fixedDeltaTime/rotVel+", radius: "+radius);
			// apply forces of acceleration and gravity to the parent
			EnviroGear gear = transform.parent.GetComponent<EnviroGear>();
			Rigidbody rigidBody2 = gear.GetComponent<Rigidbody>();
			gear.angularMomentum += mass*(lastRotVel-rotVel) - 9.81f*0.1f*Vector3.Dot(Vector3.right, transform.position-transform.parent.position);
			if (gear.isMovable)
				rigidBody2.velocity += Vector3.down*9.81f*mass;
				//rigidBody2.velocity += Vector3.left*(9.81f*0.1f*Vector3.Dot(Vector3.left, transform.position-transform.parent.position));
			//gear.angularMomentum += 200*(xrate-rotVel)*mass*radius - 9.81f*0.1f*Vector3.Dot(Vector3.right, (transform.position-transform.parent.position));
			//rigidBody2.velocity += Vector3.right*0;*/
		}
	}
	
	public void engage(bool eng)
	{
		engaged = eng;
		//stickyAura.SetActive (eng);

		if (eng) {
			transform.tag = "StickyAura";
			stickyAura.transform.localScale = new Vector3(.761f,.761f,.761f);
		}
		else
		{
			transform.tag = "Player";
			stickyAura.transform.localScale = Vector3.zero;
		}
	}
	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public override void OnTriggerEnter(Collider coll) {
		base.OnTriggerEnter(coll);
		//print("engaged: "+engaged+"; tag: "+coll.gameObject.tag+"; attachedTo: "+attachedTo);
		if (coll.gameObject.tag=="gear" && (attachedTo==null || coll.gameObject!=attachedTo.gameObject) && engaged) {
			attachedTo = coll.GetComponent<Gear>();
			print("attaching");
			// project velocity onto the parent gear
			//Vector3 vel = attachedTo.veloi
		}
	}
	
	public override void OnTriggerExit(Collider coll) {
		base.OnTriggerExit(coll);
		if (coll.gameObject.tag=="gear" && attachedTo!=null && coll.gameObject==attachedTo.gameObject) {
			attachedTo = null;
			print("detatching; attachedTo: "+attachedTo);
		}
	}

	public override void OnCollisionEnter(Collision coll)
	{
		base.OnCollisionEnter(coll);
		if (coll.gameObject.transform==transform.parent)
			return;

		Gear other = coll.gameObject.GetComponent<Gear>();
		if (other!=null && other.isMovable)
		{
			// average out velocities
			//print("this.vel: "+velocity+"; other.vel: "+other.velocity+"; impulse: "+coll.impulse);
			Vector3 totalVel = other.mass*other.velocity+rigidbody.mass*velocity-rigidbody.mass*coll.impulse;
			float totalMass = other.mass+rigidbody.mass;
			Vector3 finalVel = totalVel/totalMass;
			//print("finalVel: "+finalVel);
			other.velocity = 2*finalVel;
			velocity = 2*finalVel;
		}
	}

	void OnCollisionStay(Collision coll)
	{
		OnCollisionEnter(coll);
	}
}
