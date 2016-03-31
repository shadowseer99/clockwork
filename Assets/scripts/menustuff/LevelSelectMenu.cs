#if UNITY_EDITOR
using UnityEditor;
#endif
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

			UnlockLevel(i, false);
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
					UnlockLevel(i++, true);
				}
			}
		}

		bool l46=true, l48=true, l49=true;
		for (int i=0; i<15; ++i) {
			l46 = l46 && PlayerPrefs.HasKey("Level "+(i+1));
			l48 = l48 && PlayerPrefs.HasKey("Level "+(i+16));
			l49 = l49 && PlayerPrefs.HasKey("Level "+(i+31));
		}
		UnlockLevel(46, l46);
		UnlockLevel(47, PlayerPrefs.HasKey("Level 46"));
		UnlockLevel(48, l48);
		UnlockLevel(49, l49);
		UnlockLevel(50, PlayerPrefs.HasKey("Level 47") && PlayerPrefs.HasKey("Level 48") && PlayerPrefs.HasKey("Level 49"));

		for (int i=0; i<levels.Count; ++i)
			if (PlayerPrefs.HasKey("Level "+i))
				print("level "+i+" took "+PlayerPrefs.GetFloat("Level "+i)+" seconds");
	}

	private void UnlockLevel(int level, bool lockLevel) {
		levels[level].GetComponent<UnityEngine.UI.Button>().enabled = lockLevel;
		locks[level].gameObject.SetActive(!lockLevel);
	}
}



#if UNITY_EDITOR
/*[CustomEditor(typeof(LevelSelectMenu))]
[CanEditMultipleObjects]
public class LevelSelectMenuEditor:Editor {
	private List<float> times=new List<float>();
	private List<float> bronzeTimes=new List<float>();
	private List<float> silverTimes=new List<float>();
	private List<float> goldTimes=new List<float>();
	
	public override void OnInspectorGUI() {
		if (times.Count==0) {
			for (int i=0; i<(target as LevelSelectMenu).levels.Count; ++i) {
				times.Add(PlayerPrefs.GetFloat("Level "+i));
				bronzeTimes.Add(PlayerPrefs.GetFloat("Level "+i+" Bronze"));
				silverTimes.Add(PlayerPrefs.GetFloat("Level "+i+" Silver"));
				goldTimes.Add(PlayerPrefs.GetFloat("Level "+i+" Gold"));
			}
		}

		for (int i=0; i<times.Count; ++i) {
			Rect r = EditorGUILayout.BeginHorizontal();
			GUILayout.Label(("L"+i).PadLeft(3, ' '));
			//EditorGUI.
		}
		
		isGolden = EditorGUILayout.Toggle("Golden Gear", isGolden);
		numPegs = EditorGUILayout.IntField("Number of Pegs", numPegs);
		if (GUILayout.Button("Create Gear with "+numPegs+" pegs"))
			(target as GearSelector).GetGearByPegs(numPegs, isGolden);
		collSize = EditorGUILayout.FloatField("Collider size", collSize);
		if (GUILayout.Button("Create Gear with coll radius of "+collSize))
			(target as GearSelector).GetGearByCollSize(collSize, isGolden);
		trigSize = EditorGUILayout.FloatField("Trigger size", trigSize);
		if (GUILayout.Button("Create Gear with trig radius of "+trigSize))
			(target as GearSelector).GetGearByTrigSize(trigSize, isGolden);
	}
}*/
#endif