#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gear:CollidingObject {
	public bool isGolden=false;
	private float goldenRotation=0;
	
	public AudioClip _move;
	public AudioClip _insert;
	public AudioClip _playerHit;

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
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(Gear))]
public class GearEditor:CollidingObjectEditor {}
#endif