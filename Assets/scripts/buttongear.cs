using UnityEngine;
using System.Collections;

public class buttongear : MonoBehaviour {

    public GameObject gear;
    private Animator ani;
	// Use this for initialization
	void Start () {
        ani = gear.GetComponent<Animator>();
	}
	
	public void Enter()
    {
        ani.SetBool("State", true);
    }
    public void Exit()
    {
        ani.SetBool("State", false);
    }
}
