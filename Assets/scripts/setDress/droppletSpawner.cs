using UnityEngine;
using System.Collections;

public class droppletSpawner : MonoBehaviour {

    public GameObject dropplet;
    public int frequency = 5;
    private float time = 0;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        time += Time.deltaTime;
        if(Random.Range(0,1000)<time*frequency)
        {
            time = 0;
            Instantiate(dropplet, transform.position, Quaternion.identity);
        }
	}
}
