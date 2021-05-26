using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skill")]
public class Skill : ScriptableObject
{
    public string skillType;
    public string skillMode;
    public string targetType;
    public string targetsAllowed;
    public int hitsRequired;
    public int manaRequired;
    public float timeBetweenHitUI;
    public float timeTillEffectInflict;
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
    public bool activatable = true;

    [Tooltip("The name of the skill")]
    public new string name;
    [Tooltip("In game description")]
    public string description;
    [Tooltip("The skill's turn cooldown, if not applicable set to 0")]
    public int turnCooldown;

    [Space(1)]

    [Header("Power Multiplers")]
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


}
