using UnityEngine;
using System.Collections;

public class WaterStagnant : MonoBehaviour {
	private Mesh mesh;
	private Vector2[] origUvs;
	private float counter;
	public float wiggleMult=0.01f;
	public float wiggleSpeed=10;
	public float speed=10;

	// Use this for initialization
	void Start () {
		mesh = GetComponent<MeshFilter>().mesh;
		mesh.MarkDynamic();
		origUvs = mesh.uv;
	}
	
	// Update is called once per frame
	void Update () {
		counter += Time.deltaTime;
		Vector2[] uvs = (Vector2[])origUvs.Clone();

		for (int i=0; i<uvs.Length; ++i)
			uvs[i] += new Vector2(wiggleMult*Mathf.Sin(counter*wiggleSpeed), 0.01f*counter*speed);
		mesh.uv = uvs;
	}
}
