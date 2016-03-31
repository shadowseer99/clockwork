#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class PlayerPrefsEditor : MonoBehaviour {}


[CustomEditor(typeof(PlayerPrefsEditor))]
[CanEditMultipleObjects]
public class PlayerPrefsEditorEditor:Editor {
	private List<float> times=new List<float>();
	private List<float> bronzeTimes=new List<float>();
	private List<float> silverTimes=new List<float>();
	private List<float> goldTimes=new List<float>();

	private void Begin() {
		times.Clear();
		bronzeTimes.Clear();
		silverTimes.Clear();
		goldTimes.Clear();
		//(target as LevelSelectMenu).levels.Count
		for (int i=0; i<50; ++i) {
			times.Add(PlayerPrefs.GetFloat("Level "+i));
			bronzeTimes.Add(PlayerPrefs.GetFloat("Level "+i+" Bronze"));
			silverTimes.Add(PlayerPrefs.GetFloat("Level "+i+" Silver"));
			goldTimes.Add(PlayerPrefs.GetFloat("Level "+i+" Gold"));
		}
	}
	
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		if (times.Count==0)
			Begin();
		
		if (GUILayout.Button("Reset all data")) {
			for (int i=0; i<50; ++i) {
				PlayerPrefs.DeleteKey("Level "+i);
				PlayerPrefs.DeleteKey("Level "+i+" Bronze");
				PlayerPrefs.DeleteKey("Level "+i+" Silver");
				PlayerPrefs.DeleteKey("Level "+i+" Gold");
			}
			PlayerPrefs.SetString("test", "test");
			PlayerPrefs.Save();
			LevelSelectMenu lsMenu = GameObject.FindObjectOfType<LevelSelectMenu>();
			if (lsMenu!=null && Application.isPlaying) lsMenu.ResetLocks();
			Begin();
		}
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Level");
		GUILayout.Label("Time");
		GUILayout.Label("Bronze");
		GUILayout.Label("Silver");
		GUILayout.Label("Gold");
		EditorGUILayout.EndHorizontal();

		for (int i=0; i<times.Count; ++i) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Level "+(i+1));
			times[i] = HandleField(times[i], "Level "+(i+1));
			bronzeTimes[i] = HandleField(bronzeTimes[i], "Level "+(i+1)+" Bronze");
			silverTimes[i] = HandleField(silverTimes[i], "Level "+(i+1)+" Silver");
			goldTimes[i] = HandleField(goldTimes[i], "Level "+(i+1)+" Gold");
			EditorGUILayout.EndHorizontal();
		}
	}

	private float HandleField(float value, string name) {
		float f = EditorGUILayout.FloatField(value);
		if (f!=value) {
			if (f==0) PlayerPrefs.DeleteKey(name);
			else PlayerPrefs.SetFloat(name, f);
			PlayerPrefs.Save();
			LevelSelectMenu lsMenu = GameObject.FindObjectOfType<LevelSelectMenu>();
			if (lsMenu!=null && Application.isPlaying) lsMenu.ResetLocks();
		}
		return f;
	}
}
#endif
