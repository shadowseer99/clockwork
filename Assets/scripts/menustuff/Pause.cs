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
	public Sprite colon;
	public RectTransform timeObject;
	public float bronzeTime=180;
	public float silverTime=120;
	public float goldTime=60;
    private bool activated = false;
    private bool dropping = false;
    private float timedelay=0;
    private Image[] faders;
    
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
        //hotkeys
        if(screen.activeInHierarchy)
        {
            if(Input.GetKeyDown(KeyCode.C)|| Input.GetKeyDown(KeyCode.Escape))
            {
                Resume();
                return;
            }
            else if(Input.GetKeyDown(KeyCode.O))
            {
                Options();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                Reset();
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                MainMenu();
            }
        }
        else if (options.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                options.GetComponent<OptionsMenu>().setSounds();
                Invoke("closeOp", .001f);
                //closeOp();
                
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                closeOp();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                options.GetComponent<OptionsMenu>().setSounds();
                Invoke("closeOp", .001f);
                Invoke("Resume", .00105f);
                return;
            }
            
        }

        if (dropping&&endBanner.transform.position.y>0)
        {
            endBanner.transform.Translate(0, -500 * Time.unscaledDeltaTime, 0);
            if(Time.realtimeSinceStartup-timedelay<2)
            {
                foreach(Image i in faders)
                {
                    i.color = new Color(1, 1, 1, (Time.realtimeSinceStartup - timedelay)-1);
                }
            }
            else
            {
                foreach (Image i in faders)
                {
                    i.color = new Color(1, 1, 1);
                }
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
                else if(false)
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
		print("next level...");
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
        dropping = true;
        timedelay = Time.realtimeSinceStartup;
        levelend.SetActive(true);
        faders = levelend.GetComponentsInChildren<Image>();
		string level = "Level "+Application.loadedLevel;
		PlayerPrefs.SetFloat(level, PlayerPrefs.HasKey(level)?Mathf.Min(PlayerPrefs.GetFloat(level), timeSpent):timeSpent);
		PlayerPrefs.Save();
		AddNumber(timeObject, timeSpent, true);
    }

	public void AddNumber(RectTransform neighbor, float number, bool isTime) {
		int time = Mathf.RoundToInt(number);
		string timeStr = (isTime?(time/60)+":"+(time%60).ToString().PadLeft(2, '0'):time.ToString());
		
		for (int i=0; i<timeStr.Length; ++i) {
			GameObject temp = new GameObject("Number "+(timeStr[i]-'0'));
			RectTransform newObj = temp.AddComponent<RectTransform>();
			newObj.SetParent(neighbor.parent);
			newObj.sizeDelta = new Vector2(150, 150);
			newObj.localPosition = neighbor.transform.localPosition + new Vector3(50*(i-(timeStr.Length-1)/2f), -100);
			newObj.localScale = neighbor.localScale*1.727862f;
			Image img = newObj.gameObject.AddComponent<Image>();
			images.Add(img);
			img.sprite = (timeStr[i]==':'?colon:numbers[timeStr[i]-'0']);
		}
	}
}
