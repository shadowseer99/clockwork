#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

	public int initMenu=0;
	public GameObject canvas;
	private MenuMover[] menus;
	//public string[] levels;
	private static int curLevel=-1;
	public Texture2D cursor;
    private int loader = 0;
    public GameObject titleCorners;
	public Vector3 titleResize=new Vector3(2, 2, 1);
    private bool corIn = false;
    private bool corOut = false;
    private float strechTime = 0;
    private int activemenu = 0;
    public OptionsMenu option;
    public float cornerXscalar = 1.02;

    // Use this for initialization
    void Start ()
	{
		// find menus
		menus = new MenuMover[canvas.transform.childCount];
		for (int i=0; i<canvas.transform.childCount; ++i)
		{
			GameObject obj = canvas.transform.GetChild(i).gameObject;
			menus[i] = obj.GetComponent<MenuMover>() ?? obj.AddComponent<MenuMover>();
			menus[i].GetComponent<RectTransform>().localScale = Vector3.one*(Screen.height/327f);
		}
		
		// display first menu
		DisplayMenu(initMenu);
        //menus[0].MoveDown();
        for (int i = 0; i < menus[0].transform.childCount; ++i)
        {
            menus[0].transform.GetChild(i).gameObject.SetActive(true);
        }
        corOut = false;
        corIn = true;
        activemenu = 0;
	}

	void Update()
    {
        
        if(corIn)
        {
			if (strechTime <= 0) 
			{
				titleCorners.SetActive (false);
				strechTime += Time.deltaTime;
			}
            else if(strechTime<=1)
            {
				titleCorners.SetActive (true);
                titleCorners.transform.localScale = Vector3.Lerp(titleResize, new Vector3(cornerXscalar, 1, 1), strechTime);
                strechTime += Time.deltaTime;
            }
            else
            {
                corIn = false;
                titleCorners.transform.localScale = new Vector3(cornerXscalar, 1, 1);
            }
        }
        else if(corOut)
        {
            if (strechTime <= 1)
            {
                titleCorners.transform.localScale = Vector3.Lerp(new Vector3(cornerXscalar,1,1), titleResize,  strechTime);
                strechTime += Time.deltaTime;
            }
            else
            {
                corOut = false;
                titleCorners.transform.localScale = titleResize;
            }
        }
        if(activemenu==0)
        {
            if(Input.GetKeyDown(KeyCode.P))
            {
                LoadLevel(1);
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                DisplayMenu(1);
            }
            else if(Input.GetKeyDown(KeyCode.C))
            {
                DisplayMenu(2);
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                option.setSliders();
                DisplayMenu(4);
            }
            else if(Input.GetKeyDown(KeyCode.Q))
            {
                exit();
            }
        }
        else if(activemenu==1||activemenu==2)
        {
            if (Input.GetKeyDown(KeyCode.B)||Input.GetKeyDown(KeyCode.Escape))
            {
                DisplayMenu(0);
            }
        }
        else if(activemenu==4)
        {
            if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Escape))
            {
                option.setSounds();
                DisplayMenu(0);
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                DisplayMenu(0);
            }
        }
    }

	public void HideMenus()
	{
        corOut = true;
        strechTime = 0;
        //menus[0].gameObject.SetActive(false);
        for (int i = 0; i < menus[0].transform.childCount; ++i)
        {
            menus[0].transform.GetChild(i).gameObject.SetActive(false);
        }
		menus[0].transform.GetChild(2).gameObject.SetActive(true);
        for (int i = 1; i < menus.Length; ++i)
        {
            menus[i].gameObject.transform.position = new Vector3(menus[i].gameObject.transform.position.x, 1000, menus[i].gameObject.transform.position.z);
            //menus[i].gameObject.SetActive(false);
            for (int u = 0; u < menus[i].transform.childCount; ++u)
            {
                menus[i].transform.GetChild(u).gameObject.SetActive(false);
            }
        }

		Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
	}

	public void DisplayMenu2(int menu)
	{
		if (curLevel>=0)
			Application.UnloadLevel(curLevel);
		curLevel = -1;
		DisplayMenu(menu);
	}

	public void DisplayMenu(int menu)
	{
        activemenu = menu;
		// set only the appropriate menu active
		HideMenus();
        if(menu==7)
        {
            for (int i = 0; i < menus[menu].transform.childCount; ++i)
            {
                menus[menu].transform.GetChild(i).gameObject.SetActive(true);
            }
            menus[menu].MoveDown();
        }
		else if(menu==1||menu==4||menu==2)
        {
            menus[menu].bannerDrop();
        }
        else if(menu!=0)
        {
            for (int i = 0; i < menus[menu].transform.childCount; ++i)
            {
                menus[menu].transform.GetChild(i).gameObject.SetActive(true);
            }
            menus[menu].transform.localPosition = Vector3.zero;
        }
        else
        {
			menus[menu].bannerDrop();
            //menus[menu].gameObject.SetActive(true);
			/*
            for (int i = 0; i < menus[menu].transform.childCount; ++i)
            {
                menus[menu].transform.GetChild(i).gameObject.SetActive(true);
            }
            */
            corOut = false;
            corIn = true;
            strechTime = -.75f;
        }
        
        this.enabled = true;
		//Cursor.SetCursor(cursor, new Vector2(cursor.width/2, cursor.height/2), CursorMode.ForceSoftware);
		Cursor.SetCursor(cursor, Vector2.zero, CursorMode.ForceSoftware);
	}

	public void Restart()
	{
		if (curLevel>=0)
			LoadLevel(curLevel);
		
#if UNITY_EDITOR
		if (curLevel<0)
			EditorApplication.LoadLevelInPlayMode(EditorApplication.currentScene);
#endif
	}
    public void exit()
    {
        Application.Quit();
    }
	public void LoadLevel(int level=-1)
	{
        DisplayMenu(7);
        loader = level;
        Invoke("loadBackend", 2.5f);
    }
    private void loadBackend()
    {
        int level = loader;
        if (curLevel >= 0)
        {
            Application.UnloadLevel(curLevel);
            // additional clean that unity FAILS to unload
            try { Destroy(GameObject.FindObjectOfType<GearGuyCtrl1>().gameObject); } catch { }
        }

        // load new level OR load victory menu
        if (level == -1)
            level = curLevel + 1;

        print("level: " + level + "; levelCount: " + Application.levelCount);
        if (level < Application.levelCount)
        {
            Application.LoadLevel(level);
            curLevel = level;
        }
        else
        {
            DisplayMenu2(5);
        }


    }

    public void LoadLevelVar(UnityEngine.UI.Text slevel)
    {
        // remove any unnecessary levels/menus
        HideMenus();
        if (curLevel >= 0)
        {
            Application.UnloadLevel(curLevel);
            // additional clean that unity FAILS to unload
            try { Destroy(GameObject.FindObjectOfType<GearGuyCtrl1>().gameObject); } catch { }
        }
        int level = 0;
        // load new level OR load victory menu
        if (slevel.text == null)
            level = curLevel + 1;
        else
            int.TryParse(slevel.text, out level);
        
        if (level < Application.levelCount)
        {
            Application.LoadLevel(level);
            curLevel = level;
        }
        else
        {
            DisplayMenu2(5);
        }
    }
    /// <summary>Returns the MenuManager or loads "Main Menu" to return the MenuManager</summary>
    public static MenuManager GetManager()
	{
		// try finding the manager, load "Main Menu" if necessary
		MenuManager manager = GameObject.FindObjectOfType<MenuManager>();
		if (manager!=null)
			return manager;
		Application.LoadLevelAdditive(0);
		return GameObject.FindObjectOfType<MenuManager>();
	}
}
