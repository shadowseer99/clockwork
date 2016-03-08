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
    private float timer=0;
	private List<Image> images=new List<Image>();
	public Sprite[] numbers;
	public RectTransform timeObject;

    private bool activated = false;
    
    // Use this for initialization

    // Update is called once per frame
	void Start()
	{
		levelend.SetActive (false);

		// find all images, levelend.GetComponentsInChildren<Image>() wasn't working for some reason
		Stack<Transform> todo = new Stack<Transform>();
		todo.Push(levelend.transform);
		while (todo.Count>0) {
			Transform t = todo.Pop();
			for (int i=0; i<t.childCount; ++i)
				todo.Push(t.GetChild(i));
			Image img = t.GetComponent<Image>();
			if (img!=null)
				images.Add(img);
		}
	}
    void Update()
    {
		// handle fade
		if (levelend.activeSelf)
		{
			timer = Mathf.Min(timer+Time.unscaledDeltaTime, fadeTime);
			for (int i=0; i<images.Count; ++i)
			{
				Color c = images[i].color;
				images[i].color = new Color(c.r, c.g, c.b, timer/fadeTime);
			}
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
	public void PrintText() { print("Text"); }
	public void NextLevel()
	{
		print("next level...");
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
		levelend.SetActive (true);

		string timeStr = Mathf.RoundToInt(Time.timeSinceLevelLoad).ToString();
		timeStr = "129";
		float offset=0;
		for (int i=0; i<timeStr.Length; ++i) {
			GameObject temp = new GameObject("");
			temp.transform.parent = timeObject;
			temp.AddComponent<RectTransform>();
			RectTransform newObj = temp.transform as RectTransform;
			Image img = newObj.gameObject.AddComponent<Image>();
			img.sprite = numbers[timeStr[i]-'0'];
			newObj.localPosition = Vector3.zero;
			newObj.position += Vector3.right*offset;
			offset += img.preferredWidth;
			print("width1: "+img.flexibleWidth+", width2: "+img.minWidth+", width3: "+img.preferredWidth);
		}
    }
}
