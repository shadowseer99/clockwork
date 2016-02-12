#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gear:CollidingObject {
	public bool isGolden=false;
	private float goldenRotation=0;

	public override void PhysicsUpdate() {
		// handle goldenGear
		if (isGolden) {
			goldenRotation += Time.fixedDeltaTime*curAngularVelocity;
			if (Mathf.Abs(goldenRotation) > 360) {
				GameObject.FindObjectOfType<Pause>().EndLevel();
				return;
			}
			//Material mat;
			//DynamicGI.SetEmissive(GetComponent<Renderer>(), 
		}

		// rotate/move and apply torques
		base.PhysicsUpdate();
		/*curAngularVelocity -= Time.fixedDeltaTime*Mathf.Abs(angularAcceleration)*curAngularVelocity/maxAngularVelocity;
		curAngularVelocity += Time.fixedDeltaTime*(isMovable?0:angularAcceleration);

		// average out angular speed of neighbors
		for (int i=0; i<neighbors.Count; ++i)
		{
			// sum angularMomentum, distribute according to moment of inertia
			float totalAngularMomentum = angularMomentum - neighbors[i].angularMomentum;
			float totalMomentOfInertia = momentOfInertia + neighbors[i].momentOfInertia;
			angularMomentum = totalAngularMomentum*momentOfInertia/totalMomentOfInertia;
			neighbors[i].angularMomentum = -totalAngularMomentum*neighbors[i].momentOfInertia/totalMomentOfInertia;
		}*/
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(Gear))]
public class GearEditor:Editor {
	public enum MassType { mass, density, momentOfInertia }
	public static MassType massType;
	public override void OnInspectorGUI() {
		Gear obj = (Gear)target;
		EditorGUILayout.BeginHorizontal();
		massType = (MassType)EditorGUILayout.EnumPopup(massType);
		if (massType==MassType.mass)
			obj.mass = EditorGUILayout.FloatField(obj.mass);
		else if (massType==MassType.density)
			obj.density = EditorGUILayout.FloatField(obj.density);
		else if (massType==MassType.momentOfInertia)
			obj.momentOfInertia = EditorGUILayout.FloatField(obj.momentOfInertia);
		EditorGUILayout.EndHorizontal();
		base.OnInspectorGUI();
	}
}
#endif
