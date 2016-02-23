using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Rotator2D : MonoBehaviour {
	public bool fixChildren=true;

	public void Start() {
		FixObjs(transform);
	}

	/// <summary>Fixes obj, fixes all of obj's children if fixChildren.</summary>
	public void FixObjs(Transform obj) {
		if (fixChildren)
			for (int i=obj.childCount-1; i>=0; --i)
				FixObjs(obj.GetChild(i));
		FixObj(obj);
	}

	public void FixObj(Transform obj) {
		// rotate to (0,0,0) without changing how it looks
		MeshFilter mf = obj.GetComponent<MeshFilter>();
		Mesh m = (mf?mf.mesh:null);
		if (m) {
			// convert vertices to Quaternion.Identity
			Transform parent = obj.parent;
			obj.parent = null;
			Vector3[] verts = m.vertices;
			for (int i=0; i<verts.Length; ++i)
				verts[i] = Vector3.Scale(
					obj.TransformVector(verts[i]),
					new Vector3(1/obj.localScale.x, 1/obj.localScale.y, 1/obj.localScale.z));

			// convert vertices to the final rotation
			Vector3 v1 = obj.TransformDirection(Vector3.right);
			Vector3 v2 = obj.lossyScale;
			float angle = Mathf.Atan2(v1.y, v1.x);
			for (int i=0; i<verts.Length; ++i)
				verts[i] = new Vector3(
					Mathf.Cos(-angle)*verts[i].x*v2.x/v2.x - Mathf.Sin(-angle)*verts[i].y*v2.y/v2.x,
					Mathf.Sin(-angle)*verts[i].x*v2.x/v2.y + Mathf.Cos(-angle)*verts[i].y*v2.y/v2.y,
					verts[i].z);

			// finalize rotation
			obj.eulerAngles = new Vector3(0, 0, angle*180/Mathf.PI);
			//obj.rotation = Quaternion.identity;
			obj.parent = parent;
			m.vertices = verts;
			m.RecalculateNormals();
			m.RecalculateBounds();

			// update collider
			BoxCollider boxColl3d = obj.GetComponent<BoxCollider>();
			if (boxColl3d)
				throw new Exception("Rotator2D Doesn't Work with 3D Colliders");
			BoxCollider2D boxColl = obj.GetComponent<BoxCollider2D>();
			if (boxColl) {
				boxColl.size = m.bounds.size;
				boxColl.offset = m.bounds.center;
			}
		}
	}
}
