using UnityEngine;
using System.Collections;

public class Water : MonoBehaviour {

	public Vector3 flow=Vector3.zero;
	public float densityRatio=0;
	public float thicknessRatio=1;
	public Vector3[] contactPoints=new Vector3[0];

	private static Water _nullWater;
	public static Water nullWater
	{
		get
		{
			if (_nullWater==null)
				_nullWater = new GameObject("Null Water").AddComponent<Water>();
			return _nullWater;
		}
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
