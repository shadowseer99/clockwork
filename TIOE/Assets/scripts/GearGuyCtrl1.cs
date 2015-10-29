using System;
using UnityEngine;
using System.Collections.Generic;
namespace UnityStandardAssets._2D
{
    public class GearGuyCtrl1 : MonoBehaviour
    {
        [SerializeField] private float maxSpeed = 4f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField] private float jumpSpeed = 400f;                  // Amount of force added when the player jumps.
		[SerializeField] private GameObject stickyAura;
        [SerializeField] private float launchSpeed = 2f;

        private bool grounded; // Whether or not the player is grounded.
		private bool engaged;
		private float groundDist;
        private Rigidbody rigidBody;
        private bool facingRight = true;  // For determining which way the player is currently facing.

		public Stack<GameObject> gearChildren = new Stack<GameObject>();
        private void Awake()
        {
			groundDist = gameObject.GetComponent<Collider>().bounds.extents.y;
            // Setting up references.
            rigidBody = GetComponent<Rigidbody>();
			stickyAura.transform.localScale = Vector3.zero;
        }

		public void FixedUpdate()
		{
			grounded = Physics.Raycast(transform.position,Vector3.down,groundDist+.1f);
		}


		public void Move(float xrate,float yrate, bool crouch, bool jump)
        {
			// If the player should jump...
			/*
			if (grounded && jump&&transform.parent==null)
			{
				// Add a vertical force to the player.
				grounded = false;
				rigidBody.AddForce(new Vector3(0f, jumpSpeed,0));

			}
			*/
            
			if (transform.parent == null) 
			{
				rigidBody.AddForce(Vector3.right * xrate * maxSpeed);
			}else 
			{
				transform.Rotate(Vector3.back * Time.deltaTime*xrate*200);
				transform.RotateAround(transform.parent.position, Vector3.back, 100 * Time.deltaTime*xrate);
			}

			int i = -1;
            foreach (GameObject go in gearChildren) {
				go.transform.Rotate(rigidBody.angularVelocity*i);
				i*=-1;
			}
            /*
			if (!engaged)
				m_Rigidbody.AddForce (Vector3.right * xrate * m_MaxSpeed);
			else
				transform.Rotate (Vector3.forward * m_MaxSpeed);
            */
        }
		public void engage(bool eng)
		{

			engaged = eng;
			//stickyAura.SetActive (eng);


			if (eng) {
				transform.tag = "StickyAura";
				stickyAura.transform.localScale = new Vector3(.761f,.761f,.761f);
			}
			else
            {
				transform.tag = "Player";
				stickyAura.transform.localScale = Vector3.zero;
            }
        }
        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            facingRight = !facingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
		void OnTriggerEnter(Collider coll)
		{
			if (stickyAura.activeSelf && coll.gameObject.tag == "gear") {
				rigidBody.velocity = Vector3.zero;
                rigidBody.angularVelocity = Vector3.zero;
                transform.parent = coll.gameObject.transform;
				rigidBody.useGravity = false;
			}
		}
        
        void OnTriggerStay(Collider coll)
        {
			if (coll.gameObject.tag == "gear") {
			}
        }
        

        //deparent from gears

        void OnTriggerExit(Collider coll)
        {
            if (coll.gameObject.tag == "gear"&&transform.parent==coll.transform)
            {
				transform.SetParent(null);
				rigidBody.useGravity = true;

				Vector3 diff = transform.position-coll.transform.position;
				Vector3 speed = 2*coll.GetComponent<EnviroGear>().GetVelAtPoint(transform.position);
				print("speed: "+speed+"; magnitude: "+speed.magnitude);
				rigidBody.velocity += diff + speed;
            }   
        }
    }
}
