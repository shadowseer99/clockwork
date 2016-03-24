using UnityEngine;
using System.Collections;

public class HudScript : MonoBehaviour {

	public void reset()
    {
        Debug.Log("reset");
        Application.LoadLevel(Application.loadedLevel);
    }
}
