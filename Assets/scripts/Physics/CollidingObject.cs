using UnityEngine;
using System.Collections;

public class CollidingObject : PhysicsObject {
	protected Water inwater;
	protected Collision[] collidingWith;
	protected int numCollidingWith=0;
	protected Collision[] groundedTo;
	protected int numGroundedTo=0;

	public override void Start() {
		base.Start();
		int arrSize = Mathf.Min(GameObject.FindObjectsOfType<Collider>().Length, 100);
		collidingWith = new Collision[arrSize];
		groundedTo = new Collision[arrSize];
		inwater = Water.nullWater;
	}

	public override void PhysicsUpdate(int percolate=int.MaxValue) {
		base.PhysicsUpdate(percolate-1);

		// handle grounded
		numGroundedTo = 0;
		for (int i=0; i<numCollidingWith; ++i)
			if (collidingWith[i].impulse.y>0)
				groundedTo[numGroundedTo++] = collidingWith[i];
		numCollidingWith = 0;

		// handle water
		// use https://www.mathsisfun.com/geometry/circle-sector-segment.html for better water
		velocity += Time.fixedDeltaTime*(1-inwater.densityRatio)*Physics.gravity;
		float f = (1-1/(Time.fixedDeltaTime*inwater.thicknessRatio+1));
		velocity = f*inwater.flow + (1-f)*velocity;
	}

	public virtual void OnCollisionEnter(Collision coll) {
		collidingWith[numCollidingWith++] = coll;
	}

	public virtual void OnTriggerEnter(Collider coll) {
		Water tempWater = coll.gameObject.GetComponent<Water>();
		if (inwater==Water.nullWater && tempWater!=null)
			inwater = tempWater;
	}

	public virtual void OnTriggerExit(Collider coll) {
		if (inwater!=Water.nullWater && inwater==coll.gameObject.GetComponent<Water>())
			inwater = Water.nullWater;
	}
}
