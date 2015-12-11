using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Transparent))]
public class DeathZone : MonoBehaviour {
	public float time = 0;
	private float origTime;

	void Start()
	{
		origTime = time;
	}
	
	void OnTriggerStay(Collider coll)
	{
		if (coll.GetComponent<GearGuyCtrl1>())
			time -= Time.fixedDeltaTime;
	}

	void OnTriggerExit(Collider coll)
	{
		if (coll.GetComponent<GearGuyCtrl1>())
			time = origTime;
	}

	void Update()
	{
		if (time<0)
            Application.LoadLevel(Application.loadedLevel);
    }
}
