using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
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

    [Header("The liklihood of what the enemy will do on their turn")]
    public int attackChance;
    public int skillChance;

    [Header("Skills")]
    public List<Skill> passiveSkills = new List<Skill>();
    public List<Skill> primarySkills = new List<Skill>();
    public List<Skill> secondarySkills = new List<Skill>();
    public List<Skill> alternateSkills = new List<Skill>();
    public List<Skill> ultimateSkills = new List<Skill>();

    private DevManager _devManager;
    private Skill _activeSkill;
    //[HideInInspector]
    public List<Unit> targets = new List<Unit>();

    private void Awake()
    {
        _devManager = FindObjectOfType<DevManager>();
    }

    public void SkillFunctionality(Skill skill)
    {      
        switch (skill.targetsAllowed)
        {
            case Skill.TargetsAllowed.ENEMIES:

                switch (skill.targetType)
                {
                    case Skill.TargetType.SINGLE:
                        targets[Random.Range(0, targets.Count)].AdjustCurHealth(RoundFloatToInt(-(damage * skill.damage)), this, targets[0], skill);

                        break;

                    case Skill.TargetType.MULTI:
                        break;
                    case Skill.TargetType.NONE:
                        break;
                }

                break;

            case Skill.TargetsAllowed.ALLIES:

                switch (skill.targetType)
                {
                    case Skill.TargetType.SINGLE:
                        break;
                    case Skill.TargetType.MULTI:
                        break;
                    case Skill.TargetType.NONE:
                        break;
                }

                break;


        }
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
    public void AdjustCurHealth(float val, Unit caster, Unit target, Skill skill = null)
    {
        if (skill != null)
            _devManager.StartCoroutine(_devManager.FlashText(caster.name + " attacked " + target.name + " using " + skill.name + " (" + val + ")"));
        else
            _devManager.StartCoroutine(_devManager.FlashText(caster.name + " attacked " + target.name + " using regular attack " + " (" + val + ")"));

        curHealth += val;
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
