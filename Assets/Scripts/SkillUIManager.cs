using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoBehaviour
{
    private CombatManager _combatManager;

    [Header("Skill Initialization")]
    [SerializeField] private SkillData _relicPassiveSkill;
    [SerializeField] private SkillData _relicBasicSkill;
    [SerializeField] private SkillData _relicPrimarySkill;
    [SerializeField] private SkillData _relicSecondarySkill;
    [SerializeField] private SkillData _relicAlternateSkill;
    [SerializeField] private SkillData _relicUltimateSkill;

    [Header("Skill UI")]
    public Selector relicPassiveSelect;
    public Selector relicBasicSelect;
    public Selector relicPrimarySelect;
    public Selector relicSecondarySelect;
    public Selector relicAlternateSelect;
    public Selector relicUltimateSelect;

    private void Awake()
    {
        _combatManager = FindObjectOfType<CombatManager>();
    }

    public void AssignActiveSkill(Button button)
    {
        _combatManager.relicActiveSkill = button.transform.parent.parent.GetComponent<Selector>().skillData;  // Assign the selected skill
    }

    public void AssignSkillAesthetics(Image skillIcon, Image skillSelectionIcon, Image skillBorderIcon, 
        Color skillIconColour, Color skillSelectionColour, Color skillBorderColour)
    {
        skillIcon.color = skillIconColour;
        skillSelectionIcon.color = skillSelectionColour;
        skillBorderIcon.color = skillBorderColour;
    }

    void AssignSkills()
    {
        _relicPassiveSkill = _combatManager.activeRelic.passiveSkill;
        _relicBasicSkill = _combatManager.activeRelic.basicSkill;
        _relicPrimarySkill = _combatManager.activeRelic.primarySkill;
        _relicSecondarySkill = _combatManager.activeRelic.secondarySkill;
        _relicAlternateSkill = _combatManager.activeRelic.alternateSkill;
        _relicUltimateSkill = _combatManager.activeRelic.ultimateSkill;

        relicPassiveSelect.skillData = _combatManager.activeRelic.passiveSkill;
        relicBasicSelect.skillData = _combatManager.activeRelic.basicSkill;
        relicPrimarySelect.skillData = _combatManager.activeRelic.primarySkill;
        relicSecondarySelect.skillData = _combatManager.activeRelic.secondarySkill;
        relicAlternateSelect.skillData = _combatManager.activeRelic.alternateSkill;
        relicUltimateSelect.skillData = _combatManager.activeRelic.ultimateSkill;
    }
    void UpdateSkillAesthetics()
    {
        relicPassiveSelect.AssignSkillUIAesthetics();
        relicBasicSelect.AssignSkillUIAesthetics();
        relicPrimarySelect.AssignSkillUIAesthetics();
        relicSecondarySelect.AssignSkillUIAesthetics();
        relicAlternateSelect.AssignSkillUIAesthetics();
        relicUltimateSelect.AssignSkillUIAesthetics();
    }

    public void InitializeSkills()
    {
        AssignSkills();
        UpdateSkillAesthetics();
    }
}

