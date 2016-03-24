////mainmenu
/// Attached to camera
using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;
public class MainMenu : MonoBehaviour {
	[SerializeField] private Texture Mainmenutexture;
	[SerializeField] private Texture Level1Thumb;
	[SerializeField] private Texture Level2Thumb;
	[SerializeField] private Texture Level3Thumb;




	void OnGUI(){

		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), Mainmenutexture);
	
///Button display
		/// Make sure levels are included in build settings and adhere to naming convention "LevelX".
		/// level1
		if (GUI.Button (new Rect(Screen.width * .5f, Screen.height * .5f, Screen.width * .5f, Screen.height * .1f), new GUIContent("Play Level1",Level1Thumb),"")) {
			Application.LoadLevel("Level1");
		}
	}
}
