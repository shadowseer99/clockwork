////Endscreen
/// Attached to camera
/// Dylan Noaker's code
using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;
public class Endscreen : MonoBehaviour {
	[SerializeField] private Texture ScreenTexture;


	void Start()
	{
	
	}

	void OnGUI(){

		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), ScreenTexture);
	
///Let Player's decide if they want to go back to main menu.
		if (GUI.Button (new Rect(Screen.width * .5f, Screen.height * .5f, Screen.width * .5f, Screen.height * .1f),"New Round?")) {

			//take us back to main
			Application.LoadLevel("Demo");

		}

	}
}
