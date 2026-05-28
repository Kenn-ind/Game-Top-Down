using System.Collections.Generic;
using UnityEngine;

public enum TutorialActionType
{
    MeleeAttack,
    RangeAttack,
    Skill1Range,
    Skill1Melee,
    Skill2Range,
    Skill2Melee,
    Skill3Melee,
    Skill3Range,
    Ult
}

[CreateAssetMenu(fileName = "TutorialStep", menuName = "Tutorial/TutorialStep")]
public class TutorialStep : ScriptableObject
{
    public string title;
    public TutorialActionType requiredAction;
    public int requiredCount = 1;

    [Header("Desc Panel Pages")]
    public List<TutorialDescPage> descPages;
}