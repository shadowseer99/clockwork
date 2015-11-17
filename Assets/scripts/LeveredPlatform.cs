using UnityEngine;
using System.Collections;

public class LeveredPlatform : MovingPlatform {

	public GameObject pivot;
	public GameObject connection;
	public GameObject topPivot;
	public GameObject restingPivot;
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
	}

	public override Vector3 SetRopeLength(float length, Vector3 connection)
	{
		// calculate error/isResting from last time
		Vector3 error = targetPos-body.position;
		Vector3 diff = targetPos-lastPos;
		float lDiff = length-lastLength;
		isResting = error.magnitude/diff.magnitude>0.1f && !firstTime;
		isTaut = 1.05f*(body.position-connection).magnitude>lastLength || firstTime;
		lastLength = length;
		firstTime = false;

		// move to the appropriate position
		targetPos = connection + Vector3.down * length;
		lastPos = body.position;
		body.position = new Vector3(connection.x, body.position.y, 0);
		if (!isResting || targetPos.y>lastPos.y)
			body.MovePosition(targetPos);

		return targetPos;
	}
}
