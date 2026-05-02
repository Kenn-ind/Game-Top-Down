using UnityEngine;

public class MobileInput : MonoBehaviour
{
    public static MobileInput Instance;

    private PlayerAttack _attack;
    private PlayerUlt _ult;
    private SkillController _skillController;
    private movement _movement;
    private PlayerInteraction _interaction;

    public float buttonCooldown = 0.15f;
    private float _lastButtonTime = -999f;

    public GameObject mobileUIPanel;

    [HideInInspector] public Vector2 joystickInput = Vector2.zero;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    void Start()
    {
        _attack = GetComponent<PlayerAttack>();
        _ult = GetComponent<PlayerUlt>();
        _skillController = GetComponent<SkillController>();
        _movement = GetComponent<movement>();
        _interaction = GetComponent<PlayerInteraction>();

        UnityEngine.UI.Button[] allButtons = FindObjectsOfType<UnityEngine.UI.Button>();
        foreach (UnityEngine.UI.Button btn in allButtons)
        {
            UnityEngine.UI.Navigation nav = btn.navigation;
            nav.mode = UnityEngine.UI.Navigation.Mode.None;
            btn.navigation = nav;
        }
    }

    void Update()
    {
        (_skillController?.skill3 as Skill3SO)?.MobileHoldUpdate();
    }

    public void SetMobileUIVisible(bool visible)
    {
        if (mobileUIPanel != null)
            mobileUIPanel.SetActive(visible);
    }

    bool CanTrigger()
    {
        if (Time.time - _lastButtonTime < buttonCooldown) return false;
        _lastButtonTime = Time.time;
        return true;
    }

    public void SetJoystickInput(Vector2 input)
    {
        joystickInput = input;
        _movement?.SetMobileInput(input);
    }

    public void OnDashButton()
    {
        if (!CanTrigger()) return;
        _movement?.MobileDash();
    }

    public void OnAttackButton()
    {
        if (!CanTrigger()) return;
        _attack?.MobileAttack();
    }

    public void OnSkill1Button()
    {
        if (!CanTrigger()) return;
        (_skillController?.skill1 as Skill1SO)?.MobileTrigger();
    }

    public void OnSkill2Button()
    {
        if (!CanTrigger()) return;
        (_skillController?.skill2 as Skill2SO)?.MobileTrigger();
    }

    public void OnSkill3HoldStart()
    {
        (_skillController?.skill3 as Skill3SO)?.MobileHoldStart();
    }

    public void OnSkill3HoldEnd()
    {
        (_skillController?.skill3 as Skill3SO)?.MobileHoldEnd();
    }

    public void OnUltButton()
    {
        Debug.Log("OnUltButton dipanggil!");
        if (!CanTrigger()) return;
        _ult?.MobileUlt();
    }

    public void OnInteractButton()
    {
        if (!CanTrigger()) return;
        _interaction?.MobileInteract();
    }
}