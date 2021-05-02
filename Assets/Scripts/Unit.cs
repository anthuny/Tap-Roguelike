using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public enum UnitType { ALLY, ENEMY };
    public UnitType unitType;

    public new string name;
    public int level = 1;

    [Header("Aesthetics")]
    public Color color;

    [Header("Statistics")]
    [Tooltip("The default maximum health the enemy spawns with")]
    public float maxHealth;
    public float curHealth;
    [Tooltip("The default amount of hit damage the enemy currently has")]
    public int damage;
    [Tooltip("The percentage rate of whether the enemy will attack before the player")]
    public int speed;
    public int turnSpeed;
    public string inflictedType;

    [Header("The liklihood of what the enemy will do on their turn")]
    public int attackChance;
    public int skillChance;


    [Header("Skills")]
    public SkillData passiveSkill;
    public SkillData basicSkill;
    public SkillData primarySkill;
    public SkillData secondarySkill;
    public SkillData alternateSkill;
    public SkillData ultimateSkill;

    private DevManager _devManager;
    private Skill _activeSkill;
    //[HideInInspector]
    public List<Unit> targets = new List<Unit>();
    private CombatManager _combatManager;


    private void Awake()
    {
        _devManager = FindObjectOfType<DevManager>();
        _combatManager = FindObjectOfType<CombatManager>();
    }

    public void DetermineUnitMoveChoice(Unit unit, SkillData skillData)
    {
        if (unitType == UnitType.ENEMY)
        {
            // If i unit's attack chance is greater then their skill chance
            //if (unit.attackChance > unit.skillChance)
                //unit.UnitSkillFunctionality(skillData);   // Perform a basic attack

            // If i enemy's skill chance is greater then their skill chance
            //else
            //unit.UnitSkillFunctionality(unit.primarySkill);  // Enemy casts a random skill
        }

        if (unitType == UnitType.ALLY)
        {
            UnitSkillFunctionality(skillData);
        }
    }

    /// <summary>
    /// Caster performs a skill on a target
    /// </summary>
    public void UnitSkillFunctionality(SkillData skillData)
    {
        AssignSelectionCount(skillData);

        switch (skillData.targetsAllowed)
        {
            case "Enemies":

                switch (skillData.targetType)
                {
                    case "Single":

                        Unit targetSingle = _combatManager.targetSelections[0].GetComponentInParent<Unit>();

                        switch (skillData.skillMode)
                        {
                            case "Damage":
                                targetSingle.AdjustCurHealth(RoundFloatToInt(-skillData.valueOutcome), this, targetSingle, skillData);
                                break;

                            case "PercentMaxHealthDamage":
                                targetSingle.AdjustCurHealth(RoundFloatToInt(-((skillData.valueOutcome / 100) * targetSingle.maxHealth)), this, targetSingle, skillData);
                                break;

                            case "Heal":
                                targetSingle.AdjustCurHealth(RoundFloatToInt(skillData.valueOutcome), this, targetSingle, skillData);
                                break;
                            case "PercentMaxHealHeal":
                                targetSingle.AdjustCurHealth(RoundFloatToInt((skillData.valueOutcome / 100) * targetSingle.maxHealth), this, targetSingle, skillData);
                                break;
                        }

                        if (skillData.inflictType != "None")
                        {
                            targetSingle.AssignInflict(skillData.inflictType, this, targetSingle, skillData.inflictType);
                        }
                        break;

                    case "Multiple":

                        for (int i = 0; i < _combatManager.targetSelections.Count; i++)
                        {
                            Unit targetMultiple = _combatManager.targetSelections[i].GetComponentInParent<Unit>();

                            switch (skillData.skillMode)
                            {
                                case "Damage":
                                    targetMultiple.AdjustCurHealth(RoundFloatToInt(-skillData.valueOutcome), this, targetMultiple, skillData);
                                    break;

                                case "PercentMaxHealthDamage":
                                    targetMultiple.AdjustCurHealth(RoundFloatToInt(-((skillData.valueOutcome / 100) * targetMultiple.maxHealth)), this, targetMultiple, skillData);
                                    break;

                                case "Heal":
                                    targetMultiple.AdjustCurHealth(RoundFloatToInt(skillData.valueOutcome), this, targetMultiple, skillData);
                                    break;
                                case "PercentMaxHealhHeal":
                                    targetMultiple.AdjustCurHealth(RoundFloatToInt((skillData.valueOutcome / 100) * targetMultiple.maxHealth), this, targetMultiple, skillData);
                                    break;
                            }

                            if (skillData.inflictType != "None")
                            {
                                targetMultiple.AssignInflict(skillData.inflictType, this, targetMultiple, skillData.inflictType);
                            }
                        }
                        break;
                    case "None":
                        break;
                }

                break;

            case "Allies":

                switch (skillData.targetType)
                {
                    case "Single":
                        break;
                    case "Multiple":
                        break;
                    case "None":
                        break;
                }

                break;

            case "None":
                break;
        }

        _combatManager.StartCoroutine("StartEnemysTurn");
    }

    void AssignSelectionCount(SkillData skill)
    {
        _combatManager.maxTargetSelections = skill.maxTargetCount;
    }

    public void CalculateSpeedFinal()
    {
        turnSpeed = Random.Range(0, speed);
    }

    /// <summary>
    /// Adjust max health of unit
    /// </summary>
    public void AdjustMaxHealth(float val)
    {
        maxHealth += val;
    }

    /// <summary>
    /// Adjust max current health of unit
    /// </summary>
    public void AdjustCurHealth(float dmg, Unit caster, Unit target, SkillData skillData = null)
    {
        int totalValue = RoundFloatToInt(dmg * _combatManager.relicActiveSkillValueModifier);

        if (skillData != null)
            _devManager.StartCoroutine(_devManager.FlashText(caster.name + " attacked " + target.name + " using " + skillData.name + " (" + totalValue + ")"));
        else
            _devManager.StartCoroutine(_devManager.FlashText(caster.name + " attacked " + target.name + " using regular attack " + " (" + totalValue + ")"));

        curHealth += totalValue;

        _combatManager.UpdateSkillUI();
    }

    public void AssignInflict(string inflict, Unit caster, Unit target, string skillData)
    {
        if (_combatManager.relicActiveSkillProcModifier >= 1)
        {
            _devManager.StartCoroutine(_devManager.FlashText(caster.name + " Inflicted " + target.name + " with " + skillData));

            inflictedType = inflict;

            _combatManager.UpdateSkillUI();
        }    
    }

    /// <summary>
    /// Adjust damage of unit
    /// </summary>
    public void AdjustDamage(int val)
    {
        damage += val;
    }

    /// <summary>
    /// Adjust speed of unit
    /// </summary>
    public void AdjustSpeed(int val)
    {
        speed += val;
    }

    /// <summary>
    /// Adjust Attack Chance of unit
    /// </summary>
    public void AdjustAttackChance(int val, bool update = false)
    {
        // If update is true, override attack chance value
        if (update)
            attackChance = Random.Range(0, val);
        // otherwise, just adjust the value
        else
            attackChance += val;

        // Adjust skill chance to be the chance that attack chance is not
        skillChance = (100 - attackChance);
    }

    /// <summary>
    /// Adjust Skill Chance of unit
    /// </summary>
    public void AdjustSkillChance(int val, bool update = false)
    {
        // If update is true, override skill chance value
        if (update)
            attackChance = Random.Range(0, val);
        // otherwise, just adjust the value
        else
            attackChance += val;

        // Adjust attack chance to be the chance that skill chance is not
        skillChance = (100 - attackChance);
    }

    private int RoundFloatToInt(float f)
    {
        return Mathf.RoundToInt(f);
    }
}
