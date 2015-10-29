using UnityEngine;
using System.Collections;

public class EngageableGear : MonoBehaviour {
	[SerializeField] private float rotrate;
	private Transform gearTrans;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionStay(Collision col)
	{
		if (col.gameObject.CompareTag ("StickyAura")) {
			//float rotScalar = col.gameObject.GetComponent<Rigidbody>().angularVelocity.normalized.magnitude;
			float rotScalar = Input.GetAxis("Horizontal");
			transform.Rotate ( rotScalar*rotrate*(Vector3.left * 90));
		}

	}
	void OcCollisionEnter(Collision col)
	{
		if (col.gameObject.CompareTag ("StickyAura")) {
		}
	}
	void OcCollisionExit(Collision col)
	{
		if (col.gameObject.CompareTag ("StickyAura")) {
		}

	}

}
