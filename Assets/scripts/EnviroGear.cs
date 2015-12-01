using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnviroGear : MonoBehaviour {
	public float curAngularVelocity=0;
	public float maxAngularVelocity=90f;
	public float angularAcceleration=10f;
	public float mass=1f;
	public bool isMovable=false;
	public float momentOfIntertia;
	public bool hasForce { get { return angularAcceleration!=0; } }
	public bool isGolden=false;

	private float goldenRotation=0;
	private List<EnviroGear> neighbors=new List<EnviroGear>();
	private Transform gearTrans;
	protected float radius;
	private Rigidbody rigidBody;
	private SphereCollider col;
	
	public float angularMomentum {
		get { return momentOfIntertia*curAngularVelocity; }
		set { curAngularVelocity = value/momentOfIntertia; }
	}
	
	public virtual void Start()
	{
		// initialize vars
		Vector3 local = transform.localScale;
		col = GetComponent<SphereCollider>();
		radius = col.radius * Mathf.Max(local.x, Mathf.Max(local.y, local.z));
		rigidBody = GetComponent<Rigidbody>();
		momentOfIntertia = 0.5f*mass*transform.localScale.y*transform.localScale.z*81;
		//momentOfIntertia = 0.5f*mass*radius*radius;
		
		// handle static gears
		rigidBody.isKinematic = !isMovable;
		rigidBody.useGravity = isMovable;
		gameObject.layer = LayerMask.NameToLayer(isMovable?"Default":"Static Gear");
		rigidBody.constraints = (isMovable?RigidbodyConstraints.FreezeRotation|RigidbodyConstraints.FreezePositionZ:RigidbodyConstraints.FreezeAll);
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

		if (name=="envirogear (4)") {
			print("right: "+transform.right);
			print("forward: "+transform.forward);
			print("up: "+transform.up);
		}
		//transform.Rotate(
		// rotate/move and apply torques
		transform.Rotate(Time.fixedDeltaTime*curAngularVelocity*Vector3.forward, Space.World);
		if (isMovable)
			transform.position += Time.fixedDeltaTime*Vector3.left*curAngularVelocity*radius*(Mathf.PI/180);
		curAngularVelocity += Time.fixedDeltaTime*(isMovable?0:angularAcceleration);
		curAngularVelocity -= Time.fixedDeltaTime*Mathf.Abs(angularAcceleration)*curAngularVelocity/maxAngularVelocity;

		// average out angular speed of neighbors
		for (int i=0; i<neighbors.Count; ++i)
		{
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

	public virtual void OnTriggerEnter(Collider coll)
	{
		// find a gear, return if null
		EnviroGear gear = coll.gameObject.GetComponent<EnviroGear>();
		if (gear==null)
			return;

		// add gear to neighbors if it isn't already a neighbor
		if (!gear.neighbors.Contains(this) && !neighbors.Contains(gear))
			neighbors.Add(gear);
	}

	public virtual void OnTriggerStay(Collider coll)
	{
		// NEEDS FIXING
		if (coll.GetComponent<Water>()==null)
			rigidBody.velocity = Vector3.zero;
	}

	public virtual void OnTriggerExit(Collider coll)
	{
		// find a gear, return if null
		EnviroGear gear = coll.gameObject.GetComponent<EnviroGear>();
		if (gear==null)
			return;

		// remove gear in neighbors if it's in the neighbors
		neighbors.Remove(gear);
	}
}
