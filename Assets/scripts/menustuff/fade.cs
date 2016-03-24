using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class fade : MonoBehaviour {

	public Image img;
	private float fadetime = 6;
	private Color col = Color.black;
	private float timer;

	// Use this for initialization
	void Start () {
		timer = fadetime;
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		col.a =  (timer/fadetime);
		img.color = col;
		if(timer<=fadetime/4)
		{
			Destroy(this);
		}
	
	}
}
