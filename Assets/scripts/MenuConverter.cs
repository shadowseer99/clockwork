using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class MenuConverter : MonoBehaviour {
	public void Start() {
		this.enabled = false;

		// find vars
		RectTransform root = transform.root as RectTransform;
		Vector2 dim = root.offsetMax;
		RectTransform rt = transform as RectTransform;
		Image img = rt.GetComponent<Image>();
		float ratio = img.preferredHeight/img.preferredWidth;

		// center everything first
		/*rt.anchorMax = Vector2.one/2;
		rt.anchorMin = Vector2.one/2;
		rt.pivot = Vector2.one/2;*/

		// adjust scale
		rt.sizeDelta = new Vector2(Mathf.Min(rt.sizeDelta.x, rt.sizeDelta.y/ratio), Mathf.Min(rt.sizeDelta.y, rt.sizeDelta.x*ratio));
		rt.sizeDelta = Vector2.Scale(rt.sizeDelta, rt.localScale);
		rt.localScale = Vector2.one;

		// change anchor
		rt.anchorMax = new Vector2(rt.offsetMax.x/dim.x + 0.5f, rt.offsetMax.y/dim.y + 0.5f);
		rt.anchorMin = new Vector2(rt.offsetMin.x/dim.x + 0.5f, rt.offsetMin.y/dim.y + 0.5f);
		rt.anchoredPosition = Vector2.zero;
		rt.sizeDelta = Vector2.zero;

		// set pivot
		Vector2 pos = rt.position;
		Vector2 zero = rt.offsetMin+pos;
		Vector2 dir = rt.offsetMax-rt.offsetMin;
		Vector2 diff = pos-dim/2;
		rt.pivot = new Vector2((0.5f-rt.anchorMin.x)/(rt.anchorMax.x-rt.anchorMin.x), (0.5f-rt.anchorMin.y)/(rt.anchorMax.y-rt.anchorMin.y));
		rt.localScale = Vector2.one*0.965f;

		DestroyImmediate(this);
	}

	public void Update() { Start(); }
}
