using UnityEngine;
using System.Collections;

public class bannerdrop : MonoBehaviour {
    private bool drop = false;
    private float speed = 25;
	// Use this for initialization
	void Start () {
	
	}
	public void droptrigger()
    {
        drop = true;
    }
	// Update is called once per frame
	void Update ()
    {
	    if(drop&&transform.position.y>0)
        {
            transform.Translate(0,speed*Time.deltaTime, 0);
            
        }
	}
}
