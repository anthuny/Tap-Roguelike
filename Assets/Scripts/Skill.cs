using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skill")]
public class Skill : ScriptableObject
{
    public string skillType;
    public string skillMode;
    public string targetType;
    public string attackSequence;
    public string targetsAllowed;
    public string inflictType;

    public int maxTargetSelections = 1;

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

    [Header("Skill UI ")]
    public Color skillIconColour;
    public Color skillBorderColour;
    public Color skillSelectionColour;
    public int maxSkillCount = 1;


}
