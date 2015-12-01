using System;
using UnityEngine;
using System.Collections.Generic;
public class GearGuyCtrl1 : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 4f;
	[SerializeField] private bool adjustForSize=false;
	[SerializeField] private float acceleration=16f;
    [SerializeField] private bool m_AirControl = false;
	[SerializeField] private GameObject stickyAura;
	public float mass=0.5f;

	private Water inwater;
	private Collision[] collidingWith;
	private int numCollidingWith=0;
	private Collision[] groundedTo;
	private int numGroundedTo=0;
	private bool engaged;
	private float groundDist;
    private Rigidbody rigidBody;
	private SphereCollider collider;
    private bool facingRight = true;  // For determining which way the player is currently facing.
	private float rotVel=0;
	private float radius;
	private float localRadius;

	public Stack<GameObject> gearChildren = new Stack<GameObject>();
    private void Awake()
    {
        // Setting up references.
        rigidBody = GetComponent<Rigidbody>();
		collider = GetComponent<SphereCollider>();
		stickyAura.transform.localScale = Vector3.zero;

		// adjust vars
		groundDist = gameObject.GetComponent<Collider>().bounds.extents.y;
		int arrSize = Math.Min(GameObject.FindObjectsOfType<Collider>().Length, 100);
		collidingWith = new Collision[arrSize];
		groundedTo = new Collision[arrSize];
		if (adjustForSize) {
			maxSpeed *= transform.localScale.x;
			acceleration *= transform.localScale.x;
		}
		localRadius = collider.radius*Mathf.Max(Mathf.Max(transform.localScale.x, transform.localScale.y), transform.localScale.z);
		inwater = Water.nullWater;
    }

	public void FixedUpdate()
	{
		/*RaycastHit rayhit;
		//if (Physics.Raycast(transform.position, Vector3.down, out rayhit, groundDist+.1f))
		if (Physics.SphereCast(transform.position, radius*0.45f, Vector3.down, out rayhit, radius))
			groundedObj = rayhit.collider.gameObject;
		else
			groundedObj = null;*/

	}


	public void Move(float xrate,float yrate, bool crouch, bool jump)
    {
		// If the player should jump...
		/*
		if (grounded && jump&&transform.parent==null)
		{
			// Add a vertical force to the player.
			grounded = false;
			rigidBody.AddForce(new Vector3(0f, jumpSpeed,0));

		}
		*/

		// handle grounded
		numGroundedTo = 0;
		for (int i=0; i<numCollidingWith; ++i)
			if (collidingWith[i].impulse.y>0)
				groundedTo[numGroundedTo++] = collidingWith[i];
		numCollidingWith = 0;
			

		if (transform.parent == null) 
		{
			float mult = (numGroundedTo>0?1:0.1f);
			for (int i=0; i<numGroundedTo; ++i)
				if (groundedTo[i].gameObject.GetComponent<Collider>().material.bounciness>0.01f)
					mult = 0.1f;
			rigidBody.velocity -= Time.fixedDeltaTime*Vector3.right*(acceleration*rigidBody.velocity.x/maxSpeed)*mult;
			rigidBody.velocity += Time.fixedDeltaTime*Vector3.right*xrate*acceleration*mult;
			rigidBody.velocity += Time.fixedDeltaTime*(1-inwater.densityRatio)*Physics.gravity;
			float f = Time.fixedDeltaTime*(inwater.thicknessRatio-1);
			rigidBody.velocity = f*inwater.flow + (1-f)*rigidBody.velocity;
			rigidBody.velocity += Time.fixedDeltaTime*inwater.flow*inwater.thicknessRatio;
			transform.Rotate(Vector3.back, Time.fixedDeltaTime*rigidBody.velocity.x*180/Mathf.PI/localRadius);
		}else 
		{
			// rotate this, move this around the center
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
			//rigidBody2.velocity += Vector3.right*0;
		}

		int ii = -1;
        foreach (GameObject go in gearChildren) {
			go.transform.Rotate(rigidBody.angularVelocity*ii);
			ii*=-1;
		}
        /*
		if (!engaged)
			m_Rigidbody.AddForce (Vector3.right * xrate * m_MaxSpeed);
		else
			transform.Rotate (Vector3.forward * m_MaxSpeed);
        */
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
	void OnTriggerEnter(Collider coll)
	{
		if (engaged && coll.gameObject.tag == "gear") {
			// handle parent
			if (transform.parent==coll.transform)
				return;

			rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
            transform.parent = coll.gameObject.transform;
			radius = transform.localPosition.magnitude;
			//alter angular momentum...
			//transform.parent.GetComponent<EnviroGear>().angularMomentum -= 20;
			transform.parent.GetComponent<EnviroGear>().momentOfIntertia += mass*radius*radius;
		}

		Water tempWater = coll.gameObject.GetComponent<Water>();
		if (inwater==Water.nullWater && tempWater!=null)
			inwater = tempWater;
	}

    //deparent from gears

    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "gear"&&transform.parent==coll.transform)
        {
			Vector3 diff = Vector3.zero;//transform.position-coll.transform.position;
			Vector3 gearSpeed = coll.GetComponent<EnviroGear>().GetVelAtPoint(transform.position);
			Vector3 speed = Vector3.Cross((transform.position-transform.parent.position), Vector3.back).normalized*rotVel*radius*50*Mathf.PI/180;
			rigidBody.velocity += diff + gearSpeed - speed;

			transform.parent.GetComponent<EnviroGear>().momentOfIntertia -= mass*radius*radius;
			transform.SetParent(null);
        }

		if (inwater!=Water.nullWater && inwater==coll.gameObject.GetComponent<Water>())
			inwater = Water.nullWater;
    }

	void OnCollisionEnter(Collision coll)
	{
		collidingWith[numCollidingWith++] = coll;
		if (coll.gameObject.transform==transform.parent)
			return;

		Rigidbody other = coll.gameObject.GetComponent<Rigidbody>();
		EnviroGear gear = coll.gameObject.GetComponent<EnviroGear>();
		if (gear!=null && gear.isMovable)
		{
			// average out velocities
			//print("this.vel: "+rigidBody.velocity+"; other.vel: "+other.velocity+"; impulse: "+coll.impulse);
			Vector3 totalVel = other.mass*other.velocity+rigidBody.mass*rigidBody.velocity-rigidBody.mass*coll.impulse;
			float totalMass = other.mass+rigidBody.mass;
			Vector3 finalVel = totalVel/totalMass;
			//print("finalVel: "+finalVel);
			other.velocity = 2*finalVel;
			rigidBody.velocity = 2*finalVel;
		}
	}

	void OnCollisionStay(Collision coll)
	{
		OnCollisionEnter(coll);
	}
}