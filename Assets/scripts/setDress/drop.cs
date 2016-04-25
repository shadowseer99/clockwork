using UnityEngine;
using System.Collections;

public class drop : MonoBehaviour {
    public GameObject ani;
    public SpriteRenderer spr;
    public Rigidbody2D body;
    private bool hit = false;
    private float time = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(hit)
        {
            time += Time.deltaTime;
            if(time>.333)
            {
               Destroy(gameObject);
            }
        }
	}
    void OnCollisionEnter2D()
    {
        ani.SetActive(true);
        spr.enabled = false;
        body.gravityScale = 0;
        hit = true;
    }
}
