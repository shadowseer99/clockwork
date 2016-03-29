using UnityEngine;
using System.Collections;

public class MovingPlatform2 : MonoBehaviour {

	public Vector3 endPos;
	public float speed;
	private Vector3 startPos;
	private Vector3 dir;
	new private Rigidbody2D rigidbody;

	// Use this for initialization
	void Start () {
		startPos = transform.position;
		dir = (endPos-startPos).normalized;
		rigidbody = GetComponent<Rigidbody2D>();
		rigidbody.velocity = dir*speed;
		rigidbody.isKinematic = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		rigidbody.position = (rigidbody.position + Time.fixedDeltaTime*rigidbody.velocity);
		if (Vector3.Dot(transform.position-endPos, dir)>=0)
			rigidbody.velocity = -dir*speed;
		if (Vector3.Dot(transform.position-startPos, dir)<=0)
			rigidbody.velocity = dir*speed;
	}
}
