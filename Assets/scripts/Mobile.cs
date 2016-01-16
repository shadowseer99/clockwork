using UnityEngine;
using System.Collections;

public class Mobile : MonoBehaviour {
    public static bool reset = false;
    public static bool menu = false;
    public static bool engage = false;
    public static int movement = 0;
	
	public void SetReset(bool value) { reset = value; }
    public void SetMenu(bool value) { menu = value; }
    public void SetEngage(bool value) { engage = value; }
    public void AddMovement(int value) { movement += value; }

    public void Start()
    {
#if UNITY_IPHONE || UNITY_ANDROID
        gameObject.SetActive(false);
#endif
    }
}
