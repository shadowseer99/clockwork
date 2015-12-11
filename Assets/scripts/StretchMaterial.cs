using UnityEngine;
using System.Collections;

public class StretchMaterial : MonoBehaviour {
	public float ratio=1;
	public Vector2 offset;
	
	public void Awake() {
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector2[] uvs = mesh.uv;
		float x = transform.TransformVector(Vector3.right.normalized).magnitude;
		float y = transform.TransformVector(Vector3.up.normalized).magnitude;
		Vector2 mult = new Vector2(
			Mathf.Max(1, x/y),
			Mathf.Max(1, y/x));
		for (int i=0; i<uvs.Length; ++i) {
			uvs[i] = offset + ratio*new Vector2(mult.x*uvs[i].x, mult.y*uvs[i].y);
		}
		mesh.uv = uvs;
	}
}
