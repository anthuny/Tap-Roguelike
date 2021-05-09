using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("UI")]
    public GameObject unitUI;
    public Image healthImage;
    public Text nameText;
    public Transform effectParent;
    public List<Image> effectImages = new List<Image>();
    
    private GameObject unitGO;

    [Header("Effects")]
    public List<Effect> effects = new List<Effect>();
    public List<Text> effectPowersText = new List<Text>();
    public List<int> effectPowers = new List<int>();


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
                                targetSingle.UpdateCurHealth(-power);
                                break;

                            case "PercentMaxHealthDamage":
                                targetSingle.UpdateCurHealth(-((power / 100) * targetSingle.maxHealth));
                                break;

                            case "Heal":
                                targetSingle.UpdateCurHealth(power);
                                break;
                                
                            case "PercentMaxHealHeal":
                                targetSingle.UpdateCurHealth(((power / 100) * targetSingle.maxHealth));
                                break;
                        }

                        // If skill has an effect
                        if (skillData.effect.name != "None")
                        {
                            // If Relic didn't miss
                            if (_combatManager.relicActiveSkillProcModifier != 0)
                                targetSingle.AssignEffect(skillData.effect, this, targetSingle, skillData);     // Attempt Assign effect

                        }
                        // If skill does not have an effect
                        else
                            _devManager.FlashText(this.name, targetSingle.name, skillData.name, power);   // Debug the attack
                        break;

                    case "Multiple":

                        for (int i = 0; i < _combatManager.targetSelections.Count; i++)
                        {
                            Unit targetMultiple = _combatManager.targetSelections[i].GetComponentInParent<Unit>();

                            switch (skillData.skillMode)
                            {
                                case "Damage":
                                    targetMultiple.UpdateCurHealth(-power);
                                    break;

                                case "PercentMaxHealthDamage":
                                    targetMultiple.UpdateCurHealth(-((power / 100) * targetMultiple.maxHealth));
                                    break;

                                case "Heal":
                                    targetMultiple.UpdateCurHealth(power);
                                    break;

                                case "PercentMaxHealHeal":
                                    targetMultiple.UpdateCurHealth(((power / 100) * targetMultiple.maxHealth));
                                    break;
                            }

                            // If skill has an effect
                            if (skillData.effect.name != "None")
                            {
                                // If Relic didn't miss
                                if (_combatManager.relicActiveSkillProcModifier != 0)
                                    targetMultiple.AssignEffect(skillData.effect, this, targetMultiple, skillData);     // Attempt Assign effect

                            }
                            // If skill does not have an effect
                            else
                                _devManager.FlashText(this.name, targetMultiple.name, skillData.name, power);   // Debug the attack
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

    public void AssignUI()
    {
        unitGO = Instantiate(_combatManager.unitUI, transform.position, Quaternion.identity);
        unitGO.transform.SetParent(transform);
        unitGO.transform.localPosition = new Vector2(-26, 82);
        healthImage = unitGO.transform.Find("Health/Current Health Image").GetComponent<Image>();
        nameText = unitGO.transform.Find("Name/Name Text").GetComponent<Text>();

        effectParent = unitGO.transform.Find("Effects").transform; // Assign effects parent

        // If unit type is an enemy, Assign effect UI
        if (unitType == UnitType.ENEMY)
        {
            effectParent.gameObject.SetActive(true);

            for (int i = 0; i < _combatManager.maxEffectsActive; i++)
            {
                effectImages.Add(effectParent.GetChild(i).GetComponent<Image>());

                EffectImage effectImage = effectImages[i].GetComponent<EffectImage>();

                effectImage.ToggleEffectImage(false);
                effectImage.ToggleEffectPowerText(false);
            }
        }
        else
        {
            effectParent.gameObject.SetActive(false);
        }
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

        UpdateCurHealthVisual(curHealth/maxHealth, true);
    }

    public void UpdateCurHealthVisual(float val, bool inCombat = true)
    {
        healthImage.fillAmount = val;
    }

    public void AssignEffect(Effect effect, Unit caster, Unit target, SkillData skillData)
    {
        float rand = Random.Range(0, _combatManager.relicActiveSkillProcModifier);
        if (rand <= _combatManager.relicActiveSkillProcModifier)
        {
            //inflictedType = inflict;

            _devManager.FlashText(caster.name, target.name, skillData.name, caster.power, skillData.effectPower, skillData.effect.name);

            UpdateEffectVisual(effect, skillData.effectPower, skillData.effectDuration);
        }
    }

    public void UpdateEffectVisual(Effect effect, int effectPower, int effectDuration)
    {
        // If unit is currently already inflicted with an effect, increase the power of the same effect instead of adding a new effect
        for (int i = 0; i < effects.Count; i++)
        {
            if (effect.name == effects[i].name)
            {
                EffectImage effectImage = effectImages[i].GetComponent<EffectImage>();

                effectImage.UpdateEffectPower(effectPower);

                return;
            }
        }

        for (int i = 0; i < effectImages.Count; i++)
        {
            if (!effectImages[i].enabled)
            {
                EffectImage effectImage = effectImages[i].GetComponent<EffectImage>();

                effects.Add(effect);
                effectPowers.Add(effectPower);

                effectImage.UpdateEffectPower(effectPower);
                effectImage.UpdateEffectDuration(effectDuration);
                effectImage.ToggleEffectImage(true);
                effectImage.ToggleEffectPowerText(true);
                effectImage.SetEffectImage(effect.effectImage);

                return;
            }
        }
    }

    public void UpdateUnitType(UnitType unitType)
    {
        this.unitType = unitType;
    }

    public void UpdateName(string displayName)
    {
        this.name = displayName;
        nameText.text = this.name;
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
