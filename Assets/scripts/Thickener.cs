using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Thickener:MonoBehaviour {
	public float extraThickness=1;

	public void Start() {
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		for (int i=0; i<meshFilters.Length; ++i) {
			// initialize vars
			Mesh m = meshFilters[i].mesh;
			Transform t = meshFilters[i].transform;
			Vector3[] verts = m.vertices;

			// find average z value
			float avgZ=0;
			for (int j=0; j<verts.Length; ++j)
				avgZ += verts[j].z;
			avgZ /= verts.Length;

			// thicken vertices
			for (int j=0; j<verts.Length; ++j) {
				verts[j] = Vector3.Scale(verts[j], Vector3.one+extraThickness*t.TransformDirection(Vector3.forward))
					- extraThickness*avgZ*t.TransformDirection(Vector3.forward);
			}
			m.vertices = verts;
		}
	}
}