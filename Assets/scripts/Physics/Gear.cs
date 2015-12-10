using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gear:CollidingObject {
	public float maxAngularVelocity=90;
	public float angularAcceleration=90;
	public bool isGolden=false;
	private float goldenRotation=0;
	private List<Gear> neighbors=new List<Gear>();

	public override void PhysicsUpdate(int percolate=int.MaxValue) {
		
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
		

		// rotate/move and apply torques
		base.PhysicsUpdate(percolate-1);
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
			float totalMomentOfInertia = momentOfInertia + neighbors[i].momentOfInertia;
			angularMomentum = totalAngularMomentum*momentOfInertia/totalMomentOfInertia;
			neighbors[i].angularMomentum = -totalAngularMomentum*neighbors[i].momentOfInertia/totalMomentOfInertia;
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
		Gear gear = coll.gameObject.GetComponent<Gear>();
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
			rigidbody.velocity = Vector3.zero;
	}

	public virtual void OnTriggerExit(Collider coll)
	{
		// find a gear, return if null
		Gear gear = coll.gameObject.GetComponent<Gear>();
		if (gear==null)
			return;

		// remove gear in neighbors if it's in the neighbors
		neighbors.Remove(gear);
	}
}
