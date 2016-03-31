using System;
using UnityEngine;

public class Kickback2D:MonoBehaviour {
	public enum KickbackMode { speed, energy }
	public KickbackMode kickbackMode;
	public AudioCustom _hitSound;
	private AudioSource hitSound;
	public float mult=1;

	public void Start() {
		hitSound = gameObject.AddComponent<AudioSource>();
		hitSound.clip = _hitSound.clip;
		hitSound.loop = false;
	}
	
	public void OnCollisionExit2D(Collision2D coll) {
		if (coll.gameObject.GetComponent<CollidingObject>()!=null) {
			hitSound.Play();
			hitSound.volume = _hitSound.volume*SoundProfile.effects;

			if (kickbackMode==KickbackMode.speed)
				coll.rigidbody.velocity *= mult;
			else if (kickbackMode==KickbackMode.energy)
				coll.rigidbody.velocity *= Mathf.Sqrt(mult);
			else
				throw new Exception("Invalid KickbackMode");
		}
	}
}