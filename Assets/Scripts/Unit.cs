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
    [Tooltip("The default power the unit has")]
    public int power;
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
                                targetSingle.UpdateCurHealth(-skillData.power);
                                break;

                            case "PercentMaxHealthDamage":
                                targetSingle.UpdateCurHealth(-((skillData.power / 100) * targetSingle.maxHealth));
                                break;

                            case "Heal":
                                targetSingle.UpdateCurHealth(skillData.power);
                                break;
                                
                            case "PercentMaxHealHeal":
                                targetSingle.UpdateCurHealth(((skillData.power / 100) * targetSingle.maxHealth));
                                break;
                        }

                        if (skillData.inflictType != "None")
                        {
                            targetSingle.AssignInflict(skillData.inflictType, this, targetSingle, skillData.inflictType);
                        }

                        _devManager.FlashText(this.name, targetSingle.name, 
                            skillData.name, skillData.power, skillData.inflictType, skillData.inflictDuration);

                        break;

                    case "Multiple":

                        for (int i = 0; i < _combatManager.targetSelections.Count; i++)
                        {
                            Unit targetMultiple = _combatManager.targetSelections[i].GetComponentInParent<Unit>();

                            switch (skillData.skillMode)
                            {
                                case "Damage":
                                    targetMultiple.UpdateCurHealth(-skillData.power);
                                    break;

                                case "PercentMaxHealthDamage":
                                    targetMultiple.UpdateCurHealth(-((skillData.power / 100) * targetMultiple.maxHealth));
                                    break;

                                case "Heal":
                                    targetMultiple.UpdateCurHealth(skillData.power);
                                    break;

                                case "PercentMaxHealHeal":
                                    targetMultiple.UpdateCurHealth(((skillData.power / 100) * targetMultiple.maxHealth));
                                    break;
                            }

                            if (skillData.inflictType != "None")
                            {
                                targetMultiple.AssignInflict(skillData.inflictType, this, targetMultiple, skillData.inflictType);
                            }

                            _devManager.FlashText(this.name, targetMultiple.name,
                                skillData.name, skillData.power, skillData.inflictType, skillData.inflictDuration);
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
    public void UpdateMaxHealth(float val)
    {
        maxHealth += val;
    }


    /// <summary>
    /// Adjust max current health of unit
    /// </summary>
    public void UpdateCurHealth(float val, bool inCombat = true)
    {
        if (inCombat)
        {
            int combatValue = RoundFloatToInt(val * _combatManager.relicActiveSkillValueModifier);
            curHealth += combatValue;
        }
        else
            curHealth += RoundFloatToInt(val);


        _combatManager.UpdateSkillUI();
    }

    public void AssignInflict(string inflict, Unit caster, Unit target, string skillData)
    {
        if (_combatManager.relicActiveSkillProcModifier >= 1)
        {
            inflictedType = inflict;

            _combatManager.UpdateSkillUI();
        }
    }

    public void UpdateUnitType(UnitType unitType)
    {
        this.unitType = unitType;
    }

    public void UpdateName(string displayName)
    {
        name = displayName;
    }

    public void UpdateLevel(int level)
    {
        this.level = level;
    }

    public void UpdateColour(Color color)
    {
        this.color = color;
    }
   
    public void UpdatePower(int val)
    {
        power += val;
    }

    public void UpdateSpeed(int val)
    {
        speed += val;
    }

    /// <summary>
    /// Adjust Attack Chance of unit
    /// </summary>
    public void UpdateAttackChance(int val, bool update = false)
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
    public void UpdateSkillChance(int val, bool update = false)
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
