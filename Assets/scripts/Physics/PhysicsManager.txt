using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PhysicsManager : MonoBehaviour {
	// core data structures
	/*private List<PhysicsObject> objs=new List<PhysicsObject>();
	private Dictionary<PhysicsObject, List<PhysicsObject>> collisions=new Dictionary<PhysicsObject, List<PhysicsObject>>();
	private uint count=0;

	// temporary data structures
	private Queue<PhysicsObject> tovisit=new Queue<PhysicsObject>();
	private uint visitedNum;
	private uint POSITIVE { get { return visitedNum+2; } }
	private uint NEGATIVE { get { return visitedNum+1; } }
	private List<List<PhysicsObject>> gearSets=new List<List<PhysicsObject>>();

	private static PhysicsManager _physicsManager;
	public static PhysicsManager physicsManager {
		get {
			if (_physicsManager==null)
				_physicsManager = new GameObject("Physics Manager").AddComponent<PhysicsManager>();
			return _physicsManager;
		}
	}

	/// <summary>Adds obj to the data structures, starts updating obj.
	/// Returns true if obj hadn't already been added.</summary>
	public bool AddObject(PhysicsObject obj) {
		bool result=false;
		try {
			if (!objs.Contains(obj) && !collisions.ContainsKey(obj)) {
				objs.Add(obj);
				collisions[obj] = new List<PhysicsObject>();
				result = true;
			}
		} catch (Exception e) {
			print("add object error: "+e.Message);
		}
		Recalculate();
		return result;
	}

	/// <summary>Removes obj from the data structures, stops updating obj.
	/// Returns true if obj was successfully removed.</summary>
	public bool RemoveObject(PhysicsObject obj) {
		bool result=false;
		try {
			result = result||objs.Remove(obj);
			result = result||collisions.Remove(obj);
			foreach (KeyValuePair<PhysicsObject, List<PhysicsObject>> coll in collisions) {
				for (int i=0; i<coll.Value.Count; ++i)
					result = result||coll.Value.Remove(obj);
			}
		} catch (Exception e) {
			print("add collision error: "+e.Message);
			result = false;
		}
		Recalculate();
		return result;
	}

	/// <summary>Adds a collision between obj1 and obj2, starts handling the collision.
	/// Returns true if it successfully added the collision, false if it was already added or an error occurred.</summary>
	public bool AddCollision(PhysicsObject obj1, PhysicsObject obj2) {
		try {
			if (obj1.id<obj2.id) {
				if (!collisions[obj1].Contains(obj2)) {
					collisions[obj1].Add(obj2);
					return true;
				}
			} else {
				if (!collisions[obj2].Contains(obj1)) {
					collisions[obj2].Add(obj1);
					return true;
				}
			}
		} catch (Exception e) { print("add collision error: "+e.Message); }
		Recalculate();
		return false;
	}

	/// <summary>Removes the collision between obj1 and obj2, stops handling the collision.
	/// Returns true if it successfully removed the collision, false if it couldn't be removed or an error occurred.</summary>
	public bool RemoveCollision(PhysicsObject obj1, PhysicsObject obj2) {
		bool result;
		try {
			if (obj1.id<obj2.id)
				result = collisions[obj1].Remove(obj2);
			else
				result = collisions[obj2].Remove(obj1);
		} catch (Exception e) {
			print("remove collision error: "+e.Message);
			result = false;
		}
		Recalculate();
		return result;
	}

	public void FixedUpdate() {
		// update the objects
		for (int i=0; i<objs.Count; ++i) {
			// remove destroyed objects
			if (objs[i]==null) {
				RemoveObject(objs[i--]);
				continue;
			}
			// update objects
			objs[i].PhysicsUpdate();
		}
	}
	
	/// <summary>Recalculates data structures</summary>
	public void Recalculate() {
		visitedNum += 2;
		gearSets.Clear();

		// handle collisions
		// loop over collision lists
		foreach (PhysicsObject obj1 in collisions.Keys) {
			// continue if this has already been handled
			if (IsVisited(obj1))
				continue;

			// make a disjoint set of colliding objects
			List<PhysicsObject> gearSet=new List<PhysicsObject>(new PhysicsObject[]{obj1});
			if (!obj1.isMovable)
				tovisit.Enqueue(obj1);
			while (tovisit.Count>0) {
				// dequeue and handle the next object
				PhysicsObject cur = tovisit.Dequeue();
				List<PhysicsObject> list = collisions[cur];
				for (int i=0; i<list.Count; ++i) {
					// enqueue if not visited and not movable
					if (!IsVisited(list[i]) && !list[i].isMovable) {
						tovisit.Enqueue(list[i]);
						list[i].isPositive = !cur.isPositive;
					}
				}
			}

			// add the collision set to gearSets
			gearSets.Add(gearSet);

			// handle the collision
			// use http://web.mit.edu/8.01t/www/materials/modules/chapter21.pdf page 27 to handle collisions
		}
	}

	// helper functions
	private bool IsVisited(PhysicsObject obj) { return obj.visited>visitedNum; }
	private void VisitObj(PhysicsObject obj, bool isPositive) {
		if        ( isPositive && !obj.isMovable) {

		} else if ( isPositive &&  obj.isMovable) {

		} else if (!isPositive && !obj.isMovable) {

		} else if (!isPositive &&  obj.isMovable) {

		}
	}*/
}
