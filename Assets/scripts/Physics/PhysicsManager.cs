﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PhysicsManager : MonoBehaviour {
	// core data structures
	private List<PhysicsObject> objs=new List<PhysicsObject>();
	private List<CollidingObject> colls=new List<CollidingObject>();

	// helper data structures/variables
	private static uint visitedNum;
	private static uint POSITIVE { get { return visitedNum+2; } }
	private static uint NEGATIVE { get { return visitedNum+1; } }
	private List<GearSet> gearSets=new List<GearSet>();

	private static PhysicsManager _physicsManager;
	public static PhysicsManager physicsManager {
		get {
			// if null, create new PhysicsManager
			if (_physicsManager==null) {
				_physicsManager = new GameObject("Physics Manager").AddComponent<PhysicsManager>();
			}
			return _physicsManager;
		}
	}

	public void Start() {
		if (_physicsManager==null)
			_physicsManager = this;
		// populate
		PhysicsObject[] objs = GameObject.FindObjectsOfType<PhysicsObject>();
		for (int i=0; i<objs.Length; ++i)
			physicsManager.AddObject(objs[i]);
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
		// reset gearSets, rebuild gearSets
		for (int i=0; i<gearSets.Count; ++i)
			gearSets[i].Reset();
		int curIndex=0;
		for (int i=0; i<colls.Count; ++i) {
			// adjust gearSets, where necessary
			if (curIndex>=gearSets.Count)
				gearSets.Add(new GearSet(curIndex));
			// try adding to gearSets
			if (gearSets[curIndex].TryAddObj(colls[i], null))
				++curIndex;
		}
		// find contacts
		visitedNum += 2;
		// loop over neighbors of movable CollidingObjects
		foreach (GearSet movableGS in gearSets) {
			if (movableGS.isMovable) {
				//print("neighbor count: "+gearSet.gears[0].neighbors.Count);
				foreach (CollidingObject neighbor in movableGS.gears[0].neighbors) {
					// register movable CollidingObjects to their immovable neighbors
					if (!neighbor.isMovable) {
						if (!neighbor.gearSet.contacts.ContainsKey(movableGS))
							neighbor.gearSet.contacts[movableGS] = new GearSetRelation(movableGS);
						neighbor.gearSet.contacts[movableGS].contactPoints.Add(neighbor);
						print(neighbor.gameObject.name+" is touching "+movableGS.gears[0].gameObject.name);
					}
				}
			}
		}
		// update gearSets with their contacts
		foreach (GearSet gearSet in gearSets) {
			foreach (GearSet movableGS in gearSet.contacts.Keys) {
				if (gearSet.contacts[movableGS].contactPoints.Count==1) {
					CollidingObject other = gearSet.contacts[movableGS].contactPoints[0];
				} else {
					throw new NotImplementedException("multiple contact points isn't yet implemented");
				}
			}
		}
			/*// add torque for acceleration AND gravity
			if (coll.attachedTo!=null)
				coll.attachedTo.angularMomentum += coll.mass*Time.fixedDeltaTime*coll.accel*(coll.accelMult-coll.curSpeed/coll.maxSpeed)
					- Physics.gravity.y*coll.mass*Vector3.Dot(Vector3.right, coll.transform.position-coll.attachedTo.transform.position);*/

		// update PhysicsObjects
		for (int i=0; i<objs.Count; ++i)
			objs[i].PhysicsUpdate();
	}

	// helper functions
	private static bool IsVisited(CollidingObject obj) { return obj.visited>visitedNum; }
	private static float Mult(CollidingObject obj) { return (float)(obj.visited*2.0-POSITIVE-NEGATIVE); }

	/// <summary>Manages a collection of interacting gears</summary>
	public class GearSet {
		public List<CollidingObject> gears=new List<CollidingObject>();
		public Dictionary<GearSet, GearSetRelation> contacts=new Dictionary<GearSet, GearSetRelation>();
		public float totalAngularMomentum=0;
		public float totalMomentOfInertia=0;
		public int index=-1;
		public bool isMovable=false;

		public GearSet(int index=-1) {
			Reset(index);
		}
		public void Reset(int index=-1) {
			gears.Clear();
			contacts.Clear();
			isMovable = false;
			if (index>=0)
				this.index = index;
		}
		// adds obj to this GearSet if possible, returns true if possible
		public bool TryAddObj(CollidingObject obj, CollidingObject oldObj) {
			// add if not visited and appropriate
			if (!IsVisited(obj) && (!obj.isMovable||gears.Count==0) && !isMovable) {
				// adjust data
				obj.visited = NEGATIVE;
				if (oldObj==null || oldObj.visited==NEGATIVE)
					obj.visited = POSITIVE;
				obj.gearSet = this;
				gears.Add(obj);
				totalAngularMomentum += Mult(obj)*obj.angularMomentum;
				totalMomentOfInertia += Mult(obj)*obj.momentOfInertia;
				isMovable = obj.isMovable;

				// try adding neighbors
				for (int i=0; i<obj.neighbors.Count; ++i)
					TryAddObj(obj.neighbors[i], obj);
				return true;
			}
			return false;
		}
		// prints objects added to this
		public override string ToString() {
			string result = index+"("+gears.Count+")"+": ";
			for (int i=0; i<gears.Count; ++i)
				result += Mult(gears[i])+"*"+gears[i].gameObject.name+", ";
			return result;
		}
	}

	/// <summary>Tracks relations with other gears.</summary>
	public class GearSetRelation {
		public GearSet other;
		public List<CollidingObject> contactPoints=new List<CollidingObject>();
		public GearSetRelation(GearSet other) {
			this.other = other;
		}
	}
}
