using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof (GearGuyCtrl1))]
public class GearGuyInputs1 : MonoBehaviour
{
	private GearGuyCtrl1 m_Character;
    private bool m_Jump;


    private void Awake()
    {
		m_Character = GetComponent<GearGuyCtrl1>();
    }


    


    private void FixedUpdate()
    {
#if UNITY_IPHONE || UNITY_ANDROID
        bool crouch = Mobile.engage;
        float x = 0;
        if (Mobile.left)
            x -= 1;
        if (Mobile.right)
            x += 1;
#else
        // Read the inputs.
        bool crouch = CrossPlatformInputManager.GetAxisRaw("Jump")==1;
        float x = CrossPlatformInputManager.GetAxisRaw("Horizontal");
#endif		


        // Pass all parameters to the character control script.
        m_Character.Move(x, 0, crouch, m_Jump);
		m_Character.engage (crouch);
    }

}
