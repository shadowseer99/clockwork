#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

public class GearSelector : MonoBehaviour {
	public Gear[] gears;
	public Material goldMaterial;

	public Gear GetGearByPegs(int numPegs, bool isGolden=false) {
		// find appropriate gear
		Gear result=gears[0];
		for (int i=1; i<gears.Length; ++i)
			if (Mathf.Abs(gears[i].numPegs-numPegs) < Mathf.Abs(result.numPegs-numPegs))
				result = gears[i];
		result = PrefabUtility.InstantiatePrefab(result) as Gear;
		// handle golden gears and start it
		if (result) {
			result.Start();
			result.isGolden = isGolden;
			if (isGolden)
				result.transform.GetChild(0).GetComponent<Renderer>().material = goldMaterial;
		}
		if (result==null)
			print("NO GEAR FOUND WITH "+numPegs+" PEGS");
		return result;
	}

	public Gear GetGearByTrigSize(float radius, bool isGolden=false) {
		return GetGearByPegs(2*(int)Mathf.Round((radius)/0.1f), isGolden);
	}

	public Gear GetGearByCollSize(float radius, bool isGolden=false) {
		return GetGearByTrigSize(radius-0.05f, isGolden);
	}
}

[CustomEditor(typeof(GearSelector))]
[CanEditMultipleObjects]
public class GearSelectorEditor:Editor {
	private static int numPegs=20;
	private static float trigSize=1;
	private static float collSize=1;
	private static bool isGolden=false;
	
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
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
}
#endif