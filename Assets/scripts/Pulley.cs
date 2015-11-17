using UnityEngine;
using System.Collections;

public class Pulley : EnviroGear {

	public MovingPlatform leftObj;
	public MovingPlatform rightObj;
	private Vector3 leftPos;
	private Vector3 rightPos;
	private float leftRope;
	private float rightRope;
	private LineRenderer lines;
	private int numLines=22;

	public override void Start ()
	{
		base.Start();
		leftPos = transform.position+radius*Vector3.left;
		rightPos = transform.position+radius*Vector3.right;
		leftRope = (leftPos-leftObj.transform.position).magnitude;
		rightRope = (rightPos-rightObj.transform.position).magnitude;

		// initialize the line renderer
		lines = gameObject.AddComponent<LineRenderer>();
		Color c = new Color(214/255f, 216/255f, 93/255f);
		lines.SetColors(c, c);
		lines.material = (Material)Instantiate(GetComponent<Renderer>().material);
		lines.material.color = c;
		lines.SetWidth(0.1f, 0.1f);
		lines.SetVertexCount(numLines);
		for (int i=1; i<numLines-1; ++i)
		{
			float angle = (i-1f)/(numLines-3)*Mathf.PI;
			lines.SetPosition(i, Mathf.Cos(angle)*radius*Vector3.right + Mathf.Sin(angle)*radius*Vector3.up+transform.position);
		}
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();

		// sum weights and inertia, estimate angular velocity
		float totalPull=0;
		float inertia=momentOfIntertia;
		if (!leftObj.isResting)
		{
			totalPull += leftObj.mass*-Physics.gravity.y;
			inertia += leftObj.mass;
		}
		if (!rightObj.isResting)
		{
			totalPull -= rightObj.mass*-Physics.gravity.y;
			inertia += rightObj.mass;
		}
		float estAngularAcceleration = Time.fixedDeltaTime*(180/Mathf.PI)*radius*totalPull/inertia;
		//print("inertia: "+inertia+"; totalPull: "+totalPull+"; angularAcceleration: "+angularAcceleration);

		// apply relevant forces from resting taut objects
		if (leftObj.isResting && leftObj.isTaut && curAngularVelocity+estAngularAcceleration<0)
		{
			totalPull += leftObj.mass*-Physics.gravity.y;
			inertia += leftObj.mass;
		}
		if (rightObj.isResting && rightObj.isTaut && curAngularVelocity+estAngularAcceleration>0)
		{
			totalPull -= rightObj.mass*-Physics.gravity.y;
			inertia += rightObj.mass;
		}
		float angularAcceleration = Time.fixedDeltaTime*(180/Mathf.PI)*radius*totalPull/inertia;

		// calculate new angular velocity and apply
		curAngularVelocity += angularAcceleration;
		if (leftObj.isResting && leftObj.isTaut && curAngularVelocity+estAngularAcceleration<0)
			curAngularVelocity = Mathf.Min(0, curAngularVelocity);
		if (rightObj.isResting && rightObj.isTaut && curAngularVelocity+estAngularAcceleration>0)
			curAngularVelocity = Mathf.Max(0, curAngularVelocity);
		float diff = Time.fixedDeltaTime*(Mathf.PI/180)*radius*curAngularVelocity;

		// move objects, handle hitting the pulley
		if (leftRope+diff<=0)
		{
			curAngularVelocity = -0.8f*(curAngularVelocity-angularAcceleration);
			rightRope += leftRope;
			leftRope = 0;
		}
		else if (rightRope-diff<=0)
		{
			curAngularVelocity = -0.8f*(curAngularVelocity-angularAcceleration);
			leftRope += rightRope;
			rightRope = 0;
		}
		else
		{
			leftRope += diff;
			rightRope -= diff;
		}
		


		// apply to left rope
		bool wasResting = leftObj.isResting;
		bool wasTaut = leftObj.isTaut;
		lines.SetPosition(numLines-1, leftObj.SetRopeLength(leftRope, leftPos));
		if (wasResting && !leftObj.isResting) {
			print("picked up leftObj");
			curAngularVelocity *= inertia/(inertia+leftObj.mass)/8*0;
		}

		// apply to right rope
		wasResting = rightObj.isResting;
		wasTaut = rightObj.isTaut;
		lines.SetPosition(0, rightObj.SetRopeLength(rightRope, rightPos));
		if (wasResting && !rightObj.isResting) {
			print("picked up rightObj");
			curAngularVelocity *= inertia/(inertia+rightObj.mass)/8*0;
		}
	}
}
