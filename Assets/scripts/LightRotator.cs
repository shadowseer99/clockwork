using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LightRotator : MonoBehaviour {
	public float speed=45;
	public Vector3 rotateAround=Vector3.up;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Application.isPlaying) transform.Rotate(rotateAround, Time.fixedDeltaTime*speed, Space.World);
		if (!Application.isPlaying) Debug.DrawRay(transform.position, rotateAround*50, Color.black);
	}
}
