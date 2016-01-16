using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEngine.UI.Button.ButtonClickedEvent buttonDown;
    public UnityEngine.UI.Button.ButtonClickedEvent buttonUp;
    public void OnPointerDown(PointerEventData eventData) { buttonDown.Invoke(); }
    public void OnPointerUp(PointerEventData eventData) { buttonUp.Invoke(); }
}
