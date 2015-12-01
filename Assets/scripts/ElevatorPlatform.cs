using UnityEngine;
using System.Collections;

public class ElevatorPlatform : MovingPlatform {

	private Vector3 lastPos;
	private Vector3 targetPos;
	private float lastLength;
	private bool firstTime=true;

	// Update is called once per frame
	void FixedUpdate () 
	{
		mass = startMass;
		foreach(Rigidbody r in restingObjs)
			mass+=r.mass;
		/*if (gameObject.name=="Left Elevator" || true) {
			string str = startMass.ToString();
			foreach (Rigidbody r in restingObjs)
				str += "+"+r.mass;
			print("mass: "+str+" = "+mass);
		}*/
	}

	public override Vector3 SetRopeLength(float length, Vector3 connection)
	{
		// calculate error/isResting from last time
		Vector3 error = targetPos-body.position;
		Vector3 diff = targetPos-lastPos;
		float lDiff = length-lastLength;
		isResting = error.magnitude/diff.magnitude>0.1f && targetPos.y<body.position.y && !firstTime;
		isTaut = 1.001f*(body.position-connection).magnitude>lastLength || firstTime;
		lastLength = length;
		firstTime = false;
		//if (gameObject.name=="Left Elevator")
			//print("isTuat: "+isTaut+"; isResting: "+isResting);

		// move to the appropriate position
		targetPos = connection + Vector3.down * length;
		lastPos = body.position;
		body.position = new Vector3(connection.x, body.position.y, 0);
		if (!isResting || targetPos.y>lastPos.y)
			body.MovePosition(targetPos);

		return targetPos;
	}
}
