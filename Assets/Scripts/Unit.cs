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
    public Image healthImage;
    public Text nameText;
    public Transform effectParent;
    public List<Image> effectImages = new List<Image>();
    public Transform skillUIValueParent;

    private GameObject unitGO;

    [Header("Effects")]
    public List<Effect> effects = new List<Effect>();
    public List<Text> effectPowersText = new List<Text>();
    public List<int> effectPowers = new List<int>();
    public float recievedDamageAmp;


    [Header("Skills")]
    public SkillData passiveSkill;
    public SkillData basicSkill;
    public SkillData primarySkill;
    public SkillData secondarySkill;
    public SkillData alternateSkill;
    public SkillData ultimateSkill;

    private DevManager _devManager;
    private Skill _activeSkill;
    [HideInInspector]
    public List<Unit> targets = new List<Unit>();
    private CombatManager _combatManager;
    private SkillUIManager _skillUIManager;

    private AttackData curAttackData;
    private AttackData prevAttackData;
    private float time = 0;
    private bool started;
    [HideInInspector]
    public int hitWaveCount;
    //[HideInInspector]
    public int hitWaveCountEffect;
    //[HideInInspector]
    public int maxWaveCountEffects;
    [HideInInspector]
    public int hitCount;
    bool storingAttack;
    [HideInInspector]
    public int effectHitCount;


    private void Awake()
    {
        _devManager = FindObjectOfType<DevManager>();
        _combatManager = FindObjectOfType<CombatManager>();
        _skillUIManager = FindObjectOfType<SkillUIManager>();
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
            StartCoroutine(UnitSkillFunctionality(skillData));
        }
    }

    /// <summary>
    /// Caster performs a skill on a target
    /// </summary>
    public IEnumerator UnitSkillFunctionality(SkillData skillData)
    {
        AssignSelectionCount(skillData);

        skillData.curHitsCompleted++;   // Increase current hits completed by 1

        if (skillData.curHitsCompleted == skillData.hitsRequired)
        {
            _skillUIManager.SetSkillMaxCooldown(skillData);
            _skillUIManager.UpdateSkillCooldownVisuals(skillData, _combatManager.activeSkillSelector);
        }

        else if (skillData.curHitsCompleted >= skillData.hitsRequired)
        {
            Debug.LogWarning("1 Extra hit was not valid");
            yield return null;
        }

        if (skillData.hitsRequired == 1)
            maxWaveCountEffects = 1;

        if (skillData.hitsRequired > 1)
            maxWaveCountEffects = skillData.hitsRequired-1;

        switch (skillData.targetsAllowed)
        {
            case "Enemies":

                switch (skillData.targetType)
                {
                    case "Single":

                        Unit targetSingle = _combatManager.targetSelections[0].GetComponentInParent<Unit>();

                        hitCount++;

                        switch (skillData.skillMode)
                        {
                            case "Damage":
                                StoreSkillCause(targetSingle, this, skillData, -power, true); 
                                break;

                            case "PercentMaxHealthDamage":
                                StoreSkillCause(targetSingle, this, skillData, -((power / 100) * targetSingle.maxHealth), true);
                                break;

                            case "Heal":
                                StoreSkillCause(targetSingle, this, skillData, power, true);
                                break;

                            case "PercentMaxHealHeal":
                                StoreSkillCause(targetSingle, this, skillData, ((power / 100) * targetSingle.maxHealth), true);
                                break;
                        }
                            /*
                            // If skill has an effect
                            if (skillData.effect.name != "None")
                            {
                                // If Relic didn't miss
                                if (_combatManager.relicActiveSkillProcModifier != 0)
                                    StoreSkillCause(targetSingle, this, skillData, -power, true);

                            }
                            // If skill does not have an effect
                            */
                            
                            _devManager.FlashText(this.name, targetSingle.name, skillData.name, power, targetSingle);   // Debug the attack

                        break;


                    case "Multiple":

                        for (int i = 0; i < _combatManager.targetSelections.Count; i++)
                        {
                            Unit targetMultiple = _combatManager.targetSelections[i].GetComponentInParent<Unit>();
                            hitCount++;

                            switch (skillData.skillMode)
                            {
                                case "Damage":
                                    if (!storingAttack)
                                        StoreSkillCause(targetMultiple, this, skillData, -power, true);
                                    break;

                                case "PercentMaxHealthDamage":
                                    StoreSkillCause(targetMultiple, this, skillData, -((power / 100) * targetMultiple.maxHealth), true);
                                    break;

                                case "Heal":
                                    StoreSkillCause(targetMultiple, this, skillData, power, true);
                                    break;

                                case "PercentMaxHealHeal":
                                    StoreSkillCause(targetMultiple, this, skillData, ((power / 100) * targetMultiple.maxHealth), true);
                                    break;
                            }
                            /*
                            // If skill has an effect
                            if (skillData.effect.name != "None")
                            {
                                // If Relic didn't miss
                                if (_combatManager.relicActiveSkillProcModifier != 0)
                                    StoreSkillCause(targetMultiple, this, skillData, -power, true);

                            }
                            */
                            // If skill does not have an effect
                            _devManager.FlashText(this.name, targetMultiple.name, skillData.name, power, targetMultiple);   // Debug the attack
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

        // Disable the back button after the first hit of the skill
        if (skillData.curHitsCompleted == 1)
            _combatManager.activeAttackBar.ToggleBackButton(false, true);

        // If this attack was the last required hit
        if (skillData.curHitsCompleted == skillData.hitsRequired)
        {
            StartCoroutine(SendSkillUI(_combatManager.relicActiveSkill));
            skillData.curHitsCompleted = 0;
            StartCoroutine(_combatManager.activeAttackBar.DestroyAllHitMarkersCo());
        }
    }

    void StoreSkillCause(Unit target, Unit caster, SkillData skillData, float val = 0, bool inCombat = true)
    {
        curAttackData = _combatManager.gameObject.AddComponent<AttackData>();
        _combatManager.activeAttackData.Add(curAttackData);

        curAttackData.skillData = skillData;
        curAttackData.inCombat = inCombat;
        curAttackData.target = target;

        // if skill is not dealing more damage based on the amount of targets selected
        if (!skillData.isTargetCountValAmp)
            curAttackData.val = _combatManager.CalculateDamageDealt(val, _combatManager.relicActiveSkillValueModifier);
        else
            curAttackData.val = _combatManager.CalculateDamageDealt(val, _combatManager.relicActiveSkillValueModifier, skillData.targetAmountPowerInc, _combatManager.targetSelections.Count);

        curAttackData.effect = skillData.effect;
        curAttackData.skillName = skillData.name;
        curAttackData.skillEffectName = skillData.effect.name;
        curAttackData.effectPower = skillData.effectPower;
        curAttackData.effectDuration = skillData.effectDuration;
        curAttackData.curHitsCompleted = skillData.curHitsCompleted;
        curAttackData.hitsRequired = skillData.hitsRequired;
        curAttackData.timeBetweenHitUI = skillData.timeBetweenHitUI;
        curAttackData.timeTillEffectInflict = skillData.timeTillEffectInflict;
        curAttackData.skillUIValueParent = target.skillUIValueParent;
        curAttackData.curTargetCount = _combatManager.targetSelections.Count;
        curAttackData.effectVal = skillData.effect.stackValue;
        curAttackData.relicActiveSkillValueModifier = _combatManager.relicActiveSkillValueModifier;
    }

    IEnumerator SendSkillUI(SkillData skillData)
    {
        for (int i = 0; i < _combatManager.activeAttackData.Count; i++)
        {
            // After looping through all targets
            if (i % curAttackData.curTargetCount == 0 && i != 0)
            {
                hitWaveCount++;
                yield return new WaitForSeconds(curAttackData.timeBetweenHitUI);
            }

            // If this is not the last attack
            if (i != _combatManager.activeAttackData.Count)
            {
                // send values to target
                _combatManager.activeAttackData[i].target.UpdateCurHealth(_combatManager.activeAttackData[i].inCombat,
                    _combatManager.activeAttackData[i].val, false, _combatManager.activeAttackData[i].skillUIValueParent,
                    _combatManager.activeAttackData[i], this);

                // Attempt Assign effect
                _combatManager.activeAttackData[i].target.AssignEffect(_combatManager.activeAttackData[i].effect,
                    _combatManager.activeAttackData[i].effectPower, _combatManager.activeAttackData[i].effectDuration,
                    _combatManager.activeAttackData[i].skillData);
            }

            // If this is the last attack
            if (i == _combatManager.activeAttackData.Count -1)
            {
                yield return new WaitForSeconds(curAttackData.timeTillEffectInflict);
                // Loop through all stored attacks
                for (int x = 0; x < _combatManager.activeAttackData.Count; x++)
                {

                    _combatManager.activeAttackData[x].target.UpdateCurHealth(_combatManager.activeAttackData[x].inCombat,
                    _combatManager.activeAttackData[x].val, true, _combatManager.activeAttackData[x].skillUIValueParent,
                    _combatManager.activeAttackData[x], this);

                    // After looping through all targets
                    if ((x+1) % (curAttackData.curTargetCount) == 0)
                    {
                        hitWaveCountEffect++;
                        yield return new WaitForSeconds(curAttackData.timeBetweenHitUI);
                    }

                    // Trigger effect
                    TriggerEffect();    // does nothing atm
                }
                _combatManager.activeAttackData.Clear();
            }
        }

        hitCount = 0;
        hitWaveCount = 0;
        hitWaveCountEffect = 0;
        for (int i = 0; i < _combatManager.targetSelections.Count; i++)
        {
            _combatManager.targetSelections[i].GetUnitScript().effectHitCount = 0;
        }

        yield return new WaitForSeconds(_combatManager.postHitTime);
        _combatManager.activeAttackBar.MoveAttackBar(false);
    }

    /// <summary>
    /// Adjust max current health of unit
    /// </summary>
    public void UpdateCurHealth(bool inCombat, float val, bool isEffect = false, Transform activeSkillUIParent = null, AttackData attackData = null, Unit caster = null)
    {
        float valDefault;

        if (!attackData)
            valDefault = RoundFloatToInt(val);
        else
            valDefault = RoundFloatToInt(val * attackData.relicActiveSkillValueModifier);

        float valDefaultAbs = Mathf.Abs(valDefault);
        float effectVal = RoundFloatToInt(recievedDamageAmp * valDefaultAbs);

        if (inCombat)
        {
            if (isEffect)
                curHealth -= RoundFloatToInt(effectVal);
            else
                curHealth += RoundFloatToInt(valDefault);

            // Send effect UI
            if (isEffect)
                _combatManager.skillUIManager.DisplaySkillValue(caster, activeSkillUIParent, 0, effectVal, true, attackData);
            else
                _combatManager.skillUIManager.DisplaySkillValue(caster, activeSkillUIParent, valDefault, effectVal, false, attackData);
        }
        else
            curHealth += RoundFloatToInt(Mathf.Abs(val));

        UpdateCurHealthVisual(curHealth / maxHealth, true);
    }
     
    public void UpdateCurHealthVisual(float val, bool inCombat = true)
    {
        healthImage.fillAmount = val;
    }

    public void AssignEffect(Effect effect, int effectPower, int effectDuration, SkillData skillData)
    {
        float rand = Random.Range(0f, 1f);
        // If unit successfully rolled for the proc
        if (rand <= _combatManager.relicActiveSkillProcModifier)
        {
            _devManager.FlashText();

            UpdateEffectVisual(effect, effectPower, effectDuration, skillData);
        }
        // If unit failed the roll for the proc
        else
            _devManager.FlashText();
    }

    public void TriggerEffect()
    {


    }

    public void UpdateEffectVisual(Effect effect, int effectPower, int effectDuration, SkillData skillData)
    {
        // If unit is currently already inflicted with an effect, increase the power of the same effect instead of adding a new effect
        for (int i = 0; i < effects.Count; i++)
        {
            if (effect.name == effects[i].name)
            {
                EffectImage effectImage = effectImages[i].GetComponent<EffectImage>();

                effectImage.UpdateEffectPower(effectPower);

                effectImage.Functionality(skillData);

                return;
            }
        }

        // If this effect is unique to the current effects on the unit, spawn another one.
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

                effectImage.Functionality(skillData);

                return;
            }
        }
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
                effectImage.unit = this;
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
