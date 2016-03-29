using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine.UI;

public class LevelSelectMenu : MonoBehaviour {
	public Sprite locked;
	public List<int> _unlockedLevels;
	private static HashSet<int> unlockedLevels;
	public List<RectTransform> levels=new List<RectTransform>();
	private List<RectTransform> locks=new List<RectTransform>();

	// Use this for initialization
	void Start () {
		unlockedLevels = new HashSet<int>(_unlockedLevels);
		for (int i=0; i<levels.Count; ++i) {
			RectTransform t = levels[i].GetComponent<RectTransform>();
			RectTransform levelLock = new GameObject("Level "+levels.Count+" Lock").AddComponent<RectTransform>();
			levelLock.SetParent(t.parent);
			levelLock.sizeDelta = t.sizeDelta;
			levelLock.localScale = t.localScale;
			levelLock.localPosition = t.localPosition;
			Image img = levelLock.gameObject.AddComponent<Image>();
			img.sprite = locked;
			locks.Add(levelLock);

			levels[i].GetComponent<UnityEngine.UI.Button>().enabled = false;
		}
		for (int i=0; i<transform.childCount; ++i) {
			/*RectTransform t = transform.GetChild(i) as RectTransform;
			UnityEngine.UI.Button b = t.GetComponent<UnityEngine.UI.Button>();
			if (b!=null) {
			}*/
			/*FieldInfo[] fields = b.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
			print("fields length: "+fields.Length);
			for (int k=0; k<fields.Length; ++k) {
				print("fields["+k+"]: "+fields[k].Name+", "+fields[k].GetValue(b)+", "+fields[k].DeclaringType+", "+fields[k].FieldType+", "+fields[k].MemberType+", "+fields[k].ReflectedType);
			}*/
		}

		for (int i=0; i<levels.Count; ++i) {
			if (unlockedLevels.Contains(i+1)) {
				for (int remaining=1; i<levels.Count&&remaining>0; --remaining) {
					if (unlockedLevels.Contains(i+2)) ++remaining;
					if (PlayerPrefs.HasKey("Level "+(i+1))) ++remaining;
					levels[i].GetComponent<UnityEngine.UI.Button>().enabled = true;
					locks[i++].gameObject.SetActive(false);
				}
			}
		}

		for (int i=0; i<levels.Count; ++i)
			if (PlayerPrefs.HasKey("Level "+i))
				print("level "+i+" took "+PlayerPrefs.GetFloat("Level "+i)+" seconds");
	}
}
