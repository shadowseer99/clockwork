using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollScript : MonoBehaviour {
	public RectTransform neighbors;
	public float offset=120;
	private float minPos;
	private float maxPos;
	private Vector3 initPos;
	private float origY;
	private bool init=false;
	private Canvas canvas;

	public void Start() {
		GetComponent<Scrollbar>().onValueChanged.AddListener(ScrollBarUpdate);
		canvas = transform.root.GetComponent<Canvas>();
	}

	public void Init() {
		init = true;
		minPos = float.PositiveInfinity;
		maxPos = float.NegativeInfinity;
		initPos = neighbors.parent.position;
		origY = neighbors.localPosition.y;
		Vector3[] corners = new Vector3[4];
		for (int i=0; i<neighbors.childCount; ++i) {
			RectTransform rt = neighbors.GetChild(i).transform as RectTransform;
			rt.GetWorldCorners(corners);
			for (int j=0; j<corners.Length; ++j) {
				minPos = Mathf.Min(minPos, corners[j].y);
				maxPos = Mathf.Max(maxPos, corners[j].y);
			}
		}
	}
	
	// Update is called once per frame
	public void ScrollBarUpdate(float val) {
		if (!init)
			Init();
		float diff = maxPos - minPos;
		neighbors.localPosition = new Vector3(neighbors.localPosition.x, origY, neighbors.localPosition.z);
		neighbors.position += Vector3.up*(val*(diff-offset));
	}
}
