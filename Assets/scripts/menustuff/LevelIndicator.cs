using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelIndicator : MonoBehaviour {

    public Sprite[] backings;
    public Sprite[] solonum;
    public Sprite[] ones;
    public Sprite[] tens;
    public GameObject first;
    public GameObject second;
    private Image tile;
    private Image one;
    private Image ten;


    // Use this for initialization
    void OnLevelWasLoaded (int level)
    {
        tile = gameObject.GetComponent<Image>();
        one=first.GetComponent<Image>();
        ten=second.GetComponent<Image>();
        if(level>=1&&level<=15)
        {
            tile.sprite = backings[0];
        }
        else if (level >= 16 && level <= 30)
        {
            tile.sprite = backings[1];
        }
        else if(level >= 31 && level <= 45)
        {
            tile.sprite = backings[2];
        }
        else if (level >= 46 && level <= 50)
        {
            tile.sprite = backings[3];
        }

        if(level<10)
        {
            one.sprite = solonum[level];
            one.color = Color.white;
        }
        else
        {
            one.color = Color.white;
            ten.color = Color.white;
            int a = level / 10;
            int b = level % 10;
            one.sprite = ones[b];
            ten.sprite = tens[a];

        }



    }
	
	
}
