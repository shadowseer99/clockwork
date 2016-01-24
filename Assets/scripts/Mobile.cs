using UnityEngine;
using System.Collections;

public class Mobile : MonoBehaviour {
    public static bool reset = false;
    public static bool menu = false;
    public static bool engage = false;
    public static bool left = false;
    public static bool right = false;
    public static bool active = true;

    public void SetReset(bool value) { reset = value; }
    public void SetMenu(bool value) { menu = value; }
    public void SetEngage(bool value) { engage = value; }
    public void SetLeft(bool value) { left = value; }
    public void SetRight(bool value) { right = value; }

    void Update()
    {
        //gameObject.SetActive(active);
    }

    public void Start()
    {
#if UNITY_IPHONE || UNITY_ANDROID

#else
        gameObject.SetActive(false);
#endif
    }
}
