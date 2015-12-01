using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

	public GameObject canvas;
	private GameObject[] menus;
	public string[] levels;
	private static int curLevel=-1;
	public Texture2D cursor;

	// Use this for initialization
	void Start ()
	{
		// find menus
		menus = new GameObject[canvas.transform.childCount];
		for (int i=0; i<canvas.transform.childCount; ++i)
		{
			menus[i] = canvas.transform.GetChild(i).gameObject;
			menus[i].GetComponent<RectTransform>().localScale = Vector3.one*(Screen.height/327f);
		}
		
		// display first menu
		DisplayMenu(0);
	}

	public void HideMenus()
	{
		for (int i=0; i<menus.Length; ++i)
			menus[i].SetActive(false);
		Behaviour[] behaviors = GetComponents<Behaviour>();
		for (int i=0; i<behaviors.Length; ++i)
			behaviors[i].enabled = false;
		this.enabled = true;
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
	}

	public void DisplayMenu2(int menu)
	{
		if (curLevel>=0)
			Application.UnloadLevel(levels[curLevel]);
		curLevel = -1;
		DisplayMenu(menu);
	}

	public void DisplayMenu(int menu)
	{
		// set only the appropriate menu active
		HideMenus();
		menus[menu].SetActive(true);
		Behaviour[] behaviors = GetComponents<Behaviour>();
		for (int i=0; i<behaviors.Length; ++i)
			behaviors[i].enabled = true;
		this.enabled = true;
		Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
	}

	public void LoadLevel(int level=-1)
	{
		// remove any unnecessary levels/menus
		HideMenus();
		if (curLevel>=0)
		{
			Application.UnloadLevel(levels[curLevel]);
			// additional clean that unity FAILS to unload
			try { Destroy(GameObject.FindObjectOfType<GearGuyCtrl1>().gameObject); } catch { }
		}
		
		// load new level OR load victory menu
		if (level==-1)
			level = curLevel+1;
		if (level<levels.Length)
		{
			Application.LoadLevelAdditive(levels[level]);
			curLevel = level;
		} else
		{
			DisplayMenu2(5);
		}
	}
}
