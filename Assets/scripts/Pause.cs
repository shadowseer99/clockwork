using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Pause : MonoBehaviour {

    [SerializeField] private GameObject screen;
	[SerializeField] private GameObject levelend;

    private bool activated = false;
    
    // Use this for initialization

    // Update is called once per frame
	void Start()
	{
		levelend.SetActive (false);
	}
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
#if UNITY_IPHONE || UNITY_ANDROID
                
                Mobile.menu=false;
                Mobile.active = false;
#endif
                Time.timeScale = 0;
                activated = true;
            }
            else
            {
#if UNITY_IPHONE || UNITY_ANDROID
            
                Mobile.active = true;
#endif
                Time.timeScale = 1;
                activated = false;
            }
        }
        screen.SetActive(activated);
    }
	public void NextLevel()
	{
		Time.timeScale = 1;
		Application.LoadLevel (Application.loadedLevel + 1);
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
	public void EndLevel()
	{
#if UNITY_IPHONE || UNITY_ANDROID
        Mobile.menu = false;
        Mobile.active = false;
#endif
        Time.timeScale = 0;
		levelend.SetActive (true);

    }
}
