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
    public float yDistBetweenEffectUI;
    public float xDisplacementStr;
    public float SkillUIFadeOutSpeed;


    [Header("Active Stored Attacks")]
    public AttackData activeAttackData;

    private GameObject prevGo;
    private Transform prevskillUIValueParent;
    private Unit unit;

    private void Awake()
    {
        _combatManager = FindObjectOfType<CombatManager>();
    }

    public void AssignActiveSkill(Button button)
    {
        if (!_combatManager.relicTurn)
            return;

        SkillData skillData = button.transform.parent.parent.GetComponent<Selector>().skillData;

        // If the skill is not activatable, don't continue
        if (!skillData.activatable || skillData.onCooldown)
            return;

        _combatManager.relicActiveSkill = skillData;  // Assign the selected skill

        _combatManager.activeAttackBar.MoveAttackBar(true);
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

    public void DisplaySkillValue(Unit caster, Transform skillUIValueParent, float val = 0, float effectVal = 0, bool effectInfict = false, AttackData attackData = null)
    {
        GameObject go = Instantiate(skillUIText, skillUIValueParent.position, Quaternion.identity);    // Initialize
        if (effectInfict && effectVal != 0)
        {
            unit = skillUIValueParent.parent.GetComponent<Unit>();  // Initialize
            unit.effectHitCount++;
        }

        SkillValueUI skillValueUI = go.GetComponent<SkillValueUI>();
        go.GetComponent<SkillValueUI>().skillUIManager = this;  // Initialization
        go.transform.SetParent(skillUIValueParent);    // Set parent
        prevskillUIValueParent = skillUIValueParent;

        Vector3 pos = go.transform.localPosition;

        pos.x = Random.Range(-xDisplacementStr, xDisplacementStr);


        Text text = go.GetComponent<Text>();    // Initalization

        if (effectInfict)
        {
            if (effectVal != 0)
            {
                text.text = Mathf.Abs(effectVal).ToString();  // Display damage

                // Set font size
                if (attackData.relicActiveSkillValueModifier == attackData.skillData.perfectValueMultiplier)
                    text.fontSize = _combatManager.activeAttackBar.skillUIPerfectFontSize;
                else
                    text.fontSize = _combatManager.activeAttackBar.skillUIFontSize;

                // Set text UI Colour
                if (attackData.relicActiveSkillValueModifier == attackData.skillData.perfectValueMultiplier)
                    text.color = _combatManager.activeAttackBar.perfectSkillUIColour;
                else if (attackData.relicActiveSkillValueModifier == attackData.skillData.greatValueMultiplier)
                    text.color = _combatManager.activeAttackBar.greatSkillUIColour;
                else if (attackData.relicActiveSkillValueModifier == attackData.skillData.goodValueMultiplier)
                    text.color = _combatManager.activeAttackBar.goodSkillUIColour;
                else if (attackData.relicActiveSkillValueModifier == attackData.skillData.missValueMultiplier)
                    text.color = _combatManager.activeAttackBar.missSkillUIColour;

                float yValEffect = yDistBetweenEffectUI * (unit.effectHitCount - 1);
                pos.y = yValEffect;
            }
        }
        else
        {
            if (val != 0)
            {
                text.text = Mathf.Abs(val).ToString();  // Display damage

                // Set font size
                if (attackData.relicActiveSkillValueModifier == attackData.skillData.perfectValueMultiplier)
                    text.fontSize = _combatManager.activeAttackBar.skillUIPerfectFontSize;
                else
                    text.fontSize = _combatManager.activeAttackBar.skillUIFontSize;

                // Set text UI Colour
                if (attackData.relicActiveSkillValueModifier == attackData.skillData.perfectValueMultiplier)
                    text.color = _combatManager.activeAttackBar.perfectSkillUIColour;
                else if (attackData.relicActiveSkillValueModifier == attackData.skillData.greatValueMultiplier)
                    text.color = _combatManager.activeAttackBar.greatSkillUIColour;
                else if (attackData.relicActiveSkillValueModifier == attackData.skillData.goodValueMultiplier)
                    text.color = _combatManager.activeAttackBar.goodSkillUIColour;
                else if (attackData.relicActiveSkillValueModifier == attackData.skillData.missValueMultiplier)
                    text.color = _combatManager.activeAttackBar.missSkillUIColour;
            }
            else
            {
                if (!effectInfict)
                {
                    text.fontSize = _combatManager.activeAttackBar.skillUIMissFontSize;     // Set font size
                    text.text = "Miss";   // Display miss text
                    text.color = _combatManager.activeAttackBar.missSkillUIColour;
                }
            }
            
            float yVal = yDistBetweenUI * caster.hitWaveCount;
            pos.y = go.transform.localPosition.y + yVal;
        }

        go.transform.localPosition = pos;   // Update position

        // Set canvas sorting order to always appear infront of any other skill Text UI.
        if (effectInfict)
            skillValueUI.canvas.sortingOrder = 1 + caster.hitWaveCount + caster.hitWaveCountEffect;
        else
            skillValueUI.canvas.sortingOrder = caster.hitWaveCount;
    }

    public void SetSkillMaxCooldown(SkillData skillData)
    {
        skillData.curCooldown = skillData.turnCooldown;
        skillData.onCooldown = true;
    }

    void SetSkillCooldown(SkillData skillData, Selector selector)
    {
        if (skillData.curCooldown != 0)
            skillData.curCooldown--;

        if (skillData.curCooldown == 0)
            skillData.onCooldown = false;

        UpdateSkillCooldownVisuals(skillData, selector);
    }

    public void UpdateSkillCooldownVisuals(SkillData skillData, Selector selector)
    {
        if (skillData.turnCooldown == 0)
        {
            selector.cooldownImage.fillAmount = 0;
            selector.cooldownText.text = "";
            return;
        }

        if (skillData.curCooldown != 0)
        {
            float val = (float)skillData.curCooldown / (float)skillData.turnCooldown;
            selector.cooldownImage.fillAmount = Mathf.Round(val * 100) / 100f;
            selector.cooldownText.text = skillData.curCooldown.ToString();
        }
        else
        {
            selector.cooldownImage.fillAmount = 0;
            selector.cooldownText.text = "";
        }
    }

    public void DecrementSkillCooldown()
    {
        // Decrease all relic's cooldown
        SetSkillCooldown(_relicPassiveSkill, relicPassiveSelect);
        SetSkillCooldown(_relicBasicSkill, relicBasicSelect);
        SetSkillCooldown(_relicPrimarySkill, relicPrimarySelect);
        SetSkillCooldown(_relicSecondarySkill, relicSecondarySelect);
        SetSkillCooldown(_relicAlternateSkill, relicAlternateSelect);
        SetSkillCooldown(_relicUltimateSkill, relicUltimateSelect);
    }

    private int RoundFloatToInt(float f)
    {
        return Mathf.RoundToInt(f);
    }
}

