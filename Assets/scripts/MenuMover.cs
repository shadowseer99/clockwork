using UnityEngine;
using System.Collections;

public class MenuMover : MonoBehaviour {
	public float duration=0.5f;
	public float upDelayRatio=0;
	public float downDelayRatio=1;
	public float additionalMoveUp=0;
	public float pos;
	public float max { get { return 1+downDelayRatio; } }
	public float min { get { return -upDelayRatio; } }
	public bool stretchThis=false;

	private bool hasStarted=false;
	private Vector3 upPos;
	private Vector3 downPos;
	private int dir=0;

	public void Start() {
		if (!hasStarted) {
			hasStarted = true;
			duration *= 1+additionalMoveUp;
			downDelayRatio -= additionalMoveUp;
			downDelayRatio /= (1+additionalMoveUp);
			downPos = transform.position;
			upPos = downPos + (1+additionalMoveUp)*transform.TransformVector(Vector3.up*(transform.root as RectTransform).sizeDelta.y);
			if (stretchThis) {
				RectTransform rt = transform as RectTransform;
				rt.sizeDelta = transform.InverseTransformVector(2*(upPos-downPos));
				print("anchoredPosition: "+(2*downPos-upPos)+", sizeDelta: "+(upPos-downPos));
			}
		}
	}

	public void Update() {
		if (dir==1) {
			pos = Mathf.Min(max, pos+Time.unscaledDeltaTime/duration);
			if (pos==max)
				dir = 0;
			transform.position = Vector3.Lerp(downPos, upPos, pos);
		} else if (dir==-1) {
			pos = Mathf.Max(min, pos-Time.unscaledDeltaTime/duration);
			if (pos==min)
				dir = 0;
			transform.position = Vector3.Lerp(downPos, upPos, pos);
		}
		if (dir==0&&pos==max)
			gameObject.SetActive(false);
	}

	public void MoveUp() {
		gameObject.SetActive(true);
		if (!hasStarted) pos = max;
		Start();
		dir = 1;
	}

	public void MoveDown() {
		gameObject.SetActive(true);
		pos = max;
		if (!hasStarted) pos = min;
		Start();
		dir = -1;
	}

	public void MoveDir(bool up) {
		if (up) MoveUp();
		else MoveDown();
	}
}
