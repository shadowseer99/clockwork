using UnityEngine;
using System.Collections;

public class Wench : EnviroGear {
	
	public MovingPlatform obj;
	public bool isOnRightSide;
	public float rope;
	private float radius;
	public Vector3 conPoint;
	void Start ()
	{
		Vector3 local = transform.localScale;
		radius = GetComponent<SphereCollider>().radius * Mathf.Max(local.x, Mathf.Max(local.y, local.z));
		conPoint = transform.position + radius * (isOnRightSide ? Vector3.right : Vector3.left);
		rope = conPoint.y - obj.connectionPoint.y;
	}
	private void Rachet()
	{
		if (isOnRightSide) {
			angularMomentum=Mathf.Max(0,angularMomentum);
		}
		else
		{
			angularMomentum=Mathf.Min(0,angularMomentum);
		}
	}
	public override void FixedUpdate ()
	{
		Rachet ();
		base.FixedUpdate();
		Rachet ();
		float totalPull = (isOnRightSide ? -obj.mass : obj.mass);

		float inertia=angularMomentum+obj.mass;
		

		float estimatedAngularVel = curAngularVelocity + Time.fixedDeltaTime*radius*(180/Mathf.PI)*totalPull/inertia;
		curAngularVelocity += Time.fixedDeltaTime*radius*(180/Mathf.PI)*totalPull/inertia;
		float diff = 0;
		if (estimatedAngularVel > 0 && isOnRightSide) 
		{
			diff = curAngularVelocity * radius * (180 / Mathf.PI);
		} else if (estimatedAngularVel < 0 && !isOnRightSide) 
		{
			diff = curAngularVelocity * radius * (180 / Mathf.PI);
		}
		
		
		// sum of forces = ma
		// I*a = sum of torques
		// a = (sum of torques)/I
		// w += (sum of torques)/I
		rope += Mathf.Abs (diff);
		obj.SetRopeLength (rope,conPoint);

	}
}
