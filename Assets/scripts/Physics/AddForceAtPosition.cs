using UnityEngine;
using System.Collections;

public class AddForceAtPosition : MonoBehaviour {
	public Vector3 point;
	public Vector3 force;
	
	void Update () {
		GetComponent<PhysicsObject>().AddForceAtPoint(point, force);
	}
}
