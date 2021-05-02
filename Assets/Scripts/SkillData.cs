using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillData : MonoBehaviour
{
    public string skillType;
    public string skillMode;
    public string targetType;
    public string attackSequence;
    public string targetsAllowed;
    public string inflictType;

    public int maxTargetCount = 1;

    [Tooltip("The name of the skill")]
    public new string name;
    [Tooltip("In game description")]
    public string description;
    [Tooltip("The skill's turn cooldown, if not applicable set to 0")]
    public int turnCooldown;
    [Tooltip("The skill's damage, if not applicable set to 0.")]
    public float valueOutcome;

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
    [Tooltip("The base value of chance for this skill's effects to apply. If not applicable set to 0")]
    public float procChance;
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

    public void InitializeSkill(Color skillIconColour, Color skillBorderColour, Color skillSelectionColour, string skillType = "Basic", string skillMode = "None", string targetType = "None", string attackSequence = "Individual", string targetsAllowed = "0", string inflictType = "None",
    int targetCount = 0, string name = "", string description = "", int turnCooldown = 0, float damage = 0, float missDamageMultiplier = 0,
    float goodDamageMultiplier = 0, float greatDamageMultiplier = 0, float perfectDamageMultiplier = 0, float procChance = 0, float missProcMultiplier = 0,
    float goodProcMultiplier = 0, float greatProcMultiplier = 0, float perfectProcMultiplier = 0, int maxSkillCount = 0)
    {
        this.skillIconColour = skillIconColour;
        this.skillBorderColour = skillBorderColour;
        this.skillSelectionColour = skillSelectionColour;
        this.skillType = skillType;
        this.skillMode = skillMode;
        this.targetType = targetType;
        this.attackSequence = attackSequence;
        this.targetsAllowed = targetsAllowed;
        this.inflictType = inflictType;
        this.maxTargetCount = targetCount;
        this.name = name;
        this.description = description;
        this.turnCooldown = turnCooldown;
        this.valueOutcome = damage;
        this.missValueMultiplier = missDamageMultiplier;
        this.goodValueMultiplier = goodDamageMultiplier;
        this.greatValueMultiplier = greatDamageMultiplier;
        this.perfectValueMultiplier = perfectDamageMultiplier;
        this.procChance = procChance;
        this.missProcMultiplier = missProcMultiplier;
        this.goodProcMultiplier = goodProcMultiplier;
        this.greatProcMultiplier = greatProcMultiplier;
        this.perfectProcMultiplier = perfectProcMultiplier;
        this.maxSkillCount = maxSkillCount;
    } 
}
