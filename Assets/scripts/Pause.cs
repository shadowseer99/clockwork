using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Pause : MonoBehaviour {

    [SerializeField] private GameObject screen;

    private bool activated = false;
    
    // Use this for initialization

    // Update is called once per frame
    void Update()
    {
#if UNITY_IPHONE || UNITY_ANDROID
        bool reset = Mobile.reset;
        bool menu = Mobile.menu;
#else
        // Read the inputs.
        bool reset = CrossPlatformInputManager.GetButtonDown("Reset");
        bool menu = CrossPlatformInputManager.GetButtonDown("Pause");
#endif

        if (reset)
        {
            Reset();
        }
        
        if (menu)
        {
            if(!activated)
            {
                Time.timeScale = 0;
                activated = true;
            }
            else
            {
                Time.timeScale = 1;
                activated = false;
            }
        }
        screen.SetActive(activated);
    }
    public void Resume()
    {
        Time.timeScale = 1;
        activated = false;
    }
    public void Reset()
    {
        Time.timeScale = 1;
        Application.LoadLevel(Application.loadedLevel);
    }
    public void MainMenu()
    {
        Time.timeScale = 1;
        Application.LoadLevel("Main Menu");
    }
    public void Exit()
    {
        Application.Quit();
    }
}
