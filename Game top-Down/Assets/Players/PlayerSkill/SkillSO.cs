using UnityEngine;

public abstract class SkillSO : ScriptableObject
{
    [Header("Input")]
    public KeyCode keyBind;

    [Header("Cost & Cooldown")]
    public float cooldown;
    public int staminaCost;

    [Header("Range Detection")]
    public float closeRange;
    public float detectRadius;

    public abstract void Initialize(GameObject player);

    public abstract void OnUpdate();
}