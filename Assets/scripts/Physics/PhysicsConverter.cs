#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode]
public class PhysicsConverter : MonoBehaviour {
	public Gear gearPrefab;
	public Gear goldenGearPrefab;
	public GameObject wallPrefab;
	public DeathZone2D deathZonePrefab;
	public Boundary2D boundaryPrefab;
	public Water waterPrefab;
	public GameObject buttonPrefab;

	//private Dictionary<Type, GameObject>
	
	public void Start () {
		/*print("this: "+PrefabUtility.GetPrefabParent(gameObject));
		print("gearPrefab: "+PrefabUtility.GetPrefabParent(gearPrefab));
		print("wallPrefab: "+PrefabUtility.GetPrefabParent(wallPrefab));
		print("deathZonePrefab: "+PrefabUtility.GetPrefabParent(deathZonePrefab));
		print("boundaryPrefab: "+PrefabUtility.GetPrefabParent(boundaryPrefab));
		print("waterPrefab: "+PrefabUtility.GetPrefabParent(waterPrefab));
		print("buttonPrefab: "+PrefabUtility.GetPrefabParent(buttonPrefab));
		if (UnityEngine.Random.Range(0, 10)!=100)
			return;*/

		EnviroGear[] gears = GameObject.FindObjectsOfType<EnviroGear>();
		foreach (EnviroGear oldGear in gears) {
			// instantiate, copy transform vars
			Gear newGear = Instantiate<Gear>(oldGear.isGolden?goldenGearPrefab:gearPrefab);
			CopyTransform(newGear.gameObject, oldGear.gameObject);
			CopyColliders(newGear.gameObject, oldGear.gameObject);

			// newGear specific actions
			newGear.Start();
			newGear.mass = oldGear.mass;
			newGear.isMovable = oldGear.isMovable;
			newGear.curAngularVelocity = oldGear.angularAcceleration;
			newGear.accel = -newGear.AngularVelocityToCurSpeed();
			newGear.curAngularVelocity = oldGear.maxAngularVelocity;
			newGear.maxSpeed = -newGear.AngularVelocityToCurSpeed();
			newGear.curAngularVelocity = oldGear.curAngularVelocity;
			newGear.curSpeed = -newGear.AngularVelocityToCurSpeed();
		}

		DeathZone[] deathZones = GameObject.FindObjectsOfType<DeathZone>();
		foreach (DeathZone oldDeathZone in deathZones) {
			// instantiate, copy transform vars
			DeathZone2D newDeathZone = Instantiate<DeathZone2D>(deathZonePrefab);
			CopyTransform(newDeathZone.gameObject, oldDeathZone.gameObject);
		}
		Boundary2D[] boundaries = GameObject.FindObjectsOfType<Boundary2D>();
		foreach (Boundary2D oldBoundary in boundaries) {
			// instantiate, copy transform vars
			Boundary2D newBoundary = Instantiate<Boundary2D>(boundaryPrefab);
			CopyTransform(newBoundary.gameObject, oldBoundary.gameObject);
			CopyComponent(newBoundary.gameObject, oldBoundary.gameObject, typeof(Boundary), typeof(Boundary2D));
		}
		Water[] waters = GameObject.FindObjectsOfType<Water>();
		foreach (Water oldWater in waters) {
			// instantiate, copy transform vars
			Water newWater = Instantiate<Water>(waterPrefab);
			CopyTransform(newWater.gameObject, oldWater.gameObject);
			CopyComponent(newWater.gameObject, oldWater.gameObject, typeof(Water), typeof(Water));
		}
		Button2D[] buttons = GameObject.FindObjectsOfType<Button2D>();
		foreach (Button2D oldButton in buttons) {
			// instantiate, copy transform vars
			GameObject newButton = Instantiate<GameObject>(buttonPrefab);
			CopyTransform(newButton.gameObject, oldButton.gameObject);
			CopyComponent(newButton.gameObject, oldButton.gameObject, typeof(Button), typeof(Button2D));
		}
		/*BoxCollider[] walls = GameObject.FindObjectsOfType<BoxCollider>();
		foreach (BoxCollider oldWall in walls) {
			// instantiate, copy transform vars
			GameObject newWall = Instantiate<GameObject>(wallPrefab);
			CopyTransform(newWall.gameObject, oldWall.gameObject);
		}*/
	}

	public void CopyTransform(GameObject newObj, GameObject oldObj) {
		newObj.transform.position = oldObj.transform.position;
		newObj.transform.rotation = oldObj.transform.rotation;
		newObj.transform.localScale = oldObj.transform.localScale;
	}
	public void CopyColliders(GameObject newObj, GameObject oldObj) {
		Collider2D[] newColls = newObj.GetComponents<Collider2D>();
		Collider[] oldColls = oldObj.GetComponents<Collider>();
		Collider2D newTrig=null, newColl=null;
		for (int i=0; i<newColls.Length; ++i) {
			if (newColls[i].isTrigger)
				newTrig = newColls[i];
			else
				newColl = newColls[i];
		}

		foreach (Collider oldColl in oldColls) {
			Collider2D curColl = (oldColl.isTrigger?newTrig:newColl);
			if (curColl.GetType()==typeof(CircleCollider2D) && oldColl.GetType()==typeof(SphereCollider)) {				
				curColl.offset = (oldColl as SphereCollider).center;
				(curColl as CircleCollider2D).radius = (oldColl as SphereCollider).radius;
			} else if (curColl.GetType()==typeof(BoxCollider2D) && oldColl.GetType()==typeof(BoxCollider)) {
				curColl.offset = (oldColl as BoxCollider).center;
				(curColl as BoxCollider2D).size = (oldColl as BoxCollider).size;
			} else {
				throw new NotImplementedException("Unsupported Collider is used");
			}
		}
	}
	public void CopyComponent(GameObject newObj, GameObject oldObj, Type original, Type newType) {
		Component oldComp = oldObj.GetComponent(original);
		Component newComp = newObj.AddComponent(newType);
		System.Reflection.FieldInfo[] fields = newType.GetFields();
		for (int i=0; i<fields.Length; ++i)
			fields[i].SetValue(newComp, fields[i].GetValue(oldComp));
		/*Component copy = destination.AddComponent(type);
		// Copied fields can be restricted with BindingFlags
		System.Reflection.FieldInfo[] fields = type.GetFields(); 
		foreach (System.Reflection.FieldInfo field in fields)
		{
			field.SetValue(copy, field.GetValue(original));
		}*/
	}

	//public void Test<T>(T obj) {}
	//public void CopyToNewPrefab(MonoBehaviour obj) {}
}
