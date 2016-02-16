#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;

[ExecuteInEditMode]
public class PhysicsConverter : MonoBehaviour {
	// input prefabs
	public GameObject gearPrefab;
	public GameObject goldenGearPrefab;
	public GameObject deathZonePrefab;
	public GameObject boundaryPrefab;
	public GameObject buttonPrefab;
	public List<PrefabConverter> conversions;

	// helper vars
	private Dictionary<GameObject, GameObject> prefabs=new Dictionary<GameObject,GameObject>();
	private Dictionary<GameObject, GameObject> replacements=new Dictionary<GameObject, GameObject>();
	private Dictionary<GameObject, Type> options2=new Dictionary<GameObject, Type>();
	private Dictionary<GameObject, PropertyModification[]> prefabOptions=new Dictionary<GameObject, PropertyModification[]>();
	
	public void Start () {
		// convert generic objects
		CopyObjects<EnviroGear, Gear>(
			x=>(x.isGolden?goldenGearPrefab:gearPrefab),
			(oldGear, newGear)=>{
			// newGear specific actions
			newGear.isGolden = oldGear.isGolden;
			newGear.Start();
			newGear.mass = oldGear.mass;
			newGear.isMovable = oldGear.isMovable;
			newGear.curAngularVelocity = oldGear.angularAcceleration;
			newGear.accel = -newGear.AngularVelocityToCurSpeed();
			newGear.curAngularVelocity = oldGear.maxAngularVelocity;
			newGear.maxSpeed = -newGear.AngularVelocityToCurSpeed();
			newGear.curAngularVelocity = oldGear.curAngularVelocity;
			newGear.curSpeed = -newGear.AngularVelocityToCurSpeed();
		});
		CopyObjects<DeathZone, DeathZone2D>(x=>deathZonePrefab);
		CopyObjects<Boundary, Boundary2D>(x=>boundaryPrefab);
		CopyObjects<Button, Button2D>(x=>buttonPrefab);

		// prepare to convert prefabs, track replacements
		for (int i=0; i<conversions.Count; ++i)
			prefabs[conversions[i].oldPrefab] = conversions[i].newPrefab;
		// loop over prefabs
		GameObject[] objs = GameObject.FindObjectsOfType<GameObject>();
		for (int i=0; i<objs.Length; ++i) {
			// check if objs[i] is the prefab root of a prefab to replace
			GameObject parent = PrefabUtility.GetPrefabParent(objs[i]) as GameObject;
			if (PrefabUtility.FindPrefabRoot(objs[i])==objs[i] && parent!=null && prefabs.ContainsKey(parent)) {
				// create replacement, store changes
				GameObject replacement = PrefabUtility.InstantiatePrefab(prefabs[parent]) as GameObject;
				prefabOptions[objs[i]] = PrefabUtility.GetPropertyModifications(objs[i]);
				replacements[objs[i]] = replacement;
				CopyTransform(objs[i], replacement);
				CopyColliders(objs[i], replacement);
				print("replaced "+objs[i].name+" with "+replacement.name);
			}
		}

		// change instances of replacements
		foreach (KeyValuePair<GameObject, Type> entry in options2) {
			// search all fields for GameObjects and Transforms
			Component comp = entry.Key.GetComponent(entry.Value);
			FieldInfo[] fields = entry.Value.GetFields();
			for (int i=0; i<fields.Length; ++i) {
				// if it's a GameObject that's being replaced, find the appropriate variable to change it to
				Transform t = fields[i].GetValue(comp) as Transform;
				GameObject obj = (t!=null?t.gameObject:fields[i].GetValue(comp) as GameObject);
				GameObject cur = obj;
				Stack<string> path = new Stack<string>();
				while (cur!=null && cur.transform.parent!=null && cur!=PrefabUtility.FindPrefabRoot(cur)) {
					path.Push(cur.name);
					cur = cur.transform.parent.gameObject;
				}
				if (cur && replacements.ContainsKey(cur)) {
					Transform target = replacements[cur].transform;
					while (path.Count>0 && target!=null)
						target = target.transform.Find(path.Pop());
					fields[i].SetValue(comp, (t==null?target.gameObject as object:target as object));
				}
			}
		}
		foreach (KeyValuePair<GameObject, PropertyModification[]> entry in prefabOptions) {
			foreach (PropertyModification propMod in entry.Value) {
				//if (propMod.target
			}
		}
		// delete old objects
		GameObject destroyMe = new GameObject("GARBAGE (DESTROY ME)");
		destroyMe.SetActive(false);
		foreach (GameObject oldObj in replacements.Keys)
			oldObj.transform.parent = destroyMe.transform;
	}

