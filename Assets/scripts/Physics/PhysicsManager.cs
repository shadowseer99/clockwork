using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PhysicsManager : MonoBehaviour {
	// core data structures
	private List<PhysicsObject> objs=new List<PhysicsObject>();
	private List<CollidingObject> colls=new List<CollidingObject>();

	// temporary data structures
	private Queue<PhysicsObject> tovisit=new Queue<PhysicsObject>();
	private uint visitedNum;
	private uint POSITIVE { get { return visitedNum+2; } }
	private uint NEGATIVE { get { return visitedNum+1; } }
	private List<List<PhysicsObject>> gearSets=new List<List<PhysicsObject>>();

	private static PhysicsManager _physicsManager;
	public static PhysicsManager physicsManager {
		get {
			// if null, create new PhysicsManager and populate
			if (_physicsManager==null) {
				_physicsManager = new GameObject("Physics Manager").AddComponent<PhysicsManager>();
				PhysicsObject[] objs = GameObject.FindObjectsOfType<PhysicsObject>();
				for (int i=0; i<objs.Length; ++i)
					physicsManager.AddObject(objs[i]);
			}
			return _physicsManager;
		}
	}

	/// <summary>Adds obj to the data structures, starts updating obj.
	/// Returns true if obj hadn't already been added.</summary>
	public bool AddObject(PhysicsObject obj) {
		if (!objs.Contains(obj)) {
			objs.Add(obj);
			if (obj is CollidingObject)
				colls.Add(obj as CollidingObject);
			return true;
		}
		return false;
	}

	/// <summary>Removes obj from the data structures, stops updating obj.
	/// Returns true if obj was successfully removed.</summary>
	public bool RemoveObject(PhysicsObject obj) {
		if (objs.Contains(obj)) {
			objs.Remove(obj);
			if (obj is CollidingObject)
				colls.Remove(obj as CollidingObject);
			return true;
		}
		return false;
	}

	public void FixedUpdate() {
		visitedNum += 2;

		// remove destroyed objects
		for (int i=0; i<objs.Count; ++i) {
			if (objs[i]==null) {
				RemoveObject(objs[i--]);
				continue;
			}
		}

		// update CollidingObjects
		foreach (CollidingObject coll in colls) {
			if (IsVisited(coll))
				continue;

			// add torque for acceleration AND gravity
			if (coll.attachedTo!=null)
				coll.attachedTo.angularMomentum += coll.mass*Time.fixedDeltaTime*coll.accel*(coll.accelMult-coll.curSpeed/coll.maxSpeed)
					- 9.81f*coll.mass*Vector3.Dot(Vector3.right, coll.transform.position-coll.attachedTo.transform.position);
			// sum angularMomentum, distribute according to moment of inertia
			/*float totalAngularMomentum = coll.angularMomentum;
			float totalMomentOfInertia = coll.momentOfInertia;
			foreach (CollidingObject neighbor in coll.neighbors) {
				if (IsVisited(neighbor))
					continue;
				tovisit.Enqueue(neighbor);
				totalAngularMomentum -= neighbor.angularMomentum;
				totalMomentOfInertia += neighbor.momentOfInertia;
			}*/
		}

		// update PhysicsObjects
		for (int i=0; i<objs.Count; ++i)
			objs[i].PhysicsUpdate();
	}

	// helper functions
	private bool IsVisited(PhysicsObject obj) { return obj.visited>visitedNum; }
}
