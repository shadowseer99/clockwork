using UnityEngine;
using System.Collections;

public class ElevatorPlatform : MovingPlatform {


	
	// Update is called once per frame
	void FixedUpdate () 
	{
		weight = startWeight;
		foreach(Rigidbody r in restingObjs)
		{
			weight+=r.mass;
		}
	}

	public override void SetRopeLength(float length,Vector3 connection)
	{
		transform.position = new Vector3(connection.x,transform.position.y,0);
		body.MovePosition (connection + Vector3.down * length);
	}
}
