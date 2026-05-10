using UnityEngine;
using UnityEngine.EventSystems;


public class MobileJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Referensi")]
    public RectTransform joystickHandle;
    public float joystickRange = 150f;

    private RectTransform _background;
    private Canvas _canvas;
    private Vector2 _bgCenterScreen;
    private Vector2 _inputVector = Vector2.zero;
    private bool _isPressed = false;
    private int _pointerId = -999;

    void Start()
    {
        _background = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        // Update center background tiap frame
        _bgCenterScreen = RectTransformUtility.WorldToScreenPoint(
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
            _background.position
        );

        if (!_isPressed) return;

        // Track pointer yang aktif setiap frame
        // Support mouse (editor) dan touch (mobile)
        Vector2 currentPos = Vector2.zero;
        bool found = false;

        // Cek touch
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.touches[i].fingerId == _pointerId)
            {
                currentPos = Input.touches[i].position;
                found = true;

                // Cek jika touch sudah ended atau canceled
                if (Input.touches[i].phase == TouchPhase.Ended ||
                    Input.touches[i].phase == TouchPhase.Canceled)
                {
                    ResetJoystick();
                    return;
                }
                break;
            }
        }

        // Cek mouse (untuk editor)
        if (!found && _pointerId == -1)
        {
            if (Input.GetMouseButton(0))
            {
                currentPos = Input.mousePosition;
                found = true;
            }
            else
            {
                ResetJoystick();
                return;
            }
        }

        if (!found)
        {
            ResetJoystick();
            return;
        }

        UpdateHandle(currentPos);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _bgCenterScreen = RectTransformUtility.WorldToScreenPoint(
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
            _background.position
        );

        _isPressed = true;
        _pointerId = eventData.pointerId;
        UpdateHandle(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId == _pointerId)
            ResetJoystick();
    }

    void UpdateHandle(Vector2 screenPos)
    {
        Vector2 delta = screenPos - _bgCenterScreen;
        float scale = _canvas.scaleFactor > 0 ? _canvas.scaleFactor : 1f;
        Vector2 localDelta = delta / scale;
        localDelta = Vector2.ClampMagnitude(localDelta, joystickRange);
        joystickHandle.anchoredPosition = localDelta;
        _inputVector = localDelta / joystickRange;
        MobileInput.Instance?.SetJoystickInput(_inputVector);
    }

    void ResetJoystick()
    {
        _isPressed = false;
        _pointerId = -999;
        _inputVector = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;
        MobileInput.Instance?.SetJoystickInput(Vector2.zero);
        Debug.Log("Joystick reset!");
    }

    public Vector2 InputVector => _inputVector;
}