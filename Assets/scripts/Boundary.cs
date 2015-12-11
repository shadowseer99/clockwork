using UnityEngine;
using System.Collections;

public class Boundary : MonoBehaviour {

	public float bottomPadding=5;
	public float topPadding=50;
	public float leftPadding=20;
	public float rightPadding=20;
	public float thickness=100;
	private bool end=false;

	void Start()
	{
		// search for the highest and lowest colliders in the level
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.localScale = Vector3.one;
		Collider[] colliders = GameObject.FindObjectsOfType<Collider>();
		float top=float.NegativeInfinity;
		float bottom=float.PositiveInfinity;
		float left=float.PositiveInfinity;
		float right=float.NegativeInfinity;

		for (int i=0; i<colliders.Length; ++i)
		{
			top = Mathf.Max(top, colliders[i].bounds.max.y);
			bottom = Mathf.Min(bottom, colliders[i].bounds.min.y);
			left = Mathf.Min(left, colliders[i].bounds.min.x);
			right = Mathf.Max(right, colliders[i].bounds.max.x);
		}
		top += topPadding + thickness/2-0.5f;
		bottom -= bottomPadding + thickness/2-0.5f;
		left -= leftPadding + thickness/2-0.5f;
		right += rightPadding + thickness/2-0.5f;

		// make the colliders
		// top collider
		BoxCollider col;
		col = gameObject.AddComponent<BoxCollider>();
		col.center = new Vector3((left+right)/2, top+0.5f, 0);
		col.size = new Vector3(2+right-left, thickness, 1);
		col.isTrigger = true;

		// bottom collider
		col = gameObject.AddComponent<BoxCollider>();
		col.center = new Vector3((left+right)/2, bottom-0.5f, 0);
		col.size = new Vector3(2+right-left, thickness, 1);
		col.isTrigger = true;
		
		// left collider
		col = gameObject.AddComponent<BoxCollider>();
		col.center = new Vector3(left-0.5f, (top+bottom)/2, 0);
		col.size = new Vector3(thickness, 2+top-bottom, 1);
		col.isTrigger = true;
		
		// right collider
		col = gameObject.AddComponent<BoxCollider>();
		col.center = new Vector3(right+0.5f, (top+bottom)/2, 0);
		col.size = new Vector3(thickness, 2+top-bottom, 1);
		col.isTrigger = true;
	}

	void OnTriggerEnter(Collider coll)
	{
		if (coll.GetComponent<GearGuyCtrl1>())
			end = true;
	}

	void Update()
	{
		if (end)
            Application.LoadLevel(Application.loadedLevel);
    }
}
