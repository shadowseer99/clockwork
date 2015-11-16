using UnityEngine;
using System.Collections;

public class Pulley : EnviroGear {

	public MovingPlatform leftObj;
	public MovingPlatform rightObj;
	public float leftRope;
	public float rightRope;
	private float radius;

	void Start ()
	{
		Vector3 local = transform.localScale;
		radius = GetComponent<SphereCollider>().radius * Mathf.Max(local.x, Mathf.Max(local.y, local.z));

	}



	/*void FixedUpdate ()
	{

	}*/

	public override void FixedUpdate ()
	{
		base.FixedUpdate();

		float totalPull=0;
		float inertia=angularMomentum;

		// sum weights and inertia
		if (leftObj!=null)
		{
			totalPull += leftObj.weight;
			inertia += leftObj.weight;
		}
		if (rightObj!=null)
		{
			totalPull -= rightObj.weight;
			inertia += leftObj.weight;
		}

		float estimatedAngularVel = curAngularVelocity + Time.fixedDeltaTime*radius*(180/Mathf.PI)*totalPull/inertia;
		if (estimatedAngularVel>0)
		{
			if (leftObj!=null && leftObj.isResting)
			{
				totalPull -= leftObj.weight;
				inertia -= leftObj.weight;
			}

		} else
		{
			if (rightObj!=null && rightObj.isResting)
			{
				totalPull += rightObj.weight;
				inertia -= leftObj.weight;
			}
		}
		
		
		// sum of forces = ma
		// I*a = sum of torques
		// a = (sum of torques)/I
		// w += (sum of torques)/I
		curAngularVelocity += Time.fixedDeltaTime*radius*(180/Mathf.PI)*totalPull/inertia;


		float diff = curAngularVelocity*radius*(180/Mathf.PI);
		leftRope += diff;
		rightRope -= diff;
		//leftObj.SetRopeLength(leftRope);
		//rightObj.SetRopeLength(rightRope);
	}
}
