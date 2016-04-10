using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuMover : MonoBehaviour {
	public float duration=0.5f;
	public float upDelayRatio=0;
	public float downDelayRatio=0;
	public float additionalMoveUp=0;
	public float pos;
	public float max { get { return 1+downDelayRatio; } }
	public float min { get { return -upDelayRatio; } }
	public bool stretchThis=false;

	private bool hasStarted=false;
	private Vector3 upPos;
	private Vector3 downPos;
	private int dir=0;
    private bool drop = false;
    private bool fade = false;
    private float fader = 0;

	public void Start() {
		if (!hasStarted) {
			hasStarted = true;
			duration *= 1+additionalMoveUp;
			downDelayRatio -= additionalMoveUp;
			downDelayRatio /= (1+additionalMoveUp);
            //changed
			downPos = new Vector3(transform.position.x, 0,transform.position.z);
			upPos = downPos + (1+additionalMoveUp)*transform.TransformVector(Vector3.up*(transform.root as RectTransform).sizeDelta.y);
			if (stretchThis) {
				RectTransform rt = transform as RectTransform;
				rt.sizeDelta = transform.InverseTransformVector(2*(upPos-downPos));
				print("anchoredPosition: "+(2*downPos-upPos)+", sizeDelta: "+(upPos-downPos));
			}
		}
	}

	public void Update() {
		if (dir==1) {
			pos = Mathf.Min(max, pos+Time.unscaledDeltaTime/duration);
			if (pos==max)
				dir = 0;
			transform.position = Vector3.Lerp(downPos, upPos, pos);
		} else if (dir==-1) {
			pos = Mathf.Max(min, pos-Time.unscaledDeltaTime/duration);
			if (pos==min)
            {
                dir = 0;
                if(drop)
                {
                    fade = true;
                    fader = 0;
                    for (int i = 0; i < transform.childCount; ++i)
                    {
                        transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }
				
			transform.position = Vector3.Lerp(downPos, upPos, pos);
		}
        if(fade)
        {
            if(fader>=1)
            {
                fade = false;
            }
            else
            {
                fader += Time.deltaTime;
                foreach(Image i in transform.GetComponentsInChildren<Image>())
                {
                    i.color = new Color(1, 1, 1, fader);
                }


                transform.GetChild(1).GetComponent<Image>().color = Color.white;
                
            }
            
        }
		if (dir==0&&pos==max)
			gameObject.SetActive(false);
	}

	public void MoveUp() {
		gameObject.SetActive(false);
		//if (!hasStarted) pos = max;
		//Start();
		//dir = 1;
	}
    public void bannerDrop()
    {
        drop = true;
        transform.GetChild(1).gameObject.SetActive(true);
        MoveDown();
    }

	public void MoveDown() {
		//gameObject.SetActive(true);
		pos = max;
		if (!hasStarted) pos = min;
		Start();
		dir = -1;
        gameObject.SetActive(true);
    }

	public void MoveDir(bool up) {
		if (up) MoveUp();
		else MoveDown();
	}
}
