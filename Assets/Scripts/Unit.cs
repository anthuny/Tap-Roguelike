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

    private UnitHUDInfo _unitHudInfo;

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
    [SerializeField] private Image healthImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Image energyImage;
    [SerializeField] private Image manaImage;
    [SerializeField] private UnitSelect _unitSelect;
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

    [HideInInspector]
    public Sprite portraitSprite;
    private void Awake()
    {
        _devManager = FindObjectOfType<DevManager>();
        _combatManager = FindObjectOfType<CombatManager>();
        _skillUIManager = FindObjectOfType<SkillUIManager>();
        _unitHudInfo = FindObjectOfType<UnitHUDInfo>();
    }

    public IEnumerator DetermineUnitMoveChoice()
    {
        if (unitType == UnitType.ALLY)
        {
            //Use enemy's basic skill
            //StartCoroutine(UnitSkillFunctionality(_combatManager.activeSkill));
        }
        else
        {
            // Toggle off end turn Button
            StartCoroutine(_combatManager.uIManager.ToggleImage(_combatManager.uIManager.endTurnGO, false));

            // enemy choose skill time wait
            yield return new WaitForSeconds(_combatManager.enemySkillDetailsTime);

            // If enemy unit has enough mana for basic skill
            if (curMana >= basicSkill.manaRequired)
            {
                // Enable selection on basic skill icon 
                _unitHudInfo.ToggleSkillIconSelection(_unitHudInfo.tBasicSkillIcon, true);

                // Update Unit's mana for skill cost
                StartCoroutine(UpdateCurMana(basicSkill.manaRequired, false));

                // Unit post skill select time wait
                yield return new WaitForSeconds(_combatManager.enemySelectSkillTime);

                // Start basic skill functionality
                _unitHudInfo.tBasicSkillIcon.ActiveSkillToggleUI();

                // Unit post skill details time wait
                yield return new WaitForSeconds(_combatManager.enemyStartAttackTime);

                //Use enemy's basic skill
                StartCoroutine(UnitSkillFunctionality(basicSkill));    
            }

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

    public IEnumerator UnitSkillFunctionality(SkillData skillData)
    {
        // Only if relic, set up for attack bar
        if (unitType == UnitType.ALLY)
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
                maxWaveCountEffects = skillData.hitsRequired - 1;
        }

        // If enemy's skill, set the target to the relic 
        if (unitType == UnitType.ENEMY)
        {
            // Set target 
            _combatManager.activeRelic.target.ToggleSelectionImage(false);
            _combatManager.unitTargets.Add(_combatManager.activeRelic.target);

            // Update mana on first hit for skill mana cost
            if (skillData.curHitsCompleted == 0)
                
            // Deselect selected skill icon
            _unitHudInfo.DeselectAllSelections();
        }

        _combatManager.SetActiveSkill(skillData);   // Set active skill
        SelectEnemySkillValueMultiplier();  // Set enemy skill value multiplier

        skillData.curHitsCompleted++;   // Increase current hits completed by 1

        switch (skillData.targetsAllowed)
        {
            case "Enemies":

                switch (skillData.targetType)
                {
                    case "Single":

                        attackCount++;

                        #region Store skill values based on the value type of the skill
                        switch (skillData.skillMode)
                        {
                            case "Damage":
                                StoreSkillCause(_combatManager.targetUnits[0], this, skillData, -power, true); 
                                break;

                            case "PercentMaxHealthDamage":
                                StoreSkillCause(_combatManager.targetUnits[0], this, skillData, -((power / 100) * _combatManager.targetUnits[0].maxHealth), true);
                                break;

                            case "Heal":
                                StoreSkillCause(_combatManager.targetUnits[0], this, skillData, power, true);
                                break;

                            case "PercentMaxHealHeal":
                                StoreSkillCause(_combatManager.targetUnits[0], this, skillData, ((power / 100) * _combatManager.targetUnits[0].maxHealth), true);
                                break;
                        }
                        #endregion

                        // Display Skill Value UI
                        StartCoroutine(SendSkillUI(_combatManager.activeSkill));

                        // toggle this unit's select image off
                        _combatManager.targetUnits[0].ToggleSelectImage(false);   

                        break;


                    case "Multiple":

                        attackCount++;

                        #region Store skill values based on the value type of the skill
                        Unit targetMultiple;

                        // If unit is ally, loop through all enemies
                        if (_combatManager.activeUnit.unitType == UnitType.ALLY)
                        {
                            for (int i = 0; i < _combatManager._enemies.Count; i++)
                            {
                                targetMultiple = _combatManager._enemies[i];

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

                                // toggle this unit's select image off
                                targetMultiple.ToggleSelectImage(false);
                            }
                        }

                        // If unit is enemy, target active relic | TODO : Refactor so multiple relics are looped through
                        else
                        {
                            targetMultiple = _combatManager.activeRelic;

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

                            // toggle this unit's select image off
                            targetMultiple.ToggleSelectImage(false);
                        }
                        #endregion

                        // Display Skill Value UI
                        StartCoroutine(SendSkillUI(_combatManager.activeSkill));
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

        // If unit is ally, and this is not the final required hit of the active skill, flash off the attack bar
        if (unitType == UnitType.ALLY && skillData.curHitsCompleted != skillData.hitsRequired)
        {
            _combatManager.activeAttackBar.FlashOffAttackBar();
        }

        // Hide attack bar if this is the last required hit of active skill
        if (unitType == UnitType.ALLY && skillData.curHitsCompleted == skillData.hitsRequired)
            _combatManager.activeAttackBar.BackButtonFunctionality();

        // Disable the back button after the first hit of the skill
        if (skillData.curHitsCompleted == 1 && unitType == UnitType.ALLY)
            _combatManager._unitHudInfo.TogglePanel(_combatManager._unitHudInfo.cancelAttackGO, false);

        // If this attack was the last required hit
        if (skillData.curHitsCompleted == skillData.hitsRequired)
        {
            _combatManager.activeAttackBar.UpdateActiveSkill(false);
            skillData.curHitsCompleted = 0;
            _skillUIManager.HideValueUI();
            _skillUIManager.ResetTextOffset();

            // If enemy skill, go back to thought process after last hit of a skill
            if (unitType == UnitType.ENEMY)
                StartCoroutine(DetermineUnitMoveChoice());
        }
    }

    void StoreSkillCause(Unit target, Unit caster, SkillData skillData, float val = 0, bool inCombat = true)
    {
        curAttackData = _combatManager.gameObject.AddComponent<AttackData>();
        _combatManager.activeAttackData.Add(curAttackData);

        curAttackData.skillData = skillData;
        curAttackData.inCombat = inCombat;
        curAttackData.target = target;

        // Calculate value of skill
        curAttackData.val = _combatManager.CalculateDamageDealt(val, _combatManager.activeSkillValueModifier);

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

        curAttackData.activeSkillValueModifier = _combatManager.activeSkillValueModifier;

        curAttackData.curTargetCount = _combatManager.unitTargets.Count;
        //curAttackData.effectVal = skillData.effect.stackValue;

    }

    IEnumerator SendSkillUI(SkillData skillData)
    {
        for (int i = 0; i < _combatManager.activeAttackData.Count; i++)
        {
            // If unit is a relic, and after looping through all enemy targets
            if (unitType == UnitType.ALLY && i % _combatManager.enemyCount() == 0 && i != 0)
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

    public void SetPortraitImage(Sprite portraitSprite)
    {
        this.portraitSprite = portraitSprite;
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
    #region Update Energy
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
            valDefault = RoundFloatToInt(val * attackData.activeSkillValueModifier);

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
        if (_combatManager.targetUnits.Contains(this))
            _combatManager.RemoveTarget(this);

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
    #region Update Mana
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

                if (_combatManager.activeUnit == this)
                {
                    _unitHudInfo.SetUnitMana(_unitHudInfo.tUnitManaText, curMana);
                    _unitHudInfo.UpdateSkillInvalidImages(this);
                    //_unitHudInfo.UpdateSkillManaRequired(this);
                }

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
                if (_combatManager.activeUnit == this)
                {
                    _unitHudInfo.SetUnitMana(_unitHudInfo.tUnitManaText, curMana);
                    _unitHudInfo.UpdateSkillInvalidImages(this);
                    //_unitHudInfo.UpdateSkillManaRequired(this);
                }

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
    public void ToggleSelectImage(bool enable)
    {
        _unitSelect.ToggleTurnImage(enable);
    }
    public void ToggleTurnImage(bool enable)
    {
        turnImage.SetActive(enable);
    }

    #region Assign Effect
    public void AssignEffect(Effect effect, int effectPower, int effectDuration, SkillData skillData)
    {
        float rand = Random.Range(0f, 1f);
        // If unit successfully rolled for the proc
        if (rand <= _combatManager.activeSkillProcModifier)
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



    public void AssignUI()
    {
        unitGO.transform.SetParent(transform);
        //healthImage = unitGO.transform.Find("Health/Current Health Image").GetComponent<Image>();
        //nameText = unitGO.transform.Find("Name/Name Text").GetComponent<Text>();
        //energyImage = unitGO.transform.Find("Energy/Current Energy Image").GetComponent<Image>();
        //manaImage = unitGO.transform.Find("Mana/Current Mana Image").GetComponent<Image>();
        //selectImage = unitGO.transform.Find("Select/Select Image").GetComponent<Image>();

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
