using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof (GearGuyCtrl1))]
    public class GearGuyInputs1 : MonoBehaviour
    {
		private GearGuyCtrl1 m_Character;
        private bool m_Jump;


        private void Awake()
        {
			m_Character = GetComponent<GearGuyCtrl1>();
        }


        private void Update()
        {
            if (!m_Jump) {
				// Read the jump input in Update so button presses aren't missed.
				m_Jump = CrossPlatformInputManager.GetButtonDown ("Jump");
			}
        }


        private void FixedUpdate()
        {
            // Read the inputs.
            bool crouch = Input.GetKey(KeyCode.LeftShift);
            float x = CrossPlatformInputManager.GetAxis("Horizontal");
			float y = CrossPlatformInputManager.GetAxis("Vertical");
			//bool engaged = CrossPlatformInputManager.GetAxis ("Fire1");
            // Pass all parameters to the character control script.
            m_Character.Move(x,y, crouch, m_Jump);
			m_Character.engage (crouch);
            m_Jump = false;
        }

    }
}
