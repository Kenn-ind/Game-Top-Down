using UnityEngine;

public enum TutorialActionType
{
    MeleeAttack,
    RangeAttack,
    Skill1Range,
    Skill1Melee,
    Skill2Range,
    Skill2Melee,
    Skill3Range,
    Skill3Melee,
    Ult
}

[CreateAssetMenu(fileName = "TutorialStep", menuName = "Tutorial/TutorialStep")]
public class TutorialStep : ScriptableObject
{
    public string title;
    [TextArea] public string description;
    public TutorialActionType requiredAction;
    public int requiredCount = 1;
}