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

    public int targetCount;
    [Tooltip("The name of the skill")]
    public new string name;
    [Tooltip("In game description")]
    public string description;
    [Tooltip("The skill's turn cooldown, if not applicable set to 0")]
    public int turnCooldown;
    [Tooltip("The skill's damage, if not applicable set to 0.")]
    public float damage;

    [Space(1)]

    [Header("Damage Multiplers")]
    [Tooltip("The damage multiplier for missing. Multiplier is multiplied by damage for final outcome. If not applicable set to 0")]
    public float missDamageMultiplier;
    [Tooltip("The damage multiplier for hitting a 'good'. Multiplier is multiplied by damage for final outcome. If not applicable set to 0")]
    public float goodDamageMultiplier;
    [Tooltip("The damage multiplier for hitting a 'great'. Multiplier is multiplied by damage for final outcome. If not applicable set to 0")]
    public float greatDamageMultiplier;
    [Tooltip("The damage multiplier for hitting a 'perfect'. Multiplier is multiplied by damage for final outcome. If not applicable set to 0")]
    public float perfectDamageMultiplier;

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

    public void InitializeSkill(string skillType = "Basic", string skillMode = "None", string targetType = "None", string attackSequence = "Individual", string targetsAllowed = "0", string inflictType = "None",
int targetCount = 0, string name = "", string description = "", int turnCooldown = 0, float damage = 0, float missDamageMultiplier = 0,
float goodDamageMultiplier = 0, float greatDamageMultiplier = 0, float perfectDamageMultiplier = 0, float procChance = 0, float missProcMultiplier = 0,
float goodProcMultiplier = 0, float greatProcMultiplier = 0, float perfectProcMultiplier = 0)
    {
        this.skillType = skillType;
        this.skillMode = skillMode;
        this.targetType = targetType;
        this.attackSequence = attackSequence;
        this.targetsAllowed = targetsAllowed;
        this.inflictType = inflictType;
        this.targetCount = targetCount;
        this.name = name;
        this.description = description;
        this.turnCooldown = turnCooldown;
        this.damage = damage;
        this.missDamageMultiplier = missDamageMultiplier;
        this.goodDamageMultiplier = goodDamageMultiplier;
        this.greatDamageMultiplier = greatDamageMultiplier;
        this.perfectDamageMultiplier = perfectDamageMultiplier;
        this.procChance = procChance;
        this.missProcMultiplier = missProcMultiplier;
        this.goodProcMultiplier = goodProcMultiplier;
        this.greatProcMultiplier = greatProcMultiplier;
        this.perfectProcMultiplier = perfectProcMultiplier;
    }

    public void asd()
    {
        Debug.Log("help");
    }
}
