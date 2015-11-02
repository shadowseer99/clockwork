using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

	public GameObject canvas;
	private GameObject[] menus;
	public string[] levels;
	private int curLevel=-1;

	// Use this for initialization
	void Start ()
	{
		// find menus
		menus = new GameObject[canvas.transform.childCount];
		for (int i=0; i<canvas.transform.childCount; ++i)
			menus[i] = canvas.transform.GetChild(i).gameObject;

		// display first menu
		DisplayMenu(0);
	}

	public void HideMenus()
	{
		for (int i=0; i<menus.Length; ++i)
			menus[i].SetActive(false);
		MonoBehaviour[] components = transform.gameObject.GetComponents<MonoBehaviour>();
		for (int i=0; i<components.Length; ++i)
			components[i].enabled = false;
		this.enabled = true;
	}

	public void DisplayMenu(int menu)
	{
		// set only the appropriate menu active
		HideMenus();
		menus[menu].SetActive(true);
		MonoBehaviour[] components = transform.gameObject.GetComponents<MonoBehaviour>();
		for (int i=0; i<components.Length; ++i)
			components[i].enabled = true;
	}

	public void LoadLevel(int level=-1)
	{
		// remove any unnecessary levels/menus
		UnloadLevel();
		HideMenus();
		
		// load new level
		if (level==-1)
			level = curLevel+1;
		if (curLevel<levels.Length)
		{
			Application.LoadLevelAdditive(levels[level]);
			curLevel = level;
		} else
		{
			Application.LoadLevel("Demo");
		}
	}

	public void UnloadLevel()
	{
		if (curLevel>=0)
			Application.UnloadLevel(levels[curLevel]);
		curLevel = -1;
	}
}
