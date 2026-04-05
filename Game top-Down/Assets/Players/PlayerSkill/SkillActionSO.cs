using UnityEngine;

public abstract class SkillActionSO : ScriptableObject
{
    public abstract void Initialize(GameObject player);
    public abstract void Execute();
}