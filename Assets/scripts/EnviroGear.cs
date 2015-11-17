using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnviroGear : MonoBehaviour {
	public float curAngularVelocity=0;
	public float maxAngularVelocity=90f;
	public float angularAcceleration=10f;
	public float mass=1f;
	public bool isMovable=false;
	public bool hasForce { get { return angularAcceleration!=0; } }
	public bool isGolden=false;
	
	private float goldenRotation=0;
	private List<EnviroGear> neighbors=new List<EnviroGear>();
	private Transform gearTrans;
	protected float radius;
	private Rigidbody rigidBody;
	
	public float momentOfIntertia;
	public float angularMomentum {
		get { return momentOfIntertia*curAngularVelocity; }
		set { curAngularVelocity = value/momentOfIntertia; }
	}
	
	public virtual void Start()
	{
		// initialize vars
		Vector3 local = transform.localScale;
		radius = GetComponent<SphereCollider>().radius * Mathf.Max(local.x, Mathf.Max(local.y, local.z));
		rigidBody = GetComponent<Rigidbody>();
		momentOfIntertia = 0.5f*mass*transform.localScale.y*transform.localScale.z*81;
		
		// handle static gears and isGolden
		if (!isMovable)
			gameObject.layer = LayerMask.NameToLayer("Static Gear");
		/*if (isGolden)
		{
			GetComponent<Renderer>().material.color = new Color(.886f, 0.925f, 0f);
		}*/
	}
	
	public virtual void FixedUpdate()
	{
		// handle goldenGear
		if (isGolden)
		{
			goldenRotation += Time.fixedDeltaTime*curAngularVelocity;
			if (Mathf.Abs(goldenRotation) > 360)
			{
				GameObject.FindObjectOfType<MenuManager>().LoadLevel();
				return;
			}
		}

		// rotate and apply torques
		transform.Rotate(Time.deltaTime*curAngularVelocity*Vector3.left);
		curAngularVelocity += Time.fixedDeltaTime * (angularAcceleration - Mathf.Abs(angularAcceleration)*curAngularVelocity/maxAngularVelocity);

		// average out angular speed of neighbors
		for (int i=0; i<neighbors.Count; ++i)
		{
			//print(angularMomentum+"-"+momentOfIntertia);
			// sum angularMomentum, distribute according to moment of inertia
			float totalAngularMomentum = angularMomentum - neighbors[i].angularMomentum;
			float totalMomentOfInertia = momentOfIntertia + neighbors[i].momentOfIntertia;
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

	void OnTriggerEnter(Collider coll)
	{
		// find a gear, return if null
		EnviroGear gear = coll.gameObject.GetComponent<EnviroGear>();
		if (gear==null)
			return;

		// add gear to neighbors if it isn't already a neighbor
		if (!gear.neighbors.Contains(this) && !neighbors.Contains(gear))
			neighbors.Add(gear);
	}

	void OnTriggerStay(Collider coll)
	{
		rigidBody.velocity = Vector3.zero;
	}

	void OnTriggerExit(Collider coll)
	{
		// find a gear, return if null
		EnviroGear gear = coll.gameObject.GetComponent<EnviroGear>();
		if (gear==null)
			return;

		// remove gear in neighbors if it's in the neighbors
		neighbors.Remove(gear);
	}
}
