using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour {

    [SerializeField] private GameObject screen;

    private bool activated = false;
    
    // Use this for initialization

    // Update is called once per frame
    void Update()
    {
        bool reset = Input.GetKeyDown(KeyCode.R);
        bool menu = Input.GetKeyDown(KeyCode.Escape);
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
