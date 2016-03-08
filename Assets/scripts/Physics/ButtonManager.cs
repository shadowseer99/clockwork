using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonManager : MonoBehaviour {
	private Button2D[] buttons;
	public Button2D active=null;
	private Dictionary<Button2D, bool> wasActive=new Dictionary<Button2D,bool>();

	public void Start() {
		buttons = GetComponentsInChildren<Button2D>();
	}
	
	void Update() {
		// search for an activated button
		if (active==null) {
			for (int i=0; i<buttons.Length; ++i) {
				if (buttons[i].state!=0) {
					// record active button, turn off all buttons
					active = buttons[i];
					for (int j=0; j<buttons.Length; ++j) {
						if (buttons[j].gameObject!=active.gameObject) {
							buttons[j].state = 0;
							wasActive[buttons[j]] = buttons[j].enabled;
							buttons[j].enabled = false;
						}
					}
					break;
				}
			}
		}

		// if active button is no longer activated
		if (active!=null && active.state==0) {
			// clear active button, turn on all buttons
			active = null;
			for (int i=0; i<buttons.Length; ++i) {
				if (wasActive.ContainsKey(buttons[i]) && wasActive[buttons[i]])
					buttons[i].enabled = true;
			}
			wasActive.Clear();
		}
	}
}
