using UnityEngine;
using System.Collections;

public class glowfade : MonoBehaviour {
    public shaderGlow aura;
    private GameObject player;

	// Use this for initialization
	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update ()
    {
        aura.glowOpacity = (10 - Vector3.Distance(transform.position, player.transform.position)) / 5;
	}
}
