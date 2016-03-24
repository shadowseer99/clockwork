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
	public void OnPointerDown(PointerEventData eventData) { buttonDown.Invoke();dis.sprite = ibuttondown; }
	public void OnPointerUp(PointerEventData eventData) { buttonUp.Invoke(); dis.sprite = ibuttonup;}
}
