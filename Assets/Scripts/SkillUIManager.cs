using System.Collections;
using System.Collections.Generic;
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
    [Space(5)]
    [Header("Skill UI")]
    [Space(2)]
    [Header("Initialization")]
    public Selector relicPassiveSelect;
    public Selector relicBasicSelect;
    public Selector relicPrimarySelect;
    public Selector relicSecondarySelect;
    public Selector relicAlternateSelect;
    public Selector relicUltimateSelect;
    [Space(2)]
    [Tooltip("Text GameObject to instantiate")]
    [SerializeField] private GameObject skillUIText;
    [Tooltip("The speed at which skill UI values travel upwards")]
    public float panSpeedUI;
    [Tooltip("How long the text remains on screen before destroying")]
    public float textLifeLength;
    [Tooltip("The difference in Y value between each skill UI value has")]
    public float yDistBetweenUI;
    public float xDisplacementStr;
    public float SkillUIFadeOutSpeed;

    [Header("Active Stored Attacks")]
    public AttackData activeAttackData;

    private void Awake()
    {
        _combatManager = FindObjectOfType<CombatManager>();
    }

    public void AssignActiveSkill(Button button)
    {
        _combatManager.relicActiveSkill = button.transform.parent.parent.GetComponent<Selector>().skillData;  // Assign the selected skill
    }

    public void AssignFirstSkill(SkillData skillData)
    {
        _combatManager.relicActiveSkill = skillData;
    }
    public void AssignSkillAesthetics(Image skillIcon, Image skillSelectionIcon, Image skillBorderIcon,
        Color skillIconColour, Color skillSelectionColour, Color skillBorderColour)
    {
        skillIcon.color = skillIconColour;
        skillSelectionIcon.color = skillSelectionColour;
        skillBorderIcon.color = skillBorderColour;
    }

    public void InitializeSkills()
    {
        AssignSkills();
        UpdateSkillAesthetics();
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

    public void SetActiveAttackData(AttackData attackData)
    {
        activeAttackData = attackData;
    }

    public void DisplaySkillValue(Unit caster, AttackData attackData, Transform skillUIValueParent, int val, SkillData skillData, int curHitsCompleted)
    {
        GameObject go = Instantiate(skillUIText, skillUIValueParent.position, Quaternion.identity);    // Initialize
        SkillValueUI skillValueUI = go.GetComponent<SkillValueUI>();
        go.GetComponent<SkillValueUI>().skillUIManager = this;  // Initialization
        go.transform.SetParent(skillUIValueParent);    // Set parent

        float yVal = yDistBetweenUI * caster.hitWaveCount;
        Vector3 pos = go.transform.localPosition;
        pos.y = go.transform.localPosition.y + yVal;
        pos.x = Random.Range(-xDisplacementStr, xDisplacementStr);
        go.transform.localPosition = pos;

        skillValueUI.canvas.sortingOrder = caster.hitWaveCount;

        Text text = go.GetComponent<Text>();    // Initalization
        if (val != 0)
            text.text = Mathf.Abs(val).ToString();  // Display damage
                                                    // If unit missed the skill
        else
            text.text = "Miss";   // Display miss text

        if (caster.hitCount == attackData.curTargetCount * attackData.hitsRequired)
        {
            caster.hitWaveCount = 0;
            caster.hitCount = 0;
        }        
    }

    public void SetSkillCooldown(SkillData skillData)
    {
        skillData.curCooldown = skillData.turnCooldown;
    }

    public void UpdateSkillCooldown(SkillData skillData, Selector selector)
    {
        selector.cooldownImage.fillAmount = skillData.curCooldown / skillData.turnCooldown;

        if (skillData.turnCooldown == 0)
            selector.cooldownText.text = "";
        else
            selector.cooldownText.text = skillData.curCooldown.ToString();
    }

    private int RoundFloatToInt(float f)
    {
        return Mathf.RoundToInt(f);
    }
}

