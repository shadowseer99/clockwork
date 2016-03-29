using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using System.Collections.Generic;

public class Pause : MonoBehaviour {

    [SerializeField] private GameObject screen;
	[SerializeField] private GameObject levelend;
    [SerializeField] private GameObject options;
    [SerializeField] private float fadeTime=1;
	[SerializeField] private float fadeDelay=1;
    [SerializeField] private GameObject endBanner;
    private float timer=0;
	private List<Image> images=new List<Image>();
	private float timeSpent=0;
	public Sprite[] numbers;
	public RectTransform timeObject;
    private bool activated = false;
    private bool dropping = false;
    private float timedelay=0;
    
    // Use this for initialization

    // Update is called once per frame
	void Start()
	{
		levelend.SetActive (false);
		timer -= fadeDelay;

		// find all images, levelend.GetComponentsInChildren<Image>() wasn't working for some reason
		Stack<Transform> todo = new Stack<Transform>();
		todo.Push(levelend.transform);
		while (todo.Count>0) {
			Transform t = todo.Pop();
			for (int i=0; i<t.childCount; ++i)
				todo.Push(t.GetChild(i));
			Image img = t.GetComponent<Image>();
			if (img!=null && img.gameObject!=levelend)
				images.Add(img);
		}
	}
    void Update()
    {
        if(dropping&&endBanner.transform.position.y>0)
        {
            endBanner.transform.Translate(0, -500 * Time.unscaledDeltaTime, 0);
            if(Time.realtimeSinceStartup-timedelay>2)
            {
                loadnext();
            }
        }
        else if(!dropping)
        {
            timeSpent += Time.deltaTime;

            // handle fade
            if (levelend.activeSelf)
            {
                timer = Mathf.Min(timer + Time.unscaledDeltaTime, fadeTime);
                for (int i = 0; i < images.Count; ++i)
                {
                    Color c = images[i].color;
                    images[i].color = new Color(c.r, c.g, c.b, Mathf.Max(timer / fadeTime, 0));
                }
                Text t = timeObject.GetComponent<Text>();
                t.color = new Color(t.color.r, t.color.b, t.color.b, Mathf.Max(timer / fadeTime, 0));
                Image img = levelend.GetComponent<Image>();
                img.color = new Color(img.color.r, img.color.g, img.color.b, Mathf.Min(Mathf.Max((timer + fadeDelay) / fadeTime, 0), 1));
            }


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
                if (!activated)
                {
#if UNITY_IPHONE || UNITY_ANDROID
                
                Mobile.menu=false;
                Mobile.active = false;
#endif
                    if (!dropping)
                    {
                        Time.timeScale = 0;
                    }

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
        
		
    }
	public void PrintText() { print("Text"); }
	public void NextLevel()
	{
        dropping = true;
		print("next level...");

        timedelay = Time.realtimeSinceStartup;
        
	}

    private void loadnext()
    {
        Time.timeScale = 1;
        Application.LoadLevel(Application.loadedLevel + 1);
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
    public void Options()
    {
        activated = false;
        options.SetActive(true);
    }
    public void closeOp()
    {
        activated = true;
        options.SetActive(false);
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
		levelend.GetComponent<MenuMover>().MoveDown();

		string timeStr = Mathf.RoundToInt(timeSpent).ToString();
		for (int i=0; i<timeStr.Length; ++i) {
			GameObject temp = new GameObject("Number "+(timeStr[i]-'0'));
			temp.transform.parent = timeObject;
			temp.AddComponent<RectTransform>();
			RectTransform newObj = temp.transform as RectTransform;
			Image img = newObj.gameObject.AddComponent<Image>();
			images.Add(img);
			img.sprite = numbers[timeStr[i]-'0'];
			newObj.localPosition = Vector3.right*40*i;
		}
    }
}
