using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public Image dis;
	public Sprite ibuttonup;
	public Sprite ibuttondown;
    public UnityEngine.UI.Button.ButtonClickedEvent buttonDown;
    public UnityEngine.UI.Button.ButtonClickedEvent buttonUp;
	public void OnPointerDown(PointerEventData eventData) {
		if (buttonDown!=null) buttonDown.Invoke();
		if (ibuttondown!=null) dis.sprite = ibuttondown;
	}
	public void OnPointerUp(PointerEventData eventData) {
		if (buttonUp!=null) buttonUp.Invoke();
		if (ibuttonup!=null) dis.sprite = ibuttonup;
	}
}
