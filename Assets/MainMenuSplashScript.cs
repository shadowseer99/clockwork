using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSplashScript:MonoBehaviour {
	public float timer=5;
	public float fadeTime=1;
	private float fadeTimer=0;
	private MenuManager menu;
	private List<Image> images=new List<Image>();

	public void Start() {
		menu = GameObject.FindObjectOfType<MenuManager>();
		//print("Main menu splash is on "+gameObject.name);

		// find all images, levelend.GetComponentsInChildren<Image>() wasn't working for some reason
		Stack<Transform> todo = new Stack<Transform>();
		todo.Push(transform);
		while (todo.Count>0) {
			Transform t = todo.Pop();
			for (int i=0; i<t.childCount; ++i)
				todo.Push(t.GetChild(i));
			Image img = t.GetComponent<Image>();
			if (img!=null)
				images.Add(img);
		}
	}

	public void Update() {
		if (timer <= 0) {
			fadeTimer = Mathf.Min(fadeTimer+Time.deltaTime, fadeTime);
			for (int i=0; i<images.Count; ++i) {
				Color c = images[i].color;
				images[i].color = new Color(c.r, c.g, c.b, 1-fadeTimer/fadeTime);
			}
			transform.parent.GetChild(0).gameObject.SetActive(true);
			if (fadeTimer==fadeTime) gameObject.SetActive(false);
		}
		timer -= Time.deltaTime;
	}

	public void StartFade() {
		timer = 0;
	}
}
