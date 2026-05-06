using UnityEngine;
using UnityEngine.EventSystems;


public class MobileJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Referensi")]
    public RectTransform background;
    public float joystickRange = 50f;

    private RectTransform _handle;
    private Vector2 _startPos;
    private Vector2 _inputVector = Vector2.zero;

    void Start()
    {
        _handle = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _startPos = eventData.position;
        _handle.anchoredPosition = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - _startPos;

        float scaleFactor = background.lossyScale.x;
        if (scaleFactor == 0) scaleFactor = 1f;
        Vector2 localDelta = delta / scaleFactor;

        localDelta = Vector2.ClampMagnitude(localDelta, joystickRange);
        _handle.anchoredPosition = localDelta;

        _inputVector = localDelta / joystickRange;
        MobileInput.Instance?.SetJoystickInput(_inputVector);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _inputVector = Vector2.zero;
        _handle.anchoredPosition = Vector2.zero;
        MobileInput.Instance?.SetJoystickInput(Vector2.zero);
    }

    public Vector2 InputVector => _inputVector;
}