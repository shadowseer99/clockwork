#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityStandardAssets._2D;

[ExecuteInEditMode]
public class PhysicsConverter : MonoBehaviour {
	// input prefabs
	public GearSelector gearSelector;
	public float scaleFactor=8;
	public GameObject playerGearPrefab;
	public GameObject deathZonePrefab;
	public GameObject boundaryPrefab;
	public GameObject buttonPrefab;
	public List<PrefabConverter> conversions;

	// helper vars
	private Dictionary<GameObject, GameObject> prefabs=new Dictionary<GameObject, GameObject>();
	private Dictionary<GameObject, GameObject> replacements=new Dictionary<GameObject, GameObject>();
	private HashSet<GameObject> visited=new HashSet<GameObject>();
	private GameObject destroyMe;
	
	public void Convert () {
		// initialize destroyMe
		destroyMe = new GameObject("GARBAGE (DESTROY ME)");
		destroyMe.SetActive(false);
		transform.parent = destroyMe.transform;

		// convert generic objects
		CopyObjects<EnviroGear, Gear>(
			x=>{
				SphereCollider coll = x.GetComponent<SphereCollider>();
				float max = Mathf.Max(
					Mathf.Abs(x.transform.TransformVector(coll.radius*Vector3.right).magnitude),
					Mathf.Abs(x.transform.TransformVector(coll.radius*Vector3.up).magnitude),
					Mathf.Abs(x.transform.TransformVector(coll.radius*Vector3.forward).magnitude));
				GameObject result = (coll.isTrigger
					?gearSelector.GetGearByTrigSize(max/scaleFactor, x.isGolden)
					:gearSelector.GetGearByCollSize(max/scaleFactor, x.isGolden)).gameObject;
				result.transform.localScale = result.transform.localScale*scaleFactor;
				return result;
			},
			(oldGear, newGear)=>{
				// newGear specific actions
				newGear.isGolden = oldGear.isGolden;
				newGear.Start();
				newGear.mass = oldGear.mass;
				newGear.isMovable = oldGear.isMovable;
				newGear.curAngularVelocity = oldGear.angularAcceleration;
				newGear.accel = newGear.AngularVelocityToCurSpeed();
				newGear.curAngularVelocity = oldGear.maxAngularVelocity;
				newGear.maxSpeed = -newGear.AngularVelocityToCurSpeed();
				newGear.curAngularVelocity = oldGear.curAngularVelocity;
				newGear.curSpeed = -newGear.AngularVelocityToCurSpeed();
				CopyTransform(oldGear.gameObject, newGear.gameObject, false);
			});
		CopyObjects<GearGuyCtrl1, PlayerGear>(
			(x)=>PrefabUtility.InstantiatePrefab(playerGearPrefab) as GameObject,
			(oldPlayer, newPlayer)=>{
				newPlayer.Start();
				newPlayer.mass = oldPlayer.mass;
				newPlayer.isMovable = true;
				newPlayer.isRotatable = true;
				newPlayer.accel = oldPlayer.acceleration;
				newPlayer.maxSpeed = oldPlayer.maxSpeed;
				CopyTransform(oldPlayer.gameObject, newPlayer.gameObject, false);
			});
		CopyObjects<DeathZone, DeathZone2D>(x=>PrefabUtility.InstantiatePrefab(deathZonePrefab) as GameObject);
		CopyObjects<Boundary, Boundary2D>(x=>PrefabUtility.InstantiatePrefab(boundaryPrefab) as GameObject);
		CopyObjects<Button, Button2D>(x=>PrefabUtility.InstantiatePrefab(buttonPrefab) as GameObject);

		// prepare to convert prefabs, track replacements
		for (int i=0; i<conversions.Count; ++i)
			prefabs[conversions[i].oldPrefab] = conversions[i].newPrefab;
		// loop over prefabs
		GameObject[] objs = GameObject.FindObjectsOfType<GameObject>();
		for (int i=0; i<objs.Length; ++i) {
			try {
				// check if objs[i] is the prefab root of a prefab to replace
				GameObject parent = PrefabUtility.GetPrefabParent(objs[i]) as GameObject;
				if (PrefabUtility.FindPrefabRoot(objs[i])==objs[i] && parent!=null && prefabs.ContainsKey(parent)) {
					// create replacement, store changes
					GameObject replacement = PrefabUtility.InstantiatePrefab(prefabs[parent]) as GameObject;
					replacement.name = objs[i].name;
					PrefabUtility.SetPropertyModifications(replacement, PrefabUtility.GetPropertyModifications(objs[i]));
					CopyTransform(objs[i], replacement);
					CopyColliders(objs[i], replacement);
					objs[i].transform.parent = destroyMe.transform;
					replacements[objs[i]] = replacement;
				}
			} catch (Exception e) {
				Debug.LogWarning("Error: failed to convert "+objs[i].name+".\n\n"+e.Message);
			}
		}

		// change instances of replacements (loop over GameObjects, Components, FieldInfos, check if GameObject, Transform, Component)
		foreach (Transform t in GameObject.FindObjectsOfType<Transform>())
			TryReplace(t.gameObject);
		Selection.activeGameObject = destroyMe;
		print("Finished converting objects");
	}

