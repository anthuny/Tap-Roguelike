﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoBehaviour
{
    private CombatManager _combatManager;
    private UIManager _UIManager;
    private UnitHUDInfo _unitHUDInfo;

    public enum SkillStatus { DISABLED, ACTIVE}
    public SkillStatus skillStatus;

    [Header("Skill Initialization")]
    [SerializeField] private SkillData _passiveSkill;
    [SerializeField] private SkillData _basicSkill;
    [SerializeField] private SkillData _primarySkill;
    [SerializeField] private SkillData _secondarySkill;
    [SerializeField] private SkillData _alternateSkill;
    [SerializeField] private SkillData _ultimateSkill;
    [Space(1)]
    [Tooltip("Text GameObject to instantiate")]
    [SerializeField] private GameObject skillUIText;
    [Tooltip("The speed at which skill UI values travel upwards")]
    public float panSpeedUI;
    [Tooltip("How long the text remains on screen before destroying")]
    public float textPanLength;
    [Tooltip("How long the text remains on screen after all text is displayed")]
    public float textBonusLength;
    [Tooltip("The difference in Y value between each skill UI value has")]
    public float yDistBetweenUI;
    public float yDistBetweenEffectUI;
    public float xDisplacementStr;
    public float SkillUIFadeOutSpeed;
    [Tooltip("The higher the value, the longer each attack value displays for after the previous")]
    public float elapsedTimeDestroyMultiplier;

    //[HideInInspector]
    public List<SkillValueUI> storedSkillValueUI = new List<SkillValueUI>();

    [Header("Active Stored Attacks")]
    public AttackData activeAttackData;

    private float textOffsetDist;
    private float yVal;

    private Target target;

    private void Awake()
    {
        _combatManager = FindObjectOfType<CombatManager>();
        _UIManager = FindObjectOfType<UIManager>();
        _unitHUDInfo = FindObjectOfType<UnitHUDInfo>();
        StartCoroutine(_UIManager.ToggleImage(_UIManager.endTurnGO, false));
    }

    public void AssignFirstSkill(SkillData skillData)
    {
        _combatManager.activeSkill = skillData;
    }
    public void AssignSkillAesthetics(Image skillIcon, Image skillSelectionIcon, Image skillBorderIcon,
        Color skillIconColour, Color skillSelectionColour, Color skillBorderColour)
    {
        skillIcon.color = skillIconColour;
        skillSelectionIcon.color = skillSelectionColour;
        skillBorderIcon.color = skillBorderColour;
    }

    public void InitializeSkills(Unit unit)
    {
        AssignSkills(unit);
    }
    public void AssignSkills(Unit unit)
    {
        _basicSkill = unit.basicSkill;
        _primarySkill = unit.primarySkill;
        _secondarySkill = unit.secondarySkill;
        _alternateSkill = unit.alternateSkill;
    }

    public void SetActiveAttackData(AttackData attackData)
    {
        activeAttackData = attackData;
    }

    public void RemoveText(SkillValueUI skillValueUI)
    {
        storedSkillValueUI.Remove(skillValueUI);
    }
    public void HideValueUI()
    {
        for (int i = 0; i < storedSkillValueUI.Count; i++)
        {
            storedSkillValueUI[i].HideText();
        }
    }

    public void ResetTextOffset()
    {
        textOffsetDist = 0;
    }

    public void DisplaySkillValue(Unit caster, Unit target, Transform skillUIValueParent, float val = 0, AttackData attackData = null)
    {
        GameObject go = Instantiate(skillUIText, skillUIValueParent.position, Quaternion.identity);    // Initialize
        SkillValueUI skillValueUI = go.GetComponent<SkillValueUI>();    // Initialize
        target.storedskillValueUI.Add(skillValueUI);    // add skill value ui to unit for on destroy
        go.GetComponent<SkillValueUI>().skillUIManager = this;    // Initialize
        Text text = go.GetComponent<Text>();    // Initalization
        go.transform.SetParent(skillUIValueParent);    // Set parent
        skillValueUI.index = target.hitRecievedCount;

        // Set text off set 
        if (skillUIValueParent.childCount >= 2)
            textOffsetDist = skillUIValueParent.GetChild(skillUIValueParent.childCount-2).GetComponent<SkillValueUI>().CalculateDistanceTravelled();

        // Reset all texts movement again
        storedSkillValueUI.Add(skillValueUI);
        for (int i = 0; i < storedSkillValueUI.Count; i++)
        {
            storedSkillValueUI[i].EnableMoving();
        }

        Vector3 pos = go.transform.localPosition;    // Initalization
        pos.x = Random.Range(-xDisplacementStr, xDisplacementStr);    // Randomize x position

        // If value is not 0
        if (val != 0)
        {
            text.text = Mathf.Abs(val).ToString();  // Display damage

            // Set font size
            if (attackData.activeSkillValueModifier == attackData.skillData.perfectValueMultiplier)
                text.fontSize = _combatManager.activeAttackBar.skillUIPerfectFontSize;
            else
                text.fontSize = _combatManager.activeAttackBar.skillUIFontSize;

            // Set text UI Colour
            if (attackData.activeSkillValueModifier == attackData.skillData.perfectValueMultiplier)
                text.color = _combatManager.activeAttackBar.perfectSkillUIColour;
            else if (attackData.activeSkillValueModifier == attackData.skillData.greatValueMultiplier)
                text.color = _combatManager.activeAttackBar.greatSkillUIColour;
            else if (attackData.activeSkillValueModifier == attackData.skillData.goodValueMultiplier)
                text.color = _combatManager.activeAttackBar.goodSkillUIColour;
            else if (attackData.activeSkillValueModifier == attackData.skillData.missValueMultiplier)
                text.color = _combatManager.activeAttackBar.missSkillUIColour;
        }
        // If value is 0
        else
        {
            text.fontSize = _combatManager.activeAttackBar.skillUIMissFontSize;     // Set font size
            text.text = "Miss";   // Display miss text
            text.color = _combatManager.activeAttackBar.missSkillUIColour;
        }

        yVal = (yDistBetweenUI + textOffsetDist) * (target.hitRecievedCount-1);
        pos.y = go.transform.localPosition.y + yVal;

        go.transform.localPosition = pos;   // Update position

        // Set canvas sorting order to always appear infront of any other skill Text UI.
        skillValueUI.canvas.sortingOrder = target.hitRecievedCount;

        // If this is the last UI of the skill on for this attack
        // Reset hit recieved count for next skill UI
        if (_combatManager.activeSkill)
        {
            if (target.hitRecievedCount == _combatManager.activeSkill.hitsRequired)
                target.hitRecievedCount = 0;
        }
    }

    public void SetSkillMaxCooldown(SkillData skillData)
    {
        skillData.curCooldown = skillData.turnCooldown;
        skillData.onCooldown = true;
    }

    public void UpdateSkillCooldown(SkillData skillData)
    {
        if (skillData.curCooldown != 0)
        {
            skillData.onCooldown = true;
            skillData.curCooldown--;
        }

        if (skillData.curCooldown == 0)
            skillData.onCooldown = false;

        //UpdateSkillCooldownVisuals(skillData);
    }

    public void UpdateSkillCooldownVisuals(SkillData skillData, Unit.UnitType unitType)
    {
        /*
        if (unitType == Unit.UnitType.ENEMY)
        {
            switch (skillData.skillType)
            {
                case "Passive":
                    target = enemySkillPassiveTarget;
                    break;
                case "Basic":
                    target = enemySkillBasicTarget;
                    break;
                case "Primary":
                    target = enemySkillPrimaryTarget;
                    break;
                case "Secondary":
                    target = enemySkillSecondaryTarget;
                    break;
                case "Alternate":
                    target = enemySkillAlternateTarget;
                    break;
                case "Ultimate":
                    target = enemySkillUltimateTarget;
                    break;
            }
        }
        else if (unitType == Unit.UnitType.ALLY)
        {
            switch (skillData.skillType)
            {
                case "Passive":
                    target = relicSkillPassiveTarget;
                    break;
                case "Basic":
                    target = relicSkillBasicTarget;
                    break;
                case "Primary":
                    target = relicSkillPrimaryTarget;
                    break;
                case "Secondary":
                    target = relicSkillSecondaryTarget;
                    break;
                case "Alternate":
                    target = relicSkillAlternateTarget;
                    break;
                case "Ultimate":
                    target = relicSkillUltimateTarget;
                    break;
            }
        }

        // Loop through each relic skill to see which target to use
        if (skillData.turnCooldown == 0)
        {
            target.cooldownImage.fillAmount = 0;
            target.cooldownText.text = "";
            return;
        }

        if (skillData.curCooldown != 0)
        {
            float val = (float)skillData.curCooldown / (float)skillData.turnCooldown;
            target.cooldownImage.fillAmount = Mathf.Round(val * 100) / 100f;
            target.cooldownText.text = skillData.curCooldown.ToString();
        }
        else
        {
            target.cooldownImage.fillAmount = 0;
            target.cooldownText.text = "";
        }
        */
    }

    public void DecrementSkillCooldown()
    {
        // Decrease all relic's cooldown
        UpdateSkillCooldown(_passiveSkill);
        UpdateSkillCooldown(_basicSkill);
        UpdateSkillCooldown(_primarySkill);
        UpdateSkillCooldown(_secondarySkill);
        UpdateSkillCooldown(_alternateSkill);
        UpdateSkillCooldown(_ultimateSkill);
    }

    private int RoundFloatToInt(float f)
    {
        return Mathf.RoundToInt(f);
    }
}

