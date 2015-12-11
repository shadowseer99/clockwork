using UnityEngine;
using System.Collections;

public class Transparent : MonoBehaviour {
	
	public void Start()
	{
		Renderer renderer = GetComponent<Renderer>();
		if (renderer!=null)
			renderer.sortingOrder = 100;
	}
}
