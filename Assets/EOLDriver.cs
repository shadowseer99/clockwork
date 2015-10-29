using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Collider))]
public class EOLDriver : MonoBehaviour {
	[SerializeField] private string nextLevel;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.CompareTag ("Player") || col.gameObject.CompareTag ("StickyAura")) {
			Application.LoadLevel(nextLevel);
		}
	}
}
