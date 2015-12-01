using UnityEngine;
using System.Collections;

public class WaterWheel : EnviroGear {
	
	private Water inwater;
	private Collider waterColl;

	public override void Start ()
	{
		base.Start();
		inwater = Water.nullWater;
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		
		//print("using water: "+inwater.name);
		Vector3 contact = inwater.GetContactPoint(transform);
		Vector3 diff = contact-transform.position;
		Vector3 cross = Vector3.Cross(diff, inwater.flow);
		angularMomentum += inwater.thicknessRatio*cross.magnitude*(cross.z>0?1:-1);
	}

	public override void OnTriggerEnter(Collider coll)
	{
		base.OnTriggerEnter(coll);

		Water tempWater = coll.gameObject.GetComponent<Water>();
		if (inwater==Water.nullWater && tempWater!=null)
			inwater = tempWater;
	}

	public override void OnTriggerExit(Collider coll)
	{
		base.OnTriggerExit(coll);

		if (inwater!=Water.nullWater && inwater==coll.gameObject.GetComponent<Water>())
			inwater = Water.nullWater;
	}
}
