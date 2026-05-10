using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickHandleForwarder : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public MobileJoystick joystick;

    public void OnPointerDown(PointerEventData eventData)
    {
        joystick?.OnPointerDown(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystick?.OnPointerUp(eventData);
    }
}