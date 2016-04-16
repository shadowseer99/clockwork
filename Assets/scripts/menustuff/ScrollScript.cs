using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollScript : MonoBehaviour {
	public RectTransform neighbors;
	public float padding=10;
	private float minPos;
	private float maxPos;
	private Vector3 initPos;
	private float origY;

	public void Start() {
		GetComponent<Scrollbar>().onValueChanged.AddListener(ScrollBarUpdate);
		
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
		float diff = maxPos - minPos;
		float screenDiff = diff - Screen.height;
		float y = val*screenDiff;
		neighbors.localPosition = new Vector3(neighbors.localPosition.x, origY - val*Screen.height, neighbors.localPosition.z);
		neighbors.position = new Vector3(neighbors.position.x, neighbors.position.y + val*diff, neighbors.position.z);
		// f(0) = origY f(1) = 
	}
}
