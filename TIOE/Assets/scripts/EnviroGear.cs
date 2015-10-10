using UnityEngine;
using System.Collections;

public class EnviroGear : MonoBehaviour {
	[SerializeField] public float rotrate;
	private Transform gearTrans;
	float radius;
	private Rigidbody m_Rigidbody;
	// Use this for initialization
	void Start () {
		radius = gameObject.GetComponent<Collider> ().bounds.extents.x;
		m_Rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
	
		transform.Rotate (rotrate * (Vector3.left * -90));
		//m_Rigidbody.AddTorque (rotrate * (Vector3.left * -90));
	}


}
