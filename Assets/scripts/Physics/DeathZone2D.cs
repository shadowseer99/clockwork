using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Transparent), typeof(Collider2D))]
public class DeathZone2D:MonoBehaviour {
	public float time = 0;
	private float origTime;

	void Start() {
		origTime = time;
	}
	
	void OnTriggerStay2D(Collider2D coll) {
		if (coll.GetComponent<PlayerGear>())
			time -= Time.fixedDeltaTime;
	}

	void OnTriggerExit2D(Collider2D coll) {
		if (coll.GetComponent<PlayerGear>())
			time = origTime;
	}

	void Update() {
		if (time<0)
            Application.LoadLevel(Application.loadedLevel);
    }
}
