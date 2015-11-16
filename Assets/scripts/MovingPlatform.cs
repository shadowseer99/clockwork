using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class MovingPlatform : MonoBehaviour {

	public float startWeight;
	public float weight;
	protected List<Rigidbody> restingObjs=new List<Rigidbody>();
	public bool isResting=false;
	public Vector3 connectionPoint;
	protected Rigidbody body;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionEnter(Collision coll)
	{
		Rigidbody collBody = coll.gameObject.GetComponent<Rigidbody> ();
		if (collBody != null&&collBody.position.y-body.position.y>0) 
		{
			restingObjs.Add(collBody);
		}
	}
	void OnCollisionStay(Collision coll)
	{

	}
	void OnCollisionExit(Collision coll)
	{
		Rigidbody collBody = coll.gameObject.GetComponent<Rigidbody> ();
		if (collBody != null) 
		{
			restingObjs.Remove(collBody);
		}
	}

	public abstract void SetRopeLength(float length,Vector3 connection);
}
