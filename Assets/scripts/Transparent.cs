using UnityEngine;
using System.Collections;

public class Transparent : MonoBehaviour {
	[Tooltip("Higher numbers render later (and draw on top of earlier objects)")]public int renderOrder=100;
	
	public void Start()
	{
		Renderer renderer = GetComponent<Renderer>();
		if (renderer!=null)
			renderer.sortingOrder = renderOrder;
	}
}
