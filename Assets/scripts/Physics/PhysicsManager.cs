using UnityEngine;
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
			// if null, find or create new PhysicsManager
			if (_physicsManager==null) {
				_physicsManager = GameObject.FindObjectOfType<PhysicsManager>()
					?? new GameObject("Physics Manager").AddComponent<PhysicsManager>();
			}
			return _physicsManager;
		}
	}

	public void Start() {
		// populate if empty
		if (this.objs.Count==0) {
			PhysicsObject[] objs = GameObject.FindObjectsOfType<PhysicsObject>();
			for (int i=0; i<objs.Length; ++i)
				physicsManager.AddObject(objs[i]);
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

		// rebuild gearSets
		for (int i=0; i<gearSets.Count; ++i)
			gearSets[i].Reset();
		int curIndex=0;
		for (int i=0; i<colls.Count; ++i) {
			if (curIndex>=gearSets.Count)
				gearSets.Add(new GearSet(curIndex));
			if (gearSets[curIndex].TryAddObj(colls[i], null))
				++curIndex;
		}
		while (gearSets.Count>=curIndex+1)
			gearSets.RemoveAt(gearSets.Count-1);

		// update gearSets
		for (int i=0; i<gearSets.Count; ++i)
			gearSets[i].AnalyzeRelations();
		for (int i=0; i<gearSets.Count; ++i)
			gearSets[i].UpdateObjects();
		// move PhysicsObjects
		for (int i=0; i<objs.Count; ++i)
			objs[i].PhysicsUpdate();
		for (int i=0; i<objs.Count; ++i)
			objs[i].Move();
	}

	// helper functions
	private static bool IsVisited(CollidingObject obj) { return obj!=null && obj.visited>visitedNum; }
	private static float Mult(CollidingObject obj) { return (float)(obj.visited*2.0-POSITIVE-NEGATIVE); }

	/// <summary>Manages a collection of interacting gears</summary>
	public class GearSet {
		public List<CollidingObject> gears=new List<CollidingObject>();
		public List<GearSetRelation> relations=new List<GearSetRelation>();
		public float totalAngularMomentum=0;
		public float totalMomentOfInertia=0;
		public int index=-1;
		public bool isMovable=false;

		public GearSet(int index=-1) {
			Reset(index);
		}
		public void Reset(int index=-1) {
			gears.Clear();
			relations.Clear();
			totalAngularMomentum = totalMomentOfInertia = 0;
			isMovable = false;
			if (index>=0)
				this.index = index;
		}
		// adds obj to this GearSet if possible, returns true if possible
		public bool TryAddObj(CollidingObject obj, CollidingObject oldObj) {
			// add relations if both objects have gearSets
			if (IsVisited(obj) && IsVisited(oldObj)) {
				if (obj.attachedTo==oldObj) oldObj.gearSet.relations.Add(new GearSetRelation(oldObj, obj, true));
				if (oldObj.attachedTo==obj)    obj.gearSet.relations.Add(new GearSetRelation(obj, oldObj, true));
				if (obj.isMovable) oldObj.gearSet.relations.Add(new GearSetRelation(oldObj, obj, false));
				if (oldObj.isMovable) obj.gearSet.relations.Add(new GearSetRelation(obj, oldObj, false));
			}

			// add if not visited and appropriate
			if (!IsVisited(obj) && ((!obj.isMovable&&obj.attachedTo==null)||gears.Count==0) && !isMovable) {
				// adjust data
				obj.visited = NEGATIVE;
				if (oldObj==null || oldObj.visited==NEGATIVE)
					obj.visited = POSITIVE;
				obj.gearSet = this;
				gears.Add(obj);
				totalAngularMomentum += Mult(obj)*obj.angularMomentum;
				totalMomentOfInertia += obj.momentOfInertia;
				isMovable = obj.isMovable || obj.attachedTo!=null;

				// try adding neighbors
				for (int i=0; i<obj.neighbors.Count; ++i)
					TryAddObj(obj.neighbors[i], obj);
				return true;
			}
			return false;
		}

		public void AnalyzeRelations() {
			for (int i=0; i<relations.Count; ++i) {
				// initialize vars
				CollidingObject obj = relations[i].obj;
				CollidingObject other = relations[i].other;
				float mult = Mult(obj)*Mult(other);
				Vector3 diff = other.transform.position-obj.transform.position;

				if (relations[i].isAttached) {
					// adds other's acceleration, gravity, and momentum
					totalAngularMomentum += (mult*other.mass*180/Mathf.PI)*(other.collRadius*other.Accel()
						+ Time.fixedDeltaTime*Vector3.Dot(Vector3.Cross(diff.normalized, Physics.gravity), Vector3.forward))
						+ other.mass*mult*diff.sqrMagnitude*obj.curAngularVelocity;
					totalMomentOfInertia += diff.sqrMagnitude*other.mass;
				} else {
					// apply 1/3 of other's angular momentum, apply full momentOfInertia
					totalAngularMomentum += (mult/3)* (other.angularMomentum + other.momentum.magnitude
						*Vector3.Dot(Vector3.Cross(other.velocity.normalized, diff.normalized), Vector3.forward)*diff.magnitude);
					totalMomentOfInertia += other.momentOfInertia;
				}
			}
		}

		public void UpdateObjects() {
			// update gears
			for (int i=0; i<gears.Count; ++i) {
				gears[i].angularMomentum = Mult(gears[i])*totalAngularMomentum*(gears[i].momentOfInertia/totalMomentOfInertia);
				gears[i].curSpeed = gears[i].AngularVelocityToCurSpeed();
			}

			// update relations
			for (int i=0; i<relations.Count; ++i) {
				/*if (!relations[i].isAttached) {
					CollidingObject obj = relations[i].obj;
					CollidingObject other = relations[i].other;
					float mult = -Mult(obj)*Mult(other);
//print("applying to "+other.name+" ("+mult*(totalAngularMomentum/totalMomentOfInertia)+")");
					other.angularMomentum = mult*(totalAngularMomentum*other.momentOfInertia/totalMomentOfInertia)*(1f/3f);
					other.velocity = obj.GetVelAtPoint(other.transform.position);
				}*/
			}
		}

		// prints objects added to this
		public override string ToString() {
			string result = index+"("+gears.Count+")"+": ";
			for (int i=0; i<gears.Count; ++i)
				result += Mult(gears[i])+"*"+gears[i].gameObject.name+", ";
			return result;
		}
	}

	public class GearSetRelation {
		public CollidingObject obj;
		public CollidingObject other;
		public bool isAttached;

		public GearSetRelation(CollidingObject obj, CollidingObject other, bool isAttached) {
			this.obj = obj;
			this.other = other;
			this.isAttached = isAttached;
		}
	}
}
