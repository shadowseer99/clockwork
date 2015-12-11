using UnityEngine;
using System.Collections;

public class StretchMaterial : MonoBehaviour {
	public float ratio=1;
	public Vector2 offset;
	
	public void Awake() {
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector2[] uvs = mesh.uv;
		Vector2 mult = new Vector2(
			Mathf.Max(1, transform.localScale.x/transform.localScale.y),
			Mathf.Max(1, transform.localScale.y/transform.localScale.x));
		for (int i=0; i<uvs.Length; ++i) {
			uvs[i] = offset + ratio*new Vector2(mult.x*uvs[i].x, mult.y*uvs[i].y);
		}
		mesh.uv = uvs;
	}
}
