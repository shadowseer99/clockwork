using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine.UI;

public class LevelSelectMenu : MonoBehaviour {
	public Sprite locked;
	public Sprite bronze;
	public Sprite silver;
	public Sprite gold;
	public Vector3 medalOffset;
	public Vector3 medalSize=Vector3.one;
	public List<int> _unlockedLevels;
	public List<int> _brokenLevels;
	public List<RectTransform> levels=new List<RectTransform>();
	private static HashSet<int> unlockedLevels;
	private static HashSet<int> brokenLevels;
	private List<RectTransform> locks=new List<RectTransform>();
	private List<RectTransform> medals=new List<RectTransform>();

	// Use this for initialization
	void Start () {
		ResetLocks();
	}

	public void ResetLocks() {
		// create new locks/medals if necessary
		if (unlockedLevels==null || unlockedLevels.Count==0 || locks.Count==0) {
			unlockedLevels = new HashSet<int>(_unlockedLevels);
			brokenLevels = new HashSet<int>(_brokenLevels);
			for (int i=0; i<levels.Count; ++i) {
				RectTransform t = levels[i].GetComponent<RectTransform>();
				RectTransform levelLock = new GameObject("Level "+(i+1)+" Lock").AddComponent<RectTransform>();
				levelLock.SetParent(t.parent);
				levelLock.sizeDelta = t.sizeDelta;
				levelLock.localScale = t.localScale;
				levelLock.localPosition = t.localPosition;
				levelLock.SetParent(t);
				Image img = levelLock.gameObject.AddComponent<Image>();
				img.sprite = locked;
				locks.Add(levelLock);

				RectTransform medal = new GameObject("Level "+(i+1)+" Medal").AddComponent<RectTransform>();
				medal.SetParent(t.parent);
				medal.sizeDelta = t.sizeDelta/2;
				medal.localScale = medalSize;
				medal.localPosition = t.localPosition + medalOffset;
				medal.SetParent(t);
				medal.gameObject.AddComponent<Image>();
				medals.Add(medal);
				medal.gameObject.SetActive(false);
			}
		}

		// lock all levels
		for (int i=0; i<levels.Count; ++i)
			UnlockLevel(i, false);

		// unlock preselected levels
		for (int i=0; i<levels.Count; ++i) {
			if (unlockedLevels.Contains(i+1)) {
				for (int remaining=1; i<levels.Count&&remaining>0; ++i) {
					if (unlockedLevels.Contains(i+2)) ++remaining;
					if (brokenLevels.Contains(i+1)) continue;
					if (PlayerPrefs.HasKey("Level "+(i+1))) ++remaining;
					UnlockLevel(i, true);
					--remaining;
				}
			}
		}

		// unlock special levels
		bool l46=true, l48=true, l49=true;
		for (int i=0; i<15; ++i) {
			l46 = l46 && PlayerPrefs.HasKey("Level "+(i+1));
			l48 = l48 && PlayerPrefs.HasKey("Level "+(i+16));
			l49 = l49 && PlayerPrefs.HasKey("Level "+(i+31));
		}
		UnlockLevel(45, l46);
		UnlockLevel(46, PlayerPrefs.HasKey("Level 46"));
		UnlockLevel(47, l48);
		UnlockLevel(48, l49);
		UnlockLevel(49, PlayerPrefs.HasKey("Level 47") && PlayerPrefs.HasKey("Level 48") && PlayerPrefs.HasKey("Level 49"));

		// add medals
		for (int i=0; i<levels.Count; ++i) {
			if (!locks[i].gameObject.activeSelf) {
				float time = PlayerPrefs.GetFloat("Level "+(i+1));
				float bronze = PlayerPrefs.GetFloat("Level "+(i+1)+" Bronze");
				float silver = PlayerPrefs.GetFloat("Level "+(i+1)+" Silver");
				float gold = PlayerPrefs.GetFloat("Level "+(i+1)+" Gold");
				if (time!=0) medals[i].gameObject.SetActive(true);
				if (time>bronze || time==0) medals[i].gameObject.SetActive(false);
				if (time<=bronze && time!=0) medals[i].GetComponent<Image>().sprite = this.bronze;
				if (time<=silver && time!=0) medals[i].GetComponent<Image>().sprite = this.silver;
				if (time<=gold && time!=0) medals[i].GetComponent<Image>().sprite = this.gold;
			}
		}
	}

	private void UnlockLevel(int level, bool unlock) {
		if (brokenLevels.Contains(level+1)) {
			unlock = false;
			locks[level].GetComponent<Image>().color = new Color(1, 0, 0);
		}
		levels[level].GetComponent<UnityEngine.UI.Button>().enabled = unlock;
		locks[level].gameObject.SetActive(!unlock);
	}
}