	public void CopyObjects<OldType, NewType>(Func<OldType, GameObject> prefab, Action<OldType, NewType> customCopy=null) where OldType:Component where NewType:Component {
		// loop over <OldType> GameObjects
		OldType[] objs = GameObject.FindObjectsOfType<OldType>();
		foreach (OldType oldObj in objs) {
			if (prefab(oldObj)==null)
				continue;
			// create new prefab, copy transform/colliders, copy <OldType> to <NewType>
			GameObject newObj = PrefabUtility.InstantiatePrefab(prefab(oldObj)) as GameObject;
			CopyTransform(oldObj.gameObject, newObj);
			CopyColliders(oldObj.gameObject, newObj);
			NewType newComp = newObj.GetComponent<NewType>() ?? newObj.AddComponent<NewType>();
			if (customCopy==null)
				CopyComponent(oldObj.gameObject, newObj, typeof(OldType), typeof(NewType));
			else
				customCopy(oldObj, newComp);
			// store results
			options2[newObj] = typeof(NewType);
			replacements[oldObj.gameObject] = newObj;
		}
	}
	public void CopyTransform(GameObject oldObj, GameObject newObj) {
		// copy position, rotation, scale
		newObj.transform.position = oldObj.transform.position;
		//newObj.transform.rotation = oldObj.transform.rotation;
		newObj.transform.localScale = oldObj.transform.localScale;
		// copy parent and children (when they belong to another prefab)
		newObj.transform.parent = oldObj.transform.parent;
		for (int i=oldObj.transform.childCount-1; i>=0; --i) {
			if (PrefabUtility.FindPrefabRoot(oldObj.transform.GetChild(i).gameObject)!=oldObj) {
				oldObj.transform.GetChild(i).parent = newObj.transform;
			}
		}
	}
	public void CopyColliders(GameObject oldObj, GameObject newObj) {
		// initialize vars
		Collider2D[] newColls = newObj.GetComponents<Collider2D>();
		Collider[] oldColls = oldObj.GetComponents<Collider>();
		Collider2D newTrig=null, newColl=null;
		for (int i=0; i<newColls.Length; ++i) {
			if (newColls[i].isTrigger)
				newTrig = newColls[i];
			else
				newColl = newColls[i];
		}
		// loop over old colliders, changing values where necessary
		foreach (Collider oldColl in oldColls) {
			Collider2D curColl = (oldColl.isTrigger?newTrig:newColl);
			if (curColl.GetType()==typeof(CircleCollider2D) && oldColl.GetType()==typeof(SphereCollider)) {
				curColl.offset = (oldColl as SphereCollider).center;
				(curColl as CircleCollider2D).radius = (oldColl as SphereCollider).radius;
			} else if (curColl.GetType()==typeof(BoxCollider2D) && oldColl.GetType()==typeof(BoxCollider)) {
				curColl.offset = (oldColl as BoxCollider).center;
				(curColl as BoxCollider2D).size = (oldColl as BoxCollider).size;
			} else {
				throw new NotImplementedException("Unsupported Collider combination is used");
			}
		}
	}
	/// <summary>Copies FieldInfo from oldObj.GetComponent(oldType) to newObj.GetComponent(newType)</summary>
	public void CopyComponent(GameObject oldObj, GameObject newObj, Type oldType, Type newType) {
		Component oldComp = oldObj.GetComponent(oldType);
		Component newComp = newObj.GetComponent(newType);
		FieldInfo[] oldFields = oldType.GetFields();
		FieldInfo[] newFields = newType.GetFields();
		// loop over fields, handle for general objects and Enums
		for (int i=0; i<newFields.Length; ++i) {
			if (newFields[i].GetValue(newComp) is Enum)
				newFields[i].SetValue(newComp, (int)oldFields[i].GetValue(oldComp));
			else
				newFields[i].SetValue(newComp, oldFields[i].GetValue(oldComp));
		}
	}

	[Serializable]
	public class PrefabConverter {
		public GameObject oldPrefab;
		public GameObject newPrefab;
	}
	[CustomPropertyDrawer(typeof(PrefabConverter))]
	public class Point3Drawer : PropertyDrawer {
		public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(pos, label, property);
			pos = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Passive), label);
			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			float w=pos.width/2;
			EditorGUI.PropertyField(new Rect(pos.x  , pos.y, w, pos.height), property.FindPropertyRelative("oldPrefab"), GUIContent.none);
			EditorGUI.PropertyField(new Rect(pos.x+w, pos.y, w, pos.height), property.FindPropertyRelative("newPrefab"), GUIContent.none);
		
			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty();
		}
	}
}
#endif
