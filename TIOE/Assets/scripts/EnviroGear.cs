using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnviroGear : MonoBehaviour {
	public float maxAngularVelocity=90f;
	public float mass=1f;
	public float angularAcceleration=10f;

	private List<EnviroGear> neighbors=new List<EnviroGear>();
	private Transform gearTrans;
	float radius;
	private Rigidbody rigidBody;
	
	private float curAngularVelocity;
	private float momentOfIntertia { get { return 0.5f*mass*transform.localScale.y*transform.localScale.z; } }
	private float angularMomentum {
		get { return momentOfIntertia*curAngularVelocity; }
		set { curAngularVelocity = value/momentOfIntertia; }
	}
	
	void Start ()
	{
		// initialize vars
		radius = gameObject.GetComponent<Collider>().bounds.extents.x;
		rigidBody = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate ()
	{
		// rotate angularSpeed degrees every second
		transform.Rotate(Time.deltaTime*curAngularVelocity*Vector3.left);
		
		// add the angularAcceleration with an appropriate drag force
		curAngularVelocity += Time.fixedDeltaTime * (angularAcceleration - angularAcceleration*curAngularVelocity/maxAngularVelocity);

		// average out angular speed of neighbors
		for (int i=0; i<neighbors.Count; ++i)
		{
			//print(angularMomentum+"-"+momentOfIntertia);
			// sum angularMomentum, distribute according to moment of inertia
			float totalAngularMomentum = angularMomentum - neighbors[i].angularMomentum;
			float totalMomentOfInertia = momentOfIntertia + neighbors[i].momentOfIntertia;
			print("averaging out: totalAngularMomentum: "+totalAngularMomentum+"; totalMomentOfInertia: "+totalMomentOfInertia);
			print(totalAngularMomentum*momentOfIntertia/totalMomentOfInertia);
			angularMomentum = totalAngularMomentum*momentOfIntertia/totalMomentOfInertia;
			neighbors[i].angularMomentum = -totalAngularMomentum*neighbors[i].momentOfIntertia/totalMomentOfInertia;
		}
	}

	/// <summary>
	/// Returns the speed of an object rotating around this gear at the given point.
	/// </summary>
	public Vector3 GetVelAtPoint(Vector3 point)
	{
		// use the cross product, multiply by angularSpeed in radians
		Vector3 diff = point-transform.position;
		Vector3 result = Vector3.Cross(Vector3.forward, diff);
		return result*curAngularVelocity*Mathf.PI/180;
	}

	void OnCollisionEnter(Collision coll)
	{
		// find a gear, return if null
		EnviroGear gear = coll.gameObject.GetComponent<EnviroGear>();
		if (gear==null)
			return;

		// add gear to neighbors if it isn't already a neighbor
		if (!gear.neighbors.Contains(this) && !neighbors.Contains(gear))
			neighbors.Add(gear);
	}

	void OnCollisionStay(Collision coll)
	{
		rigidBody.velocity = Vector3.zero;
	}

	void OnCollisionExit(Collision coll)
	{
		// find a gear, return if null
		EnviroGear gear = coll.gameObject.GetComponent<EnviroGear>();
		if (gear==null)
			return;

		// remove gear in neighbors if it's in the neighbors
		neighbors.Remove(gear);
	}
}
