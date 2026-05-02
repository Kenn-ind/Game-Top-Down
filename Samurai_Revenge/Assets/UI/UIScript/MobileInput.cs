using UnityEngine;


public class MobileInput : MonoBehaviour
{
    public static MobileInput Instance;

    private PlayerAttack _attack;
    private PlayerUlt _ult;
    private SkillController _skillController;
    private movement _movement;
    private PlayerInteraction _interaction;

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
    }

    public void SetJoystickInput(Vector2 input)
    {
        joystickInput = input;
        _movement?.SetMobileInput(input);
    }

    public void OnDashButton()
    {
        _movement?.MobileDash();
    }

    public void OnAttackButton()
    {
        _attack?.MobileAttack();
    }

    public void OnSkill1Button()
    {
        if (_skillController?.skill1 != null)
            (_skillController.skill1 as Skill1SO)?.MobileTrigger();
    }

    public void OnSkill2Button()
    {
        if (_skillController?.skill2 != null)
            (_skillController.skill2 as Skill2SO)?.MobileTrigger();
    }

    public void OnSkill3TapButton()
    {
        if (_skillController?.skill3 != null)
            (_skillController.skill3 as Skill3SO)?.MobileTriggerTap();
    }

    public void OnSkill3HoldStart()
    {
        if (_skillController?.skill3 != null)
            (_skillController.skill3 as Skill3SO)?.MobileHoldStart();
    }

    public void OnSkill3HoldEnd()
    {
        if (_skillController?.skill3 != null)
            (_skillController.skill3 as Skill3SO)?.MobileHoldEnd();
    }

    public void OnUltButton()
    {
        _ult?.MobileUlt();
    }

    public void OnInteractButton()
    {
        _interaction?.MobileInteract();
    }
}