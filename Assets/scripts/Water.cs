using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Transparent))]
public class Water : MonoBehaviour {

	public Vector3 flow=Vector3.zero;
	public float densityRatio=0;
	public float thicknessRatio=1;
	public Vector3[] contactPoints=new Vector3[0];
	public AudioClip _flow;
	public AudioClip _splash;
	[HideInInspector]public AudioSource flow2;
	[HideInInspector]public AudioSource splash;

	private static Water _nullWater;
	public static Water nullWater {
		get {
			if (_nullWater==null) {
				GameObject obj = GameObject.Find("Null Water");
				if (obj)
					_nullWater = obj.GetComponent<Water>();
				if (_nullWater==null)
					_nullWater = new GameObject("Null Water").AddComponent<Water>();
			}
			return _nullWater;
		}
	}

	public void Start() {
		splash = new GameObject("splash").AddComponent<AudioSource>();
		splash.transform.parent = transform;
		flow2 = gameObject.AddComponent<AudioSource>();
		splash.clip = _splash;
		flow2.clip = _flow;
		splash.loop = false;
		flow2.loop = true;
		flow2.Play();
	}

	public Vector3 GetContactPoint(Transform t)
	{
		Vector3 result=Vector3.one*float.PositiveInfinity;
		for (int i=0; i<contactPoints.Length; ++i)
			if ((t.position-contactPoints[i]).magnitude<(t.position-result).magnitude)
				result = contactPoints[i];
		if (result.magnitude>1000000)
			result = Vector3.zero;
		return result;
	}
}
