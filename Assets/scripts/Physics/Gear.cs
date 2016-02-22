#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gear:CollidingObject {
	public bool isGolden=false;
	private float goldenRotation=0;

	public override void Start() {
		base.Start();

		if (isGolden && Application.isPlaying) {
			// initialize vars
			Mesh mesh = GetComponentInChildren<MeshFilter>().mesh;
			Vector3[] verts = mesh.vertices;
			Vector2[] uvs = mesh.uv;

			// find max and min vertices->uvs
			Vector2 max=Vector2.one*-1000, min=Vector2.one*1000;
			for (int i=0; i<verts.Length; ++i) {
				max = new Vector2(Mathf.Max(max.x, verts[i].x), Mathf.Max(max.y, verts[i].y));
				min = new Vector2(Mathf.Min(min.x, verts[i].x), Mathf.Min(min.y, verts[i].y));
			}
			Vector2 diff = max - min;

			// update uvs based off of vertices
			for (int i=0; i<verts.Length; ++i) {
				for (int j=0; j<2; ++j) {
					uvs[i][j] = (verts[i][j]-min[j])/diff[j];
					//print("set uv to "+100*uvs[i][j]);
				}
			}
			mesh.uv = uvs;
		}
	}

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
