using UnityEngine;

public class SkillController : MonoBehaviour
{
    [Header("Skills")]
    public SkillSO skill1;
    public SkillSO skill2;
    public SkillSO skill3;

    void Start()
    {
        skill1?.Initialize(gameObject);
        skill2?.Initialize(gameObject);
        skill3?.Initialize(gameObject);
    }

    void Update()
    {
        skill1?.OnUpdate();
        skill2?.OnUpdate();
        skill3?.OnUpdate();
    }
}