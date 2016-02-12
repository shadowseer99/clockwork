using System;
using UnityEngine;

public class Kickback2D:MonoBehaviour {
	public enum KickbackMode { speed, energy }
	public KickbackMode kickbackMode;
	public float mult=1;
	
	void OnCollisionExit2D(Collision2D coll) {
		if (kickbackMode==KickbackMode.speed)
			coll.rigidbody.velocity *= mult;
		else if (kickbackMode==KickbackMode.energy)
			coll.rigidbody.velocity *= Mathf.Sqrt(mult);
		else
			throw new Exception("Invalid KickbackMode");
	}
}