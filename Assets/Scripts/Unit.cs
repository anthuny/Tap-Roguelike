using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public enum UnitType { ALLY, ENEMY };
    public UnitType unitType;

    public enum UnitState { ALIVE, DEAD };
    public UnitState unitState;

    [Header("Main")]
    public new string name;
    public int level = 1;

    [Space(3)]

    [Header("Main Stats")]
    [Tooltip("The default power the unit has")]
    public int power;
    [Tooltip("The default maximum health the enemy spawns with")]
    public float maxHealth;
    public float curHealth;
    public int curMana;
    public int maxMana;
    [Tooltip("Mana gained at the start of unit's turn")]
    public int manaGainTurn;
    public string inflictedType;
    [Tooltip("The percentage rate of whether the enemy will attack before the player")]
    public int energy;
    public int turnEnergy;

    [Space(3)]

    [Header("Growth Stats")]
    public int powerGrowth;
    public int healthGrowth;
    public int manaGrowth;
    public int maxManaGrowth;

    [Space(3)]

    [Header("Aesthetics")]
    public Color color;

    [Space(3)]

    [Header("The liklihood of what the enemy will do on their turn")]
    public int attackChance;
    public int skillChance;

    [Space(3)]

    [Header("UI")]
    private Image healthImage;
    private Text nameText;
    private Image energyImage;
    private Image manaImage;
    public Transform effectParent;
    public List<Image> effectImages = new List<Image>();
    public Transform skillUIValueParent;
    [SerializeField] private GameObject turnImage;

    [Space(3)]

    [SerializeField] private GameObject unitGO;

    [Space(3)]

    [Header("Effects")]
    public List<Effect> effects = new List<Effect>();
    public List<Text> effectPowersText = new List<Text>();
    public List<int> effectPowers = new List<int>();
    public float recievedDamageAmp;

    [Space(3)]

    [Header("Skills")]
    public SkillData passiveSkill;
    public SkillData basicSkill;
    public SkillData primarySkill;
    public SkillData secondarySkill;
    public SkillData alternateSkill;
    public SkillData ultimateSkill;

    [Space(3)]

    [HideInInspector]
    public Target target;
    private DevManager _devManager;
    public SkillData activeSkill;
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
    public int attackCount;
    [HideInInspector]
    public int hitRecievedCount;
    bool storingAttack;
    [HideInInspector]
    public int effectHitCount;

    [HideInInspector]
    public List<SkillValueUI> storedskillValueUI = new List<SkillValueUI>();

    private void Awake()
    {
        _devManager = FindObjectOfType<DevManager>();
        _combatManager = FindObjectOfType<CombatManager>();
        _skillUIManager = FindObjectOfType<SkillUIManager>();
    }

    public IEnumerator DetermineUnitMoveChoice(Unit unit, SkillData skillData = null)
    {
        if (unitType == UnitType.ENEMY)
        {
            yield return new WaitForSeconds(_combatManager.enemySkillAnimationTime);

            // If enemy unit has enough mana for basic skill
            if (curMana >= basicSkill.manaRequired)
                StartCoroutine(UnitSkillFunctionality(false, basicSkill, _combatManager.enemyThinkTime));    //Use enemy's basic skill
            // If enemy unit has no mana for any skill
            else
                _combatManager.EndTurn();    // If combat did not end, end turn 
        }
    }

    void SelectEnemySkillValueMultiplier()
    {
        // If unit is enemy, set act ive skill value modifier to be the good value
        if (unitType == UnitType.ENEMY)
            _combatManager.activeSkillValueModifier = _combatManager.activeSkill.goodValueMultiplier;
    }

    /// <summary>
    /// Caster performs a skill on a target
    /// </summary>
    public IEnumerator UnitSkillFunctionality(bool relic, SkillData skillData, float time = 0)
    {
        yield return new WaitForSeconds(time);

        // If enemy's skill, set the target to the relic 
        if (unitType == UnitType.ENEMY)
        {
            // Set target 
            _combatManager.activeRelic.target.ToggleSelectionImage();
            _combatManager.unitTargets.Add(_combatManager.activeRelic.target);

            // Update mana on first hit for skill mana cost
            if (skillData.curHitsCompleted == 0)
                StartCoroutine(_combatManager.activeUnit.UpdateCurMana(skillData.manaRequired, false));
                //_combatManager.activeAttackBar.MoveAttackBar(true);
        }

        yield return new WaitForSeconds(time);

        _combatManager.SetActiveSkill(skillData);   // Set active skill

        SelectEnemySkillValueMultiplier();  // Set enemy skill value multiplier

        AssignSelectionCount(skillData);

        skillData.curHitsCompleted++;   // Increase current hits completed by 1

        yield return new WaitForSeconds(time);  // Time for skill animation 

        // Only if relic, set up for attack bar
        if (relic)
        {
            if (skillData.curHitsCompleted == skillData.hitsRequired)
            {
                _skillUIManager.SetSkillMaxCooldown(skillData);
                _skillUIManager.UpdateSkillCooldown(skillData);
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
        }

        switch (skillData.targetsAllowed)
        {
            case "Enemies":

                switch (skillData.targetType)
                {
                    case "Single":

                        Unit targetSingle = _combatManager.unitTargets[0].GetComponentInParent<Unit>();

                        attackCount++;

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

                        StartCoroutine(SendSkillUI(_combatManager.relicActiveSkill));

                        //_devManager.FlashText(this.name, targetSingle.name, skillData.name, power, targetSingle);   // Debug the attack

                        break;


                    case "Multiple":

                        for (int i = 0; i < _combatManager.unitTargets.Count; i++)
                        {
                            Unit targetMultiple = _combatManager.unitTargets[i].GetComponentInParent<Unit>();
                            attackCount++;

                            switch (skillData.skillMode)
                            {
                                case "Damage":
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

                            StartCoroutine(SendSkillUI(_combatManager.relicActiveSkill));

                            // If skill does not have an effect
                            //_devManager.FlashText(this.name, targetMultiple.name, skillData.name, power, targetMultiple);   // Debug the attack
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

        if (relic && skillData.curHitsCompleted == skillData.hitsRequired)
            _combatManager.activeAttackBar.MoveAttackBar(false);

        // Disable the back button after the first hit of the skill
        if (skillData.curHitsCompleted == 1 && unitType == UnitType.ALLY)
        {
            yield return new WaitForSeconds(_combatManager.activeAttackBar.timeTillAttackBarReturns);
            _combatManager.activeAttackBar.ToggleBackButton(false, true);
        }

        // If this attack was the last required hit
        if (skillData.curHitsCompleted == skillData.hitsRequired)
        {
            _combatManager.activeAttackBar.UpdateActiveSkill(false);
            _combatManager.ClearUnitTargets();
            _skillUIManager.UpdateSkillStatus(SkillUIManager.SkillStatus.DISABLED);   // Update skill status to disabled
            skillData.curHitsCompleted = 0;
            _skillUIManager.HideValueUI();
            _skillUIManager.ResetTextOffset();

            // If enemy skill, go back to thought process after last hit of a skill
            if (unitType == UnitType.ENEMY)
                StartCoroutine(DetermineUnitMoveChoice(this, skillData));
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
            curAttackData.val = _combatManager.CalculateDamageDealt(val, _combatManager.activeSkillValueModifier);
        else
            curAttackData.val = _combatManager.CalculateDamageDealt(val, _combatManager.activeSkillValueModifier, skillData.targetAmountPowerInc, _combatManager.unitTargets.Count);

        curAttackData.effect = skillData.effect;
        curAttackData.skillName = skillData.name;
        //if (skillData.effect.name != "")
            //curAttackData.skillEffectName = skillData.effect.name;
        curAttackData.effectPower = skillData.effectPower;
        curAttackData.effectDuration = skillData.effectDuration;
        curAttackData.curHitsCompleted = skillData.curHitsCompleted;
        curAttackData.hitsRequired = skillData.hitsRequired;
        curAttackData.timeBetweenHitUI = skillData.timeBetweenHitUI;
        curAttackData.timeTillEffectInflict = skillData.timeTillEffectInflict;
        curAttackData.skillUIValueParent = target.skillUIValueParent;
        curAttackData.curTargetCount = _combatManager.unitTargets.Count;
        //curAttackData.effectVal = skillData.effect.stackValue;
        curAttackData.relicActiveSkillValueModifier = _combatManager.activeSkillValueModifier;
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
                _combatManager.activeAttackData[i].target.hitRecievedCount++;

                // send values to target
                _combatManager.activeAttackData[i].target.UpdateCurHealth(_combatManager.activeAttackData[i].inCombat,
                    _combatManager.activeAttackData[i].val, false, _combatManager.activeAttackData[i].skillUIValueParent,
                    _combatManager.activeAttackData[i], this, _combatManager.activeAttackData[i].target);

                // Attempt Assign effect
                /*
                _combatManager.activeAttackData[i].target.AssignEffect(_combatManager.activeAttackData[i].effect,
                    _combatManager.activeAttackData[i].effectPower, _combatManager.activeAttackData[i].effectDuration,
                    _combatManager.activeAttackData[i].skillData);
                    */
            }
        }

        _combatManager.activeAttackData.Clear();
        attackCount = 0;
        hitWaveCount = 0;
        hitWaveCountEffect = 0;
    }

    public void UpdateUnitState(UnitState unitState)
    {
        this.unitState = unitState;

    }

    #region Update Unit Type
    public void UpdateUnitType(UnitType unitType)
    {
        this.unitType = unitType;
    }
    #endregion
    #region Update Level
    public void UpdateLevel(int level)
    {
        this.level = level;
    }
    #endregion
    #region Update Name
    public void UpdateName(string displayName)
    {
        this.name = displayName;
        nameText.text = this.name;
    }
    #endregion
    #region Update Power
    public void UpdatePower(int val)
    {
        power += val;
    }
    #endregion
    #region Update Mana
    public void UpdateEnergy(int val)
    {
        energy += val;
    }

    public void UpdateTurnEnergy(int val)
    {
        turnEnergy = val;
        UpdateEnergyVisual();
    }

    public void CalculateTurnEnergy()
    {
        turnEnergy += energy;
        UpdateEnergyVisual();
    }

    void UpdateEnergyVisual()
    {
        energyImage.fillAmount = (float)turnEnergy / 100f;
    }

    public bool HasEnoughManaForSkill()
    {
        if (curMana >= _combatManager.activeSkill.manaRequired)
            return true;
        else
            return false;
    }

    #endregion
    #region Update Health Stat
    /// <summary>
    /// Adjust max current health of unit
    /// </summary>
    public void UpdateCurHealth(bool inCombat, float val, bool isEffect = false, Transform activeSkillUIParent = null, AttackData attackData = null, Unit caster = null, Unit target = null)
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
            curHealth += RoundFloatToInt(valDefault);

            // If health change is result of last hit of skill, check if unit died
            if (attackData.curHitsCompleted == attackData.hitsRequired)
            {
                // If unit's health is 0 or lower, destroy unit
                if (curHealth <= 0)
                {
                    curHealth = 0;

                    UpdateUnitState(UnitState.DEAD);

                    if (unitType == UnitType.ENEMY)
                        StartCoroutine(DestroyUnit(false));
                }
            }
            _combatManager.skillUIManager.DisplaySkillValue(caster, target, activeSkillUIParent, valDefault, attackData);
        }
        else
            curHealth += RoundFloatToInt(Mathf.Abs(val));

        UpdateCurHealthVisual();
    }

    /// <summary>
    /// Adjust max health of unit
    /// </summary>
    public void UpdateMaxHealth(float val)
    {
        maxHealth += val;
        UpdateCurHealthVisual();
    }

    public void UpdateCurHealthVisual()
    {
        float val = (curHealth / maxHealth);
        healthImage.fillAmount = val;
    }
    public IEnumerator DestroyUnit(bool combatEnd)
    {
        if (unitType == UnitType.ENEMY)
        {
            if (combatEnd)
                yield return new WaitForSeconds(0);
            else
                yield return new WaitForSeconds(_combatManager.enemyDeathTime);
        }

        else if (unitType == UnitType.ALLY)
            yield return new WaitForSeconds(0);

        // Remove from unit selection if in one
        if (target.selectEnabled)
            target.ToggleSelectionImage();

        DestroyAllSkillValueUI();   // Destroy persisting skill value UI

        // Remove from stored variables
        _combatManager.RemoveEnemy(this);    // remove from enemies  
        _combatManager.RemoveUnitTurnOrder(this);    // remove from turn order
        _combatManager.RemoveEnemyPosition(this);     // remove from enemiesposition

        // If dead unit is an enemy, and it's the last one, end turn
        if (unitType == UnitType.ENEMY)
            if (_combatManager.enemyCount() == 0)
                _combatManager.EndTurn();

        Destroy(this.gameObject);
    }

    void DestroyAllSkillValueUI()
    {
        for (int i = 0; i < storedskillValueUI.Count; i++)
        {
            if (_skillUIManager.storedSkillValueUI.Contains(storedskillValueUI[i]))
                storedskillValueUI[i].DestroySkillUI();
        }
    }

    #endregion
    #region Update Energy Stats
    public IEnumerator UpdateCurMana(int mana, bool positive = true)
    {
        // Increase current mana in intervals
        if (positive)
        {
            for (int i = 0; i < mana; i++)
            {
                curMana++;
                if (curMana > maxMana)
                {
                    curMana = maxMana;
                    break;
                }
                UpdateManaVisual();

                yield return new WaitForSeconds(_combatManager.ManaUpdateInterval);
            }
        }
        else
            for (int i = 0; i < mana; i++)
            {
                curMana--;
                if (curMana < 0)
                {
                    curMana = 0;
                    break;
                }
                UpdateManaVisual();

                yield return new WaitForSeconds(_combatManager.ManaUpdateInterval);
            }
    }

    public void UpdateMaxMana(int addedMaxMana)
    {
        maxMana += addedMaxMana;

        UpdateManaVisual();
    }

    public void UpdateEnergyTurnGrowth(int energyGainTurn)
    {
        this.manaGainTurn += energyGainTurn;
    }

    void UpdateManaVisual()
    {
        manaImage.fillAmount = (float)curMana / (float)maxMana;
    }
    #endregion
    #region Update Growth Stats
    public void UpdatePowerGrowth(int powerGrowth)
    {
        this.powerGrowth = powerGrowth;
    }

    public void UpdateHealthGrowth(int healthGrowth)
    {
        this.healthGrowth = healthGrowth;
    }

    public void UpdateManaGrowth(int manaGrowth)
    {
        this.manaGrowth = manaGrowth;
    }

    public void UpdateMaxEnergyGrowth(int maxEnergyGrowth)
    {
        this.maxManaGrowth = maxEnergyGrowth;
    }
    #endregion
    #region Update Colour
    public void UpdateColour(Color color)
    {
        this.color = color;
    }
    #endregion
    #region Assign Effect
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
    #endregion

    public void ToggleTurnImage(bool toggle)
    {
        turnImage.SetActive(toggle);
    }
    void AssignSelectionCount(SkillData skill)
    {
        _combatManager.maxUnitTargets = skill.maxTargetCount;
    }

    public void AssignUI()
    {
        unitGO.transform.SetParent(transform);
        healthImage = unitGO.transform.Find("Health/Current Health Image").GetComponent<Image>();
        nameText = unitGO.transform.Find("Name/Name Text").GetComponent<Text>();
        energyImage = unitGO.transform.Find("Energy/Current Energy Image").GetComponent<Image>();
        manaImage = unitGO.transform.Find("Mana/Current Mana Image").GetComponent<Image>();

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
