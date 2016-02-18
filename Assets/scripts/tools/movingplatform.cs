using UnityEngine;
using System.Collections;

public class movingplatform : MonoBehaviour {

	public int range=0;
	public bool vertical=false;
	public bool invertD=false;
	public float speed=1;
	private Vector3 direction;
	private float lowb;
	private float upb;

	// Use this for initialization
	void Start () 
	{
		if (vertical) {
			direction = Vector3.up;
			if (invertD) {
				upb = transform.position.y;
				lowb = upb - range;
			} else {
				lowb = transform.position.y;
				upb = lowb + range;
			}
		} else {
			direction = Vector3.right;
			if (invertD) {
				upb = transform.position.x;
				lowb = upb - range;
			} else {
				lowb = transform.position.x;
				upb = lowb + range;
			}
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		transform.Translate (direction * speed * Time.deltaTime);
		if (vertical && (transform.position.y <= lowb || transform.position.y >= upb)) {
			direction = direction * -1;
			transform.Translate (direction * speed * Time.deltaTime);
		} else if (!vertical && (transform.position.x <= lowb || transform.position.x >= upb)) {
			direction = direction * -1;
			transform.Translate (direction * speed * Time.deltaTime);
		}
	}
}
