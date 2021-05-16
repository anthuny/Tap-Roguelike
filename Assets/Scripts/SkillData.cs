using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillData : MonoBehaviour
{
    public int curTargetCount;
    public int curCooldown;
    public int curHitsCompleted;
    public int roundOfAttacks;

    public string skillType;
    public string skillMode;
    public string targetType;
    public string targetsAllowed;
    public int hitsRequired;
    public float timeBetweenHitUI;
    public float timeForNextHitMarker;
    public Effect effect;
    public string effectTarget;
    public int effectPower;
    public int effectDuration;
    public string effectHitEffect;
    public string effectDurationDecrease;
    public bool counterSkill;
    public float stackValue;
    public float targetAmountPowerInc;
    public bool isTargetCountValAmp;
    public int maxTargetCount = 1;

    [Tooltip("The name of the skill")]
    public new string name;
    [Tooltip("In game description")]
    public string description;
    [Tooltip("The skill's turn cooldown, if not applicable set to 0")]
    public int turnCooldown;

    [Space(1)]

    [Header("Damage Multiplers")]
    [Tooltip("The damage multiplier for missing. Multiplier is multiplied by damage for final outcome. If not applicable set to 0")]
    public float missValueMultiplier;
    [Tooltip("The damage multiplier for hitting a 'good'. Multiplier is multiplied by damage for final outcome. If not applicable set to 0")]
    public float goodValueMultiplier;
    [Tooltip("The damage multiplier for hitting a 'great'. Multiplier is multiplied by damage for final outcome. If not applicable set to 0")]
    public float greatValueMultiplier;
    [Tooltip("The damage multiplier for hitting a 'perfect'. Multiplier is multiplied by damage for final outcome. If not applicable set to 0")]
    public float perfectValueMultiplier;

    [Space(1)]

    [Header("Proc chance and Multiplers")]
    [Tooltip("The proc multiplier for missing. Multiplier is multiplied by proc chance for final outcome. If not applicable set to 0")]
    public float missProcMultiplier;
    [Tooltip("The proc multiplier for hitting a 'good'. Multiplier is multiplied by proc chance for final outcome. If not applicable set to 0")]
    public float goodProcMultiplier;
    [Tooltip("The proc multiplier for hitting a 'great'. Multiplier is multiplied by proc chance for final outcome. If not applicable set to 0")]
    public float greatProcMultiplier;
    [Tooltip("The proc multiplier for hitting a 'perfect'. Multiplier is multiplied by proc chance for final outcome. If not applicable set to 0")]
    public float perfectProcMultiplier;

    [Header("Skill UI ")]
    public Color skillIconColour;
    public Color skillBorderColour;
    public Color skillSelectionColour;
    public int maxSkillCount = 1;

    public void InitializeSkill(Color skillIconColour, Color skillBorderColour, Color skillSelectionColour, string skillType = "Basic", string skillMode = "None",
        string targetType = "None", string targetsAllowed = "0", int hitsRequired = 0, float timeBetweenHitUI = 0, float timeForNextHitMarker = 0, Effect effect = null, string effectTarget = "None", int effectPower = 0,
        int effectDuration = 0, string effectHitEffect = "None", string effectDurationDecrease = "None", bool counterSkill = false, float stackValue = 0,
        float targetAmountPowerInc = 1, bool isTargetCountValAmp = false, int targetCount = 0, string name = "", string description = "", int turnCooldown = 0, float missDamageMultiplier = 0, float goodDamageMultiplier = 0,
        float greatDamageMultiplier = 0, float perfectDamageMultiplier = 0, float missProcMultiplier = 0, float goodProcMultiplier = 0, 
        float greatProcMultiplier = 0, float perfectProcMultiplier = 0, int maxSkillCount = 0)
    {
        this.skillIconColour = skillIconColour;
        this.skillBorderColour = skillBorderColour;
        this.skillSelectionColour = skillSelectionColour;
        this.skillType = skillType;
        this.skillMode = skillMode;
        this.targetType = targetType;
        this.targetsAllowed = targetsAllowed;
        this.hitsRequired = hitsRequired;
        this.timeBetweenHitUI = timeBetweenHitUI;
        this.timeForNextHitMarker = timeForNextHitMarker;
        this.effect = effect;
        this.effectTarget = effectTarget;
        this.effectPower = effectPower;
        this.effectDuration = effectDuration;
        this.effectHitEffect = effectHitEffect;
        this.counterSkill = counterSkill;
        this.stackValue = stackValue;
        this.targetAmountPowerInc = targetAmountPowerInc;
        this.isTargetCountValAmp = isTargetCountValAmp;
        this.maxTargetCount = targetCount;
        this.name = name;
        this.description = description;
        this.turnCooldown = turnCooldown;
        this.missValueMultiplier = missDamageMultiplier;
        this.goodValueMultiplier = goodDamageMultiplier;
        this.greatValueMultiplier = greatDamageMultiplier;
        this.perfectValueMultiplier = perfectDamageMultiplier;
        this.missProcMultiplier = missProcMultiplier;
        this.goodProcMultiplier = goodProcMultiplier;
        this.greatProcMultiplier = greatProcMultiplier;
        this.perfectProcMultiplier = perfectProcMultiplier;
        this.maxSkillCount = maxSkillCount;
    } 
}
