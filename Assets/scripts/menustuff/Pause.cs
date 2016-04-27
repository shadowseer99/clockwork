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
	public RectTransform bestTimeObject;
	private Vector3 bestTimePos;
	public float bronzeTime=180;
	public float silverTime=120;
	public float goldTime=60;
    private bool activated = false;
    private bool dropping = false;
    private float timedelay=0;
    private Image[] faders;
	public Image bronzeImage;
	public Image silverImage;
	public Image goldImage;
	private Vector3 bronzePos;
	private Vector3 silverPos;
	private Vector3 goldPos;
	public float rotationSpeed=360;
	public float coinDelayTime;
	public float coinFadeTime;
	public float coinStaggerTime;
	public float coinFallHeight=300;
	bool hasEnded=false;
	public Vector3 timeOffset=230*Vector3.right;
    private bool speedup = false;
    
    // Use this for initialization

    // Update is called once per frame
	void Start()
	{
		// handle bronze/silver/gold
		bronzeImage.gameObject.SetActive(false);
		silverImage.gameObject.SetActive(false);
		goldImage.gameObject.SetActive(false);

		levelend.SetActive (false);
		timer -= fadeDelay;
		bestTimePos = bestTimeObject.position;
		bronzePos = bronzeImage.transform.position;
		silverPos = silverImage.transform.position;
		goldPos = goldImage.transform.position;

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
        if(dropping)
        {
            Cursor.visible = true;
            if(speedup)
            {
                if (endBanner.transform.position.y > 0)
                    endBanner.transform.Translate(0, -1000 * Time.unscaledDeltaTime, 0);
                timer = Mathf.Min(timer + Time.unscaledDeltaTime*2, fadeTime + coinDelayTime + 2 * coinStaggerTime + coinFadeTime);
            }
            else
            {
                if (endBanner.transform.position.y > 0)
                    endBanner.transform.Translate(0, -500 * Time.unscaledDeltaTime, 0);
                timer = Mathf.Min(timer + Time.unscaledDeltaTime, fadeTime + coinDelayTime + 2 * coinStaggerTime + coinFadeTime);
                if(Input.GetMouseButtonDown(0))
                {
                    speedup = true;
                }
            }

			
            if(timer < fadeTime)
            {
                // handle fade
                if (timer < 0)
                {
                    for (int i = 0; i < images.Count; ++i)
                    {
                        Color c = images[i].color;
                        images[i].color = new Color(c.r, c.g, c.b, 0);
                    }
                }
                else if (timer < fadeTime)
                {
                    for (int i = 0; i < images.Count; ++i)
                    {
                        Color c = images[i].color;
                        images[i].color = new Color(c.r, c.g, c.b, Mathf.Max(timer / fadeTime, 0));
                    }
                }
            }

			// handle bronze/silver/gold
			if (timeSpent<=bronzeTime&&timer>.5) bronzeImage.gameObject.SetActive(true);
			if (timeSpent<=silverTime&&timer>1) silverImage.gameObject.SetActive(true);
			if (timeSpent<=goldTime&&timer>1.5) goldImage.gameObject.SetActive(true);



			//goldImage.color = new Color(goldImage.color.r, goldImage.color.g, goldImage.color.b,
				//Mathf.Min((timer-fadeTime-coinDelayTime-2*coinStaggerTime)/coinFadeTime, 1));
            
        }
        else if(!dropping)
        {
            timeSpent += Time.deltaTime;

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
                    Cursor.visible = true;
                }
                else
                {
#if UNITY_IPHONE || UNITY_ANDROID
            
                Mobile.active = true;
#endif
                    Time.timeScale = 1;
                    activated = false;
                    Cursor.visible = false;
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
		if (hasEnded)
			return;
		hasEnded = true;

#if UNITY_IPHONE || UNITY_ANDROID
        Mobile.menu = false;
        Mobile.active = false;
#endif
        Time.timeScale = 0;
        dropping = true;
        timedelay = Time.realtimeSinceStartup;
        levelend.SetActive(true);
        faders = levelend.GetComponentsInChildren<Image>();

		// save and print results
		string level = "Level "+Application.loadedLevel;
		PlayerPrefs.SetFloat(level, PlayerPrefs.HasKey(level)?Mathf.Min(PlayerPrefs.GetFloat(level), timeSpent):timeSpent);
		PlayerPrefs.SetFloat(level+" Bronze", bronzeTime);
		PlayerPrefs.SetFloat(level+" Silver", silverTime);
		PlayerPrefs.SetFloat(level+" Gold", goldTime);
		PlayerPrefs.Save();
		AddNumber(timeObject, timeSpent, true);
		AddNumber(bestTimeObject, PlayerPrefs.GetFloat(level), true);


    }

	public void AddNumber(RectTransform neighbor, float number, bool isTime) {
		int time = Mathf.RoundToInt(number);
		string timeStr = (isTime
			?(time/60).ToString().PadLeft(1, '0')+":"+(time%60).ToString().PadLeft(2, '0')
			:time.ToString());
		
		for (int i=0; i<timeStr.Length; ++i) {
			GameObject temp = new GameObject("Number "+(timeStr[i]-'0'));
			RectTransform newObj = temp.AddComponent<RectTransform>();
			newObj.SetParent(neighbor.parent);
			newObj.sizeDelta = new Vector2(150, 150);
			newObj.localPosition = neighbor.transform.localPosition + Vector3.right*50*(i-(timeStr.Length-1)/2f) + timeOffset;
			newObj.localScale = neighbor.localScale*1.727862f;
			Image img = newObj.gameObject.AddComponent<Image>();
			images.Add(img);
			img.sprite = (timeStr[i]==':'?colon:numbers[timeStr[i]-'0']);
			images.Add(img);
		}
	}
}
