using UnityEngine;
using System.Collections;

public class MenuPan : MonoBehaviour {

    public int ymin = -174;
    public int ymax = 155;
    public int xmin = -204;
    public int xmax = 214;
    public float speed = 1;
    private Vector3 direction = new Vector3(1, 1).normalized;
	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        this.transform.Translate(direction * speed * Time.deltaTime);
        if(transform.position.x>xmax||transform.position.x<xmin)
        {
            direction.x=direction.x * -1;
        }
        if (transform.position.y > ymax || transform.position.y < ymin)
        {
            direction.y = direction.y * -1;
        }
    }
}