	public void TryReplace(GameObject obj) {
		// handle early termination
		if (obj.transform.root==destroyMe.transform || visited.Contains(obj))
			return;
		visited.Add(obj);

		foreach (Component comp in obj.GetComponents<Component>()) {
			foreach (FieldInfo field in comp.GetType().GetFields()) {
				try {
					// validate if the field should be changed
					object val = field.GetValue(comp);
					if (val as GameObject || val as Component) {
						// initialize, handle inactive, handle early termination
						GameObject obj2 = val as GameObject ?? (val as Component).gameObject;
						if (!visited.Contains(obj2) && obj2.transform.root.gameObject!=destroyMe)
							TryReplace(obj2);
						if (obj2.transform.root!=destroyMe.transform)
							continue;

						// find the path to the object
						GameObject cur = obj2;
						Stack<string> path = new Stack<string>();
						while (cur!=null && cur.transform.parent!=null && cur!=PrefabUtility.FindPrefabRoot(cur)) {
							path.Push(cur.name);
							cur = cur.transform.parent.gameObject;
						}

						// change to the equivalent object
						if (cur && replacements.ContainsKey(cur)) {
							Transform target = replacements[cur].transform;
							while (path.Count>0 && target!=null)
								target = target.transform.Find(path.Pop());
							PropertyModification[] mods = PrefabUtility.GetPropertyModifications(comp);
							if (mods!=null) {
								foreach (PropertyModification propMod in mods) {
									if (propMod.target==comp)
										propMod.objectReference = target;
								}
								PrefabUtility.SetPropertyModifications(comp, mods);
							}
							field.SetValue(comp, (target==null?target:(val is GameObject?target.gameObject as object:target.GetComponent(val.GetType()))));
						}
					}
				} catch(Exception e) {
					Debug.LogWarning("Error: failed to convert "+obj.name+"-"+comp.GetType()+" field "+field.Name+".\n\n"+e.Message);
				}
			}
		}
	}

	public void CopyObjects<OldType, NewType>(Func<OldType, GameObject> prefab, Action<OldType, NewType> customCopy=null) where OldType:Component where NewType:Component {
		// loop over <OldType> GameObjects
		OldType[] objs = GameObject.FindObjectsOfType<OldType>();
		foreach (OldType oldObj in objs) {
			try {
				// initialize, continue if necessary
				if (oldObj.transform.parent==destroyMe.transform)
					continue;
				GameObject newObj = prefab(oldObj);
				if (newObj==null)
					continue;
				newObj.name = oldObj.name;

				// copy components
				OldType[] oldComps = oldObj.GetComponents<OldType>();
				for (int i=0; i<oldComps.Length; ++i) {
					NewType newComp = newObj.GetComponent<NewType>();
					if (newComp==null || i!=0)
						newComp = newObj.AddComponent<NewType>();
					if (customCopy==null) {
						CopyTransform(oldComps[i].gameObject, newObj);
						CopyColliders(oldComps[i].gameObject, newObj);
						CopyComponent(oldComps[i], newComp, typeof(OldType), typeof(NewType));
					} else {
						customCopy(oldComps[i], newComp);
					}
				}

				// store results
				replacements[oldObj.gameObject] = newObj;
				oldObj.transform.parent = destroyMe.transform;
			} catch (Exception e) {
				Debug.LogWarning("Error: failed to convert "+oldObj.name+"-"+typeof(OldType)+".\n\n"+e.Message);
			}
		}
	}
	public void CopyTransform(GameObject oldObj, GameObject newObj, bool copyScaleRot=true) {
		try {
			// copy parent
			newObj.transform.parent = oldObj.transform.parent;

			// copy position, scale, rotation
			newObj.transform.localPosition = oldObj.transform.localPosition;
			if (copyScaleRot) {
				newObj.transform.rotation = oldObj.transform.rotation;
				newObj.transform.localScale = oldObj.transform.localScale;
			}

			// copy children (when they belong to another prefab)
			for (int i=oldObj.transform.childCount-1; i>=0; --i) {
				if (PrefabUtility.FindPrefabRoot(oldObj.transform.GetChild(i).gameObject)!=oldObj) {
					oldObj.transform.GetChild(i).parent = newObj.transform;
				}
			}
		} catch (Exception e) {
			Debug.LogWarning("Error: failed to copy transform from "+oldObj.name+".\n\n"+e.Message);
		}
	}
	public void CopyColliders(GameObject oldObj, GameObject newObj) {
		try {
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
		} catch (Exception e) {
			Debug.LogWarning("Error: failed to copy colliders from "+oldObj.name+".\n\n"+e.Message);
		}
	}
	/// <summary>Copies FieldInfo from oldComp to newComp</summary>
	public void CopyComponent(Component oldComp, Component newComp, Type oldType, Type newType) {
		try {
			FieldInfo[] oldFields = oldType.GetFields();
			FieldInfo[] newFields = newType.GetFields();
			// loop over fields, handle for general objects and Enums
			for (int i=0; i<newFields.Length; ++i) {
				if (newFields[i].GetValue(newComp) is Enum)
					newFields[i].SetValue(newComp, (int)oldFields[i].GetValue(oldComp));
				else
					newFields[i].SetValue(newComp, oldFields[i].GetValue(oldComp));
			}
		} catch (Exception e) {
			Debug.LogWarning("Error: failed to copy "+oldComp.GetType()+" from "+oldComp.gameObject.name+".\n\n"+e.Message);
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

[CustomEditor(typeof(PhysicsConverter))]
[CanEditMultipleObjects]
public class PhysicsConverterEditor:Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		if (GUILayout.Button("Convert"))
			(target as PhysicsConverter).Convert();
		//if (GUILayout.Button("Reload Scene"))
			//Application.LoadLevel(Application.loadedLevelName);
	}
}

#endif
