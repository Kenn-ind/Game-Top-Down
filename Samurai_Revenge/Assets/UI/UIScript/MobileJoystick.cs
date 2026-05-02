using UnityEngine;
using UnityEngine.EventSystems;

// ============================================================
//  MobileJoystick.cs
//  Attach ke JoystickHandle.
//
//  SETUP DI UNITY:
//  Canvas
//  └── JoystickBackground
//      └── JoystickHandle  ← attach MobileJoystick.cs di sini
// ============================================================

public class MobileJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Referensi")]
    public RectTransform background;
    public float joystickRange = 50f;

    private RectTransform _handle;
    private Vector2 _startPos;         // posisi screen saat pertama touch
    private Vector2 _inputVector = Vector2.zero;

    void Start()
    {
        _handle = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Simpan posisi screen saat pertama touch
        _startPos = eventData.position;
        _handle.anchoredPosition = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Hitung delta dari posisi awal touch, bukan dari background
        Vector2 delta = eventData.position - _startPos;

        // Konversi delta screen ke local space background
        // (handle skala layar yang berbeda-beda)
        float scaleFactor = background.lossyScale.x;
        if (scaleFactor == 0) scaleFactor = 1f;
        Vector2 localDelta = delta / scaleFactor;

        // Clamp dalam radius
        localDelta = Vector2.ClampMagnitude(localDelta, joystickRange);
        _handle.anchoredPosition = localDelta;

        // Normalize input (-1 sampai 1)
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