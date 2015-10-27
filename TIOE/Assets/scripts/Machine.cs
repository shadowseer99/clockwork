using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class Machine : MonoBehaviour {
	[SerializeField] private GameObject door;
	[SerializeField] private GameObject topgear;
	[SerializeField] private GameObject frntgear;
	[SerializeField] private GameObject GearDrop;
	[SerializeField] private float rotrate;
	[SerializeField] private float distToMove;
	[SerializeField] private MeshRenderer gearDropMat;
	[SerializeField] private bool activated;
	[SerializeField] private float deviceDuration;
	private Vector3 temp;
	private float tempDevDur;
	// Use this for initialization
	void Start () {
		gearDropMat = GearDrop.GetComponent<MeshRenderer> ();
		tempDevDur = deviceDuration;
		temp = door.transform.position + Vector3.left * distToMove;
		//distToMove = gameObject.GetComponent<Collider> ().bounds.extents.x;
	}
	
	// Update is called once per frame
	void Update () {
		if(activated&&tempDevDur>0)
		{
            //machine execution
			float rate = distToMove/deviceDuration;
			float frameDist = Time.deltaTime*rate;
			topgear.transform.Rotate(rotrate*Vector3.back);
			frntgear.transform.Rotate(rotrate*Vector3.back);
			GearDrop.transform.Rotate(rotrate*Vector3.back);
			tempDevDur-=Time.deltaTime;
			door.transform.Translate(Vector3.left*frameDist);
		}
	
	}
	void OnTriggerEnter(Collider col)
	{
			if (col.gameObject.CompareTag ("Player")) {
				GearGuyCtrl1 gearguy = col.gameObject.GetComponent<GearGuyCtrl1> ();
				if (gearguy.gearChildren.Count > 0 && !activated) {
					GameObject droppedPickup =gearguy.gearChildren.Pop () ;
					gearDropMat.material = droppedPickup.GetComponent<MeshRenderer> ().material;
					GameObject.Destroy(droppedPickup);
					activated=true;
				}
			}
	}
}