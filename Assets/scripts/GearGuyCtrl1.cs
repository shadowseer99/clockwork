using System;
using UnityEngine;
using System.Collections.Generic;
public class GearGuyCtrl1 : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 4f;
	[SerializeField] private float acceleration=16f;
    [SerializeField] private bool m_AirControl = false;
	[SerializeField] private GameObject stickyAura;
	public float mass=0.5f;

    private bool grounded; // Whether or not the player is grounded.
	private bool engaged;
	private float groundDist;
    private Rigidbody rigidBody;
	private SphereCollider collider;
    private bool facingRight = true;  // For determining which way the player is currently facing.
	private float rotVel=0;
	private float radius;

	public Stack<GameObject> gearChildren = new Stack<GameObject>();
    private void Awake()
    {
		groundDist = gameObject.GetComponent<Collider>().bounds.extents.y;
        // Setting up references.
        rigidBody = GetComponent<Rigidbody>();
		collider = GetComponent<SphereCollider>();
		stickyAura.transform.localScale = Vector3.zero;
    }

	public void FixedUpdate()
	{
		grounded = Physics.Raycast(transform.position,Vector3.down,groundDist+.1f);
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

		if (transform.parent == null) 
		{
			float mult = (grounded?1:0.1f);
			rigidBody.velocity += Time.fixedDeltaTime*Vector3.right*xrate*acceleration*mult;
			//rigidBody.velocity -= Time.fixedDeltaTime*Vector3.right*(acceleration*rigidBody.velocity.x/maxSpeed)*mult;
			transform.Rotate(Vector3.back, Time.fixedDeltaTime*rigidBody.velocity.x*collider.radius*360/Mathf.PI);
			//print(rigidBody.velocity);
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

		int i = -1;
        foreach (GameObject go in gearChildren) {
			go.transform.Rotate(rigidBody.angularVelocity*i);
			i*=-1;
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
			rigidBody.useGravity = false;
		}
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
			rigidBody.useGravity = true;
        }
    }

	void OnCollisionEnter(Collision coll)
	{
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