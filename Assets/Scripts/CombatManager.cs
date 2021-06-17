using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public enum CombatState { NONE, RELICWIN, ENENYWIN}
    public CombatState combatState;
    private enum PrevUnitType { ALLY, ENEMY }
    private PrevUnitType prevUnitType;

    private DevManager _devManager;
    [HideInInspector]
    public UnitHUDInfo _unitHudInfo;
    [HideInInspector]
    public UIManager uIManager;
    private AnimatorController _animatorController;

    [Header("General")]
    [SerializeField] private Transform _unitPositions;
    [SerializeField] private Transform _enemySpawnPoint;
    [SerializeField] private Transform _relicSpawnPoint;
    [SerializeField] private Room _activeRoom;
    [SerializeField] private AttackBar _attackBar;
    [SerializeField] private Button _fightButton;
    [SerializeField] private Transform enemyTurnUnitPosition;
    [SerializeField] private Transform relicTurnUnitPosition;
    public float swapTeamUnitMovespeed;
    [SerializeField] private Animator unitPositionsMovement;
    [SerializeField] private string animationVarName;
    private bool movedForRelicTurn;
    [HideInInspector]
    public Unit activeUnit;
    [HideInInspector]
    public List<Unit> targetUnits = new List<Unit>();
    public GameObject relicGO;

    [Space(3)]
    [Header("Combat Settings")]
    public float displayAttackBarTimeWait = 1;
    public float unitSelectImageActiveAlpha;
    public float unitSelectImageInactiveAlpha;
    [SerializeField] private float enemyUnitStartWait;
    [SerializeField] private float postAllyAttackWait;

    [Space(3)]

    [Header("Relic Settings")]
    [Tooltip("The time that must elapse before another thing happens")]
    public float breatheTime = .25f;
    [Tooltip("The amount of time in seconds that must elapse after the enemy spawns before the next stage can begin")]
    public float enemyTimeWaitSpawn;

    [Space(3)]

    [Header("Combat")]
    public float attackBarHideTime;
    public float postHitTime;
    public float enemySelectSkillTime;
    public float enemySkillDetailsTime;
    public float enemyStartAttackTime;
    public float enemySkillAnimationTime;
    public float ManaUpdateInterval;
    public float enemyDeathTime;

    [Space(3)]

    [Header("Effects Settings")]
    public int maxEffectsActive;
    //[HideInInspector]
    public SkillData activeSkill;
    //[HideInInspector]
    public List<Target> unitTargets = new List<Target>();
    [HideInInspector]
    public List<Target> skillTargets = new List<Target>();

    [Header("Active Stored Attacks")]
    public List<AttackData> activeAttackData = new List<AttackData>();

    //[HideInInspector]
    public int maxUnitTargets;
    //[HideInInspector]
    public int curUnitTargets;
    [HideInInspector]
    public int maxSkillTargets;
    [HideInInspector]
    public int curSkillTargets;
    [HideInInspector]
    public int activeRoomMaxEnemiesCount;
    [HideInInspector]
    public int oldCurTargetSelections;

    [Space(3)]

    [Header("Other Intitialization")]
    public Relic startingRelic;
    public GameObject skillUIValuesPar;
    public GameObject unitUI;
    [HideInInspector]
    public SkillUIManager skillUIManager;
    //[HideInInspector]
    public AttackBar activeAttackBar;
    [HideInInspector]
    public float activeSkillValueModifier, activeSkillProcModifier;

    private bool relicInitialized;
    //[HideInInspector]
    public bool relicTurn;
    [HideInInspector]
    public Target activeSkillTarget;
    [HideInInspector]
    public Target relicSelector;

    [HideInInspector]
    public List<Unit> _enemiesPosition = new List<Unit>();      //Original position of enemies in active room
    [HideInInspector]
    public List<Unit> _enemies = new List<Unit>();
    public List<Unit> turnOrder = new List<Unit>();
    [HideInInspector]
    public Unit activeRelic, activeEnemy;
    private int targetAmount;

    [Header("Skill Keywords")]
    public string[] skillType;
    public string[] skillModes;
    public string[] targetType;
    public string[] attackSequence;
    public string[] targetsAllowed;
    public string[] inflictType;

    private void Awake()
    {
        _devManager = FindObjectOfType<DevManager>();
        skillUIManager = FindObjectOfType<SkillUIManager>();
        uIManager = FindObjectOfType<UIManager>();
        _unitHudInfo = FindObjectOfType<UnitHUDInfo>();      
        _animatorController = FindObjectOfType<AnimatorController>();

        StartCoroutine(uIManager.ToggleImage(uIManager.startFightGO, true));   // Toggle start button on
        StartCoroutine(uIManager.ToggleImage(uIManager.selectedUnitPortraitsGO, true));
    }

    public void StartBattle()
    {
        ToggleFightButton(false);
        SpawnRelic(startingRelic);
        SpawnEnemies(_activeRoom);
        StartCoroutine(uIManager.ToggleImage(uIManager.endTurnGO, true));
        EndTurn();
    }

    public bool DetermineIfCombatEnded()
    {
        if (_enemies.Count == 0)
        {
            combatState = CombatState.RELICWIN;
            return true;
        }
        else if (activeRelic.unitState == Unit.UnitState.DEAD)
        {
            combatState = CombatState.ENENYWIN;
            return true;
        }
        else
            return false;
    }

    public int enemyCount()
    {
        return _enemies.Count;
    }

    public void RemoveEnemy(Unit unit)
    {
        _enemies.Remove(unit);
    }

    void RemoveAllEnemies()
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            StartCoroutine(_enemies[i].DestroyUnit(true));
        }
    }
    public void RemoveUnitTurnOrder(Unit unit)
    {
        turnOrder.Remove(unit);
    }

    public void RemoveEnemyPosition(Unit unit)
    {
        _enemiesPosition.Remove(unit);
    }

    public void EndCombat(CombatState cState)
    {
        UpdateCombatState(cState);

        Debug.Log("Combat Ended " + cState);

        if (combatState == CombatState.RELICWIN)
            StartCoroutine(activeRelic.DestroyUnit(true));

        else if (combatState == CombatState.ENENYWIN)
            RemoveAllEnemies();

        StartCoroutine(uIManager.ToggleImage(uIManager.startFightGO, true));     // Enable start fight button

        _attackBar.Refresh();   // Refresh attack bar 
    }

    void UpdateCombatState(CombatState combatState)
    {
        this.combatState = combatState;
    }
    public void SetActiveAttackBar(AttackBar attackBar)
    {
        activeAttackBar = attackBar;
    }

    public void ToggleUnitSelectImages(SkillData skillData)
    {
        switch (skillData.targetsAllowed)
        {
            case "Enemies":
                // If ally, toggle enemies select
                if (activeUnit.unitType == Unit.UnitType.ALLY)
                    for (int i = 0; i < _enemies.Count; i++)
                    {
                        _enemies[i].ToggleSelectImage(true);

                        if (activeUnit.HasEnoughManaForSkill())
                        {
                            _enemies[i].ToggleTargetable(true);
                            _enemies[i].UpdateSelectImageAlpha(true);
                        }
                        else
                        {
                            _enemies[i].ToggleTargetable(false);
                            _enemies[i].UpdateSelectImageAlpha(false);
                        }
                    }
                // If enemy, toggle allies select
                else
                {
                    activeRelic.ToggleSelectImage(true);
                    activeRelic.target.targetable = true;
                }
                break;

            case "Allies":
                // If ally, toggle allies select
                if (activeUnit.unitType == Unit.UnitType.ALLY)
                {
                    activeRelic.ToggleSelectImage(true);
                    activeRelic.target.targetable = true;
                    if (activeUnit.HasEnoughManaForSkill())
                    {
                        activeRelic.ToggleTargetable(true);
                        activeRelic.UpdateSelectImageAlpha(true);
                    }
                    else
                    {
                        activeRelic.ToggleTargetable(false);
                        activeRelic.UpdateSelectImageAlpha(false);
                    }
                }
                // If enemy, toggle enemies select
                else
                    for (int i = 0; i < _enemies.Count; i++)
                    {
                        _enemies[i].ToggleSelectImage(true);
                        _enemies[i].target.targetable = true;
                    }
                break;
        }
    }
    public void ClearUnitSelectImages()
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            _enemies[i].ToggleSelectImage(false);
            _enemies[i].target.targetable = false;
        }

        activeRelic.ToggleSelectImage(false);
        activeRelic.target.targetable = false;
    }

    public void AddTarget(Unit unit)
    {
        targetUnits.Add(unit);
    }

    public void RemoveTarget(Unit unit)
    {
        targetUnits.Remove(unit);
    }

    public void ClearTargets()
    {
        targetUnits.Clear();
    }

    public IEnumerator BeginUnitTurn(bool enemyTeam)
    {
        // Assign next unit's skills
        _unitHudInfo.AssignUnitSkillsToSkillIcon(GetNextTurnUnit());

        // Disable relic attack bar + skills UI if its not relic turn
        if (enemyTeam)
        {
            // Hide relic attack bar / end turn UI
            StartCoroutine(uIManager.ToggleImage(uIManager.attackBarGO, false));
            StartCoroutine(uIManager.ToggleImage(uIManager.endTurnGO, false));

            // Display unit HUD info
            _unitHudInfo.TogglePanels(GetNextTurnUnit());
        }

        // Enable relic attack bar + skills UI on relic turn
        else
        {
            //StartCoroutine(uIManager.ToggleImage(uIManager.attackBarGO, true, 0.3f));
            //StartCoroutine(uIManager.ToggleImage(uIManager.relicSkillGO, true, 0.3f));
            StartCoroutine(uIManager.ToggleImage(uIManager.endTurnGO, true));
            //SetMaxUnitTargets(1);

            // Display unit HUD info
            _unitHudInfo.TogglePanels(GetNextTurnUnit());
        }

        // Update CDs
        //skillUIManager.AssignSkills(GetNextTurnUnit());     // assign skills for cd update
        //skillUIManager.DecrementSkillCooldown();    //Decrease all skill cooldowns

        activeUnit.UpdateTurnEnergy(0);   // Reset active unit's turn mana
        GetNextTurnUnit().ToggleTurnImage(true);    // Enable next unit's turn image

        //_unitHudInfo.SetUnitInfoUI(GetNextTurnUnit());

        if (enemyTeam)    
            yield return new WaitForSeconds(enemyUnitStartWait);    // Time to wait before unit is targeted as the active unit

        // Give next unit's mana for the start of their turn           
        StartCoroutine(GetNextTurnUnit().UpdateCurMana(GetNextTurnUnit().manaGainTurn));

        // Begin the next unit's turn, Trigger determine move
        StartCoroutine(GetNextTurnUnit().DetermineUnitMoveChoice());
    }

    public void SetActiveSkill(SkillData skillData)
    {
        if (activeSkill != skillData)
        {
            activeSkill = skillData;
            activeRelic.activeSkill = skillData;
        }
    }

    public void EndTurn()
    {
        ClearUnitSelectImages();   // Remove all unit select images
        UpdateTurnOrder();     // Update turn orders
        ToggleAllTurnImages(false);  // Disable turn image 
        activeAttackBar.DestroyActiveHitMarker();

        // Toggle off all unit Skill Icon UI Off
        uIManager.ToggleImage(uIManager.attackBarGO, false);
        _unitHudInfo.TogglePanel(_unitHudInfo.eAllSkillPanel, false);
        _unitHudInfo.TogglePanel(_unitHudInfo.rAllSkillPanel, false);
        _unitHudInfo.TogglePanel(_unitHudInfo.rActiveSkillPanel, false);

        // Set the active team for this turn
        if (GetNextTurnUnit().unitType == Unit.UnitType.ENEMY)
        {
            prevUnitType = PrevUnitType.ENEMY;
            SetTeamTurn(false);
        }
        else
        {
            prevUnitType = PrevUnitType.ALLY;
            SetTeamTurn(true);
        }

        // End combat if combat should be over
        if (DetermineIfCombatEnded())
        {
            EndCombat(combatState);
            return;
        }

        // If it's enemies turn, begin their turn
        if (!GetTeamTurn())
            StartCoroutine(BeginUnitTurn(true));
        // If it's relics turn, Do everyting except start the attack
        else
            StartCoroutine(BeginUnitTurn(false));
    }

    void ToggleAllTurnImages(bool enable)
    {
        for (int i = 0; i < turnOrder.Count; i++)
        {
            turnOrder[i].ToggleTurnImage(enable);
        }
    }

    void UpdateTurnOrder()
    {
        for (int i = 0; i < turnOrder.Count; i++)
        {
            turnOrder[i].CalculateTurnEnergy();
        }

        turnOrder.Sort(ApplyTurnOrder);     // Sort turn order list of units
        turnOrder.Reverse();

        SetActiveUnit(turnOrder[0]);    // Set active unit to first unit in stored list
    }

    void SetActiveUnit(Unit unit)
    {
        activeUnit = unit;
    }
    Unit GetNextTurnUnit()
    {
        if (turnOrder.Count >= 1)
            return turnOrder[0];
        else
            return null;
    }

    bool GetTeamTurn()
    {
        return relicTurn;
    }
    void SetTeamTurn(bool relicTurn)
    {
        this.relicTurn = relicTurn;
    }

    private void ToggleFightButton(bool cond)
    {
        _fightButton.gameObject.SetActive(cond);
    }

    private void InitializeUnitSkills(Unit unit)
    {
        unit.basicSkill = unit.gameObject.AddComponent<SkillData>();
        unit.primarySkill = unit.gameObject.AddComponent<SkillData>();
        unit.secondarySkill = unit.gameObject.AddComponent<SkillData>();
        unit.alternateSkill = unit.gameObject.AddComponent<SkillData>();
    }

    private int ApplyTurnOrder(Unit a, Unit b)
    {
        if (a.turnEnergy < b.turnEnergy)
        {
            return -1;
        }
        else if (a.turnEnergy > b.turnEnergy)
        {
            return 1;
        }
        return 0;
    }

    void UpdateActiveRelic(Unit relic)
    {
        if (activeRelic != relic)
            activeRelic = relic;
    }

    void UpdateActiveEnemy(Unit enemy)
    {
        if (activeEnemy != enemy)
            activeEnemy = enemy;
    }

    public float CalculateDamageDealt(float value = 0, float valueModifier = 1, float targetCountPowerInc = 1, float targetCountValAmp = 1)
    {
        return (value * valueModifier) * (targetCountValAmp * targetCountPowerInc);
    }

    public int CalculateTargetAmount()
    {
        int targetAmount = 0;

        if (activeSkill)
        {
            switch (activeSkill.targetType)
            {
                case "Single":
                    targetAmount = 1;
                    break;

                case "Multiple":
                    if (activeSkill.targetsAllowed == "Enemies")
                        targetAmount = _enemies.Count;
                    else
                        targetAmount = 1;
                    break;
                case "Random":
                    break;
            }
        }
        return targetAmount;
    }
    /// <summary>
    /// Spawn enemies in room
    /// </summary>
    void SpawnEnemies(Room room)
    {
        activeRoomMaxEnemiesCount = room.roomMaxEnemies;

        for (int i = 0; i < room.roomEnemies.Count; i++)
        {
            int val = i + 1;
            GameObject go = Instantiate(room.roomEnemyGO[i]);
            go.name = room.roomEnemies[i].name + " " + val.ToString();
            go.transform.SetParent(_enemySpawnPoint.GetChild(i).transform);
            go.transform.position = _enemySpawnPoint.GetChild(i).transform.position;

            #region Initialize Unit
            UpdateActiveEnemy(go.GetComponent<Unit>());

            activeEnemy.skillUIValueParent = go.transform.Find("Skill UI Values");

            activeEnemy.SetPortraitImage(room.roomEnemies[i].portraitSprite);

            activeEnemy.UpdateUnitType(Unit.UnitType.ENEMY);
            activeEnemy.AssignUI(); // Initialize Unit UI
            activeEnemy.UpdateName(room.roomEnemies[i].name);
            activeEnemy.UpdateLevel(room.roomEnemies[i].level);

            activeEnemy.UpdatePower(room.roomEnemies[i].power);
            activeEnemy.UpdateMaxHealth(room.roomEnemies[i].maxHealth);
            activeEnemy.UpdateCurHealth(false, room.roomEnemies[i].maxHealth);
            activeEnemy.UpdateMaxMana(room.roomEnemies[i].maxMana);
            StartCoroutine(activeEnemy.UpdateCurMana(room.roomEnemies[i].maxMana));
            activeEnemy.UpdateEnergyTurnGrowth(room.roomEnemies[i].manaGainTurn);
            activeEnemy.UpdatePowerGrowth(room.roomEnemies[i].powerGrowth);
            activeEnemy.UpdateHealthGrowth(room.roomEnemies[i].healthGrowth);
            activeEnemy.UpdateManaGrowth(room.roomEnemies[i].energyGrowth);
            activeEnemy.UpdateMaxEnergyGrowth(room.roomEnemies[i].maxManaGrowth);
            activeEnemy.UpdateColour(room.roomEnemies[i].color);
            activeEnemy.UpdateEnergy(room.roomEnemies[i].energy);
            activeEnemy.UpdateAttackChance(room.roomEnemies[i].attackChance, true);

            activeEnemy.target = go.GetComponentInChildren<Target>();

            InitializeUnitSkills(activeEnemy);

            #region Initialize Enemy Skills
            activeEnemy.basicSkill.InitializeSkill(
                room.roomEnemies[i].basicSkill.skillIconColour,
                room.roomEnemies[i].basicSkill.skillBorderColour,
                room.roomEnemies[i].basicSkill.skillSelectionColour,
                room.roomEnemies[i].basicSkill.sprite,
                room.roomEnemies[i].basicSkill.skillType,
                room.roomEnemies[i].basicSkill.skillMode,
                room.roomEnemies[i].basicSkill.targetType,
                room.roomEnemies[i].basicSkill.targetsAllowed,
                room.roomEnemies[i].basicSkill.hitsRequired,
                room.roomEnemies[i].basicSkill.manaRequired,
                room.roomEnemies[i].basicSkill.timeBetweenHitUI,
                room.roomEnemies[i].basicSkill.timeTillEffectInflict,
                room.roomEnemies[i].basicSkill.timeForNextHitMarker,
                room.roomEnemies[i].basicSkill.effect,
                room.roomEnemies[i].basicSkill.effectTarget,
                room.roomEnemies[i].basicSkill.effectPower,
                room.roomEnemies[i].basicSkill.effectDuration,
                room.roomEnemies[i].basicSkill.effectHitEffect,
                room.roomEnemies[i].basicSkill.effectDurationDecrease,
                room.roomEnemies[i].basicSkill.counterSkill,
                room.roomEnemies[i].basicSkill.stackValue,
                room.roomEnemies[i].basicSkill.targetAmountPowerInc,
                room.roomEnemies[i].basicSkill.isTargetCountValAmp,
                room.roomEnemies[i].basicSkill.maxTargetCount,
                room.roomEnemies[i].basicSkill.activatable,
                room.roomEnemies[i].basicSkill.name,
                room.roomEnemies[i].basicSkill.description,
                room.roomEnemies[i].basicSkill.turnCooldown,
                room.roomEnemies[i].basicSkill.missValueMultiplier,
                room.roomEnemies[i].basicSkill.goodValueMultiplier,
                room.roomEnemies[i].basicSkill.greatValueMultiplier,
                room.roomEnemies[i].basicSkill.perfectValueMultiplier,
                room.roomEnemies[i].basicSkill.missProcMultiplier,
                room.roomEnemies[i].basicSkill.goodProcMultiplier,
                room.roomEnemies[i].basicSkill.greatProcMultiplier,
                room.roomEnemies[i].basicSkill.perfectProcMultiplier);

            activeEnemy.primarySkill.InitializeSkill(
                room.roomEnemies[i].primarySkill.skillIconColour,
                room.roomEnemies[i].primarySkill.skillBorderColour,
                room.roomEnemies[i].primarySkill.skillSelectionColour,
                room.roomEnemies[i].primarySkill.sprite,
                room.roomEnemies[i].primarySkill.skillType,
                room.roomEnemies[i].primarySkill.skillMode,
                room.roomEnemies[i].primarySkill.targetType,
                room.roomEnemies[i].primarySkill.targetsAllowed,
                room.roomEnemies[i].primarySkill.hitsRequired,
                room.roomEnemies[i].primarySkill.manaRequired,
                room.roomEnemies[i].primarySkill.timeBetweenHitUI,
                room.roomEnemies[i].primarySkill.timeTillEffectInflict,
                room.roomEnemies[i].primarySkill.timeForNextHitMarker,
                room.roomEnemies[i].primarySkill.effect,
                room.roomEnemies[i].primarySkill.effectTarget,
                room.roomEnemies[i].primarySkill.effectPower,
                room.roomEnemies[i].primarySkill.effectDuration,
                room.roomEnemies[i].primarySkill.effectHitEffect,
                room.roomEnemies[i].primarySkill.effectDurationDecrease,
                room.roomEnemies[i].primarySkill.counterSkill,
                room.roomEnemies[i].primarySkill.stackValue,
                room.roomEnemies[i].primarySkill.targetAmountPowerInc,
                room.roomEnemies[i].primarySkill.isTargetCountValAmp,
                room.roomEnemies[i].primarySkill.maxTargetCount,
                room.roomEnemies[i].primarySkill.activatable,
                room.roomEnemies[i].primarySkill.name,
                room.roomEnemies[i].primarySkill.description,
                room.roomEnemies[i].primarySkill.turnCooldown,
                room.roomEnemies[i].primarySkill.missValueMultiplier,
                room.roomEnemies[i].primarySkill.goodValueMultiplier,
                room.roomEnemies[i].primarySkill.greatValueMultiplier,
                room.roomEnemies[i].primarySkill.perfectValueMultiplier,
                room.roomEnemies[i].primarySkill.missProcMultiplier,
                room.roomEnemies[i].primarySkill.goodProcMultiplier,
                room.roomEnemies[i].primarySkill.greatProcMultiplier,
                room.roomEnemies[i].primarySkill.perfectProcMultiplier);

            activeEnemy.primarySkill.InitializeSkill(
                room.roomEnemies[i].primarySkill.skillIconColour,
                room.roomEnemies[i].primarySkill.skillBorderColour,
                room.roomEnemies[i].primarySkill.skillSelectionColour,
                room.roomEnemies[i].primarySkill.sprite,
                room.roomEnemies[i].primarySkill.skillType,
                room.roomEnemies[i].primarySkill.skillMode,
                room.roomEnemies[i].primarySkill.targetType,
                room.roomEnemies[i].primarySkill.targetsAllowed,
                room.roomEnemies[i].primarySkill.hitsRequired,
                room.roomEnemies[i].primarySkill.manaRequired,
                room.roomEnemies[i].primarySkill.timeBetweenHitUI,
                room.roomEnemies[i].primarySkill.timeTillEffectInflict,
                room.roomEnemies[i].primarySkill.timeForNextHitMarker,
                room.roomEnemies[i].primarySkill.effect,
                room.roomEnemies[i].primarySkill.effectTarget,
                room.roomEnemies[i].primarySkill.effectPower,
                room.roomEnemies[i].primarySkill.effectDuration,
                room.roomEnemies[i].primarySkill.effectHitEffect,
                room.roomEnemies[i].primarySkill.effectDurationDecrease,
                room.roomEnemies[i].primarySkill.counterSkill,
                room.roomEnemies[i].primarySkill.stackValue,
                room.roomEnemies[i].primarySkill.targetAmountPowerInc,
                room.roomEnemies[i].primarySkill.isTargetCountValAmp,
                room.roomEnemies[i].primarySkill.maxTargetCount,
                room.roomEnemies[i].primarySkill.activatable,
                room.roomEnemies[i].primarySkill.name,
                room.roomEnemies[i].primarySkill.description,
                room.roomEnemies[i].primarySkill.turnCooldown,
                room.roomEnemies[i].primarySkill.missValueMultiplier,
                room.roomEnemies[i].primarySkill.goodValueMultiplier,
                room.roomEnemies[i].primarySkill.greatValueMultiplier,
                room.roomEnemies[i].primarySkill.perfectValueMultiplier,
                room.roomEnemies[i].primarySkill.missProcMultiplier,
                room.roomEnemies[i].primarySkill.goodProcMultiplier,
                room.roomEnemies[i].primarySkill.greatProcMultiplier,
                room.roomEnemies[i].primarySkill.perfectProcMultiplier);

            activeEnemy.alternateSkill.InitializeSkill(
                room.roomEnemies[i].secondarySkill.skillIconColour,
                room.roomEnemies[i].secondarySkill.skillBorderColour,
                room.roomEnemies[i].secondarySkill.skillSelectionColour,
                room.roomEnemies[i].secondarySkill.sprite,
                room.roomEnemies[i].secondarySkill.skillType,
                room.roomEnemies[i].secondarySkill.skillMode,
                room.roomEnemies[i].secondarySkill.targetType,
                room.roomEnemies[i].secondarySkill.targetsAllowed,
                room.roomEnemies[i].secondarySkill.hitsRequired,
                room.roomEnemies[i].secondarySkill.manaRequired,
                room.roomEnemies[i].secondarySkill.timeBetweenHitUI,
                room.roomEnemies[i].secondarySkill.timeTillEffectInflict,
                room.roomEnemies[i].secondarySkill.timeForNextHitMarker,
                room.roomEnemies[i].secondarySkill.effect,
                room.roomEnemies[i].secondarySkill.effectTarget,
                room.roomEnemies[i].secondarySkill.effectPower,
                room.roomEnemies[i].secondarySkill.effectDuration,
                room.roomEnemies[i].secondarySkill.effectHitEffect,
                room.roomEnemies[i].secondarySkill.effectDurationDecrease,
                room.roomEnemies[i].secondarySkill.counterSkill,
                room.roomEnemies[i].secondarySkill.stackValue,
                room.roomEnemies[i].secondarySkill.targetAmountPowerInc,
                room.roomEnemies[i].secondarySkill.isTargetCountValAmp,
                room.roomEnemies[i].secondarySkill.maxTargetCount,
                room.roomEnemies[i].secondarySkill.activatable,
                room.roomEnemies[i].secondarySkill.name,
                room.roomEnemies[i].secondarySkill.description,
                room.roomEnemies[i].secondarySkill.turnCooldown,
                room.roomEnemies[i].secondarySkill.missValueMultiplier,
                room.roomEnemies[i].secondarySkill.goodValueMultiplier,
                room.roomEnemies[i].secondarySkill.greatValueMultiplier,
                room.roomEnemies[i].secondarySkill.perfectValueMultiplier,
                room.roomEnemies[i].secondarySkill.missProcMultiplier,
                room.roomEnemies[i].secondarySkill.goodProcMultiplier,
                room.roomEnemies[i].secondarySkill.greatProcMultiplier,
                room.roomEnemies[i].secondarySkill.perfectProcMultiplier);

            activeEnemy.alternateSkill.InitializeSkill(
                room.roomEnemies[i].alternateSkill.skillIconColour,
                room.roomEnemies[i].alternateSkill.skillBorderColour,
                room.roomEnemies[i].alternateSkill.skillSelectionColour,
                room.roomEnemies[i].alternateSkill.sprite,
                room.roomEnemies[i].alternateSkill.skillType,
                room.roomEnemies[i].alternateSkill.skillMode,
                room.roomEnemies[i].alternateSkill.targetType,
                room.roomEnemies[i].alternateSkill.targetsAllowed,
                room.roomEnemies[i].alternateSkill.hitsRequired,
                room.roomEnemies[i].alternateSkill.manaRequired,
                room.roomEnemies[i].alternateSkill.timeBetweenHitUI,
                room.roomEnemies[i].alternateSkill.timeTillEffectInflict,
                room.roomEnemies[i].alternateSkill.timeForNextHitMarker,
                room.roomEnemies[i].alternateSkill.effect,
                room.roomEnemies[i].alternateSkill.effectTarget,
                room.roomEnemies[i].alternateSkill.effectPower,
                room.roomEnemies[i].alternateSkill.effectDuration,
                room.roomEnemies[i].alternateSkill.effectHitEffect,
                room.roomEnemies[i].alternateSkill.effectDurationDecrease,
                room.roomEnemies[i].alternateSkill.counterSkill,
                room.roomEnemies[i].alternateSkill.stackValue,
                room.roomEnemies[i].alternateSkill.targetAmountPowerInc,
                room.roomEnemies[i].alternateSkill.isTargetCountValAmp,
                room.roomEnemies[i].alternateSkill.maxTargetCount,
                room.roomEnemies[i].alternateSkill.activatable,
                room.roomEnemies[i].alternateSkill.name,
                room.roomEnemies[i].alternateSkill.description,
                room.roomEnemies[i].alternateSkill.turnCooldown,
                room.roomEnemies[i].alternateSkill.missValueMultiplier,
                room.roomEnemies[i].alternateSkill.goodValueMultiplier,
                room.roomEnemies[i].alternateSkill.greatValueMultiplier,
                room.roomEnemies[i].alternateSkill.perfectValueMultiplier,
                room.roomEnemies[i].alternateSkill.missProcMultiplier,
                room.roomEnemies[i].alternateSkill.goodProcMultiplier,
                room.roomEnemies[i].alternateSkill.greatProcMultiplier,
                room.roomEnemies[i].alternateSkill.perfectProcMultiplier);

            #endregion

            #endregion

            _enemies.Add(activeEnemy);      // Add enemy to enemies
            _enemiesPosition.Add(activeEnemy);  // Add enemy position to enemy positions
            turnOrder.Add(activeEnemy);     // Add unit to turn order 
            //activeEnemy.selector.enemyIndex = _enemiesPosition.IndexOf(activeEnemy);
            activeEnemy.targets.Add(activeRelic);
            //activeEnemy.CalculateEnergy();
        }
    }

    /// <summary>
    /// Spawns a relic
    /// </summary>
    void SpawnRelic(Relic relic)
    {
        if (relicInitialized)
            return;

        #region Relic Initialize

        GameObject go = Instantiate(relicGO);
        go.name = relic.name;
        go.transform.SetParent(_relicSpawnPoint.transform);
        go.transform.position = _relicSpawnPoint.transform.position;

        //Image image = go.AddComponent<Image>();
        //image.color = relic.color;

        UpdateActiveRelic(go.GetComponent<Unit>());

        GameObject skillUIPar = Instantiate(skillUIValuesPar, activeRelic.transform.position, Quaternion.identity);
        activeRelic.skillUIValueParent = skillUIPar.transform;

        skillUIPar.transform.SetParent(go.transform);

        activeRelic.skillUIValueParent = go.transform.Find("Skill UI Values");

        activeRelic.SetPortraitImage(relic.portraitSprite);

        activeRelic.UpdateUnitType(Unit.UnitType.ALLY);
        activeRelic.AssignUI(); // Initialize Unit UI
        activeRelic.UpdateName(relic.name);
        activeRelic.UpdateColour(relic.color);
        activeRelic.UpdateMaxHealth(relic.maxHealth);
        activeRelic.UpdateCurHealth(false, relic.maxHealth);
        activeRelic.UpdateMaxMana(relic.maxMana);
        StartCoroutine(activeRelic.UpdateCurMana(relic.maxMana));
        activeRelic.UpdateEnergyTurnGrowth(relic.manaGainTurn);
        activeRelic.UpdatePower(relic.power);
        activeRelic.UpdateEnergy(relic.energy);
        SetActiveAttackBar(FindObjectOfType<AttackBar>());     // Set active weapon

        activeRelic.target = go.GetComponentInChildren<Target>();

        InitializeUnitSkills(activeRelic);

        #region Initialize Relic Skills
        activeRelic.basicSkill.InitializeSkill(
            relic.basicSkill.skillIconColour,
            relic.basicSkill.skillBorderColour,
            relic.basicSkill.skillSelectionColour,
            relic.basicSkill.sprite,
            relic.basicSkill.skillType,
            relic.basicSkill.skillMode,
            relic.basicSkill.targetType,
            relic.basicSkill.targetsAllowed,
            relic.basicSkill.hitsRequired,
            relic.basicSkill.manaRequired,
            relic.basicSkill.timeBetweenHitUI,
            relic.basicSkill.timeTillEffectInflict,
            relic.basicSkill.timeForNextHitMarker,
            relic.basicSkill.effect,
            relic.basicSkill.effectTarget,
            relic.basicSkill.effectPower,
            relic.basicSkill.effectDuration,
            relic.basicSkill.effectHitEffect,
            relic.basicSkill.effectDurationDecrease,
            relic.basicSkill.counterSkill,
            relic.basicSkill.stackValue,
            relic.basicSkill.targetAmountPowerInc,
            relic.basicSkill.isTargetCountValAmp,
            relic.basicSkill.maxTargetCount,
            relic.basicSkill.activatable,
            relic.basicSkill.name,
            relic.basicSkill.description,
            relic.basicSkill.turnCooldown,
            relic.basicSkill.missValueMultiplier,
            relic.basicSkill.goodValueMultiplier,
            relic.basicSkill.greatValueMultiplier,
            relic.basicSkill.perfectValueMultiplier,
            relic.basicSkill.missProcMultiplier,
            relic.basicSkill.goodProcMultiplier,
            relic.basicSkill.greatProcMultiplier,
            relic.basicSkill.perfectProcMultiplier,
            relic.basicSkill.maxSkillCount);

        activeRelic.primarySkill.InitializeSkill(
            relic.primarySkill.skillIconColour,
            relic.primarySkill.skillBorderColour,
            relic.primarySkill.skillSelectionColour,
            relic.primarySkill.sprite,
            relic.primarySkill.skillType,
            relic.primarySkill.skillMode,
            relic.primarySkill.targetType,
            relic.primarySkill.targetsAllowed,
            relic.primarySkill.hitsRequired,
            relic.primarySkill.manaRequired,
            relic.primarySkill.timeBetweenHitUI,
            relic.primarySkill.timeTillEffectInflict,
            relic.primarySkill.timeForNextHitMarker,
            relic.primarySkill.effect,
            relic.primarySkill.effectTarget,
            relic.primarySkill.effectPower,
            relic.primarySkill.effectDuration,
            relic.primarySkill.effectHitEffect,
            relic.primarySkill.effectDurationDecrease,
            relic.primarySkill.counterSkill,
            relic.primarySkill.stackValue,
            relic.primarySkill.targetAmountPowerInc,
            relic.primarySkill.isTargetCountValAmp,
            relic.primarySkill.maxTargetCount,
            relic.primarySkill.activatable,
            relic.primarySkill.name,
            relic.primarySkill.description,
            relic.primarySkill.turnCooldown,
            relic.primarySkill.missValueMultiplier,
            relic.primarySkill.goodValueMultiplier,
            relic.primarySkill.greatValueMultiplier,
            relic.primarySkill.perfectValueMultiplier,
            relic.primarySkill.missProcMultiplier,
            relic.primarySkill.goodProcMultiplier,
            relic.primarySkill.greatProcMultiplier,
            relic.primarySkill.perfectProcMultiplier,
            relic.primarySkill.maxSkillCount);

        activeRelic.secondarySkill.InitializeSkill(
            relic.secondarySkill.skillIconColour,
            relic.secondarySkill.skillBorderColour,
            relic.secondarySkill.skillSelectionColour,
            relic.secondarySkill.sprite,
            relic.secondarySkill.skillType,
            relic.secondarySkill.skillMode,
            relic.secondarySkill.targetType,
            relic.secondarySkill.targetsAllowed,
            relic.secondarySkill.hitsRequired,
            relic.secondarySkill.manaRequired,
            relic.secondarySkill.timeBetweenHitUI,
            relic.secondarySkill.timeTillEffectInflict,
            relic.secondarySkill.timeForNextHitMarker,
            relic.secondarySkill.effect,
            relic.secondarySkill.effectTarget,
            relic.secondarySkill.effectPower,
            relic.secondarySkill.effectDuration,
            relic.secondarySkill.effectHitEffect,
            relic.secondarySkill.effectDurationDecrease,
            relic.secondarySkill.counterSkill,
            relic.secondarySkill.stackValue,
            relic.secondarySkill.targetAmountPowerInc,
            relic.secondarySkill.isTargetCountValAmp,
            relic.secondarySkill.maxTargetCount,
            relic.secondarySkill.activatable,
            relic.secondarySkill.name,
            relic.secondarySkill.description,
            relic.secondarySkill.turnCooldown,
            relic.secondarySkill.missValueMultiplier,
            relic.secondarySkill.goodValueMultiplier,
            relic.secondarySkill.greatValueMultiplier,
            relic.secondarySkill.perfectValueMultiplier,
            relic.secondarySkill.missProcMultiplier,
            relic.secondarySkill.goodProcMultiplier,
            relic.secondarySkill.greatProcMultiplier,
            relic.secondarySkill.perfectProcMultiplier,
            relic.secondarySkill.maxSkillCount);

        activeRelic.alternateSkill.InitializeSkill(
            relic.alternateSkill.skillIconColour,
            relic.alternateSkill.skillBorderColour,
            relic.alternateSkill.skillSelectionColour,
            relic.alternateSkill.sprite,
            relic.alternateSkill.skillType,            
            relic.alternateSkill.skillMode,
            relic.alternateSkill.targetType,
            relic.alternateSkill.targetsAllowed,
            relic.alternateSkill.hitsRequired,
            relic.alternateSkill.manaRequired,
            relic.alternateSkill.timeBetweenHitUI,
            relic.alternateSkill.timeTillEffectInflict,
            relic.alternateSkill.timeForNextHitMarker,
            relic.alternateSkill.effect,
            relic.alternateSkill.effectTarget,
            relic.alternateSkill.effectPower,
            relic.alternateSkill.effectDuration,
            relic.alternateSkill.effectHitEffect,
            relic.alternateSkill.effectDurationDecrease,
            relic.alternateSkill.counterSkill,
            relic.alternateSkill.stackValue,
            relic.alternateSkill.targetAmountPowerInc,
            relic.alternateSkill.isTargetCountValAmp,
            relic.alternateSkill.maxTargetCount,
            relic.alternateSkill.activatable,
            relic.alternateSkill.name,
            relic.alternateSkill.description,
            relic.alternateSkill.turnCooldown,
            relic.alternateSkill.missValueMultiplier,
            relic.alternateSkill.goodValueMultiplier,
            relic.alternateSkill.greatValueMultiplier,
            relic.alternateSkill.perfectValueMultiplier,
            relic.alternateSkill.missProcMultiplier,
            relic.alternateSkill.goodProcMultiplier,
            relic.alternateSkill.greatProcMultiplier,
            relic.alternateSkill.perfectProcMultiplier,
            relic.alternateSkill.maxSkillCount);

        activeRelic.alternateSkill.InitializeSkill(
            relic.alternateSkill.skillIconColour,
            relic.alternateSkill.skillBorderColour,
            relic.alternateSkill.skillSelectionColour,
            relic.alternateSkill.sprite,
            relic.alternateSkill.skillType,
            relic.alternateSkill.skillMode,
            relic.alternateSkill.targetType,
            relic.alternateSkill.targetsAllowed,
            relic.alternateSkill.hitsRequired,
            relic.alternateSkill.manaRequired,
            relic.alternateSkill.timeBetweenHitUI,
            relic.alternateSkill.timeTillEffectInflict,
            relic.alternateSkill.timeForNextHitMarker,
            relic.alternateSkill.effect,
            relic.alternateSkill.effectTarget,
            relic.alternateSkill.effectPower,
            relic.alternateSkill.effectDuration,
            relic.alternateSkill.effectHitEffect,
            relic.alternateSkill.effectDurationDecrease,
            relic.alternateSkill.counterSkill,
            relic.alternateSkill.stackValue,
            relic.alternateSkill.targetAmountPowerInc,
            relic.alternateSkill.isTargetCountValAmp,
            relic.alternateSkill.maxTargetCount,
            relic.alternateSkill.activatable,
            relic.alternateSkill.name,
            relic.alternateSkill.description,
            relic.alternateSkill.turnCooldown,
            relic.alternateSkill.missValueMultiplier,
            relic.alternateSkill.goodValueMultiplier,
            relic.alternateSkill.greatValueMultiplier,
            relic.alternateSkill.perfectValueMultiplier,
            relic.alternateSkill.missProcMultiplier,
            relic.alternateSkill.goodProcMultiplier,
            relic.alternateSkill.greatProcMultiplier,
            relic.alternateSkill.perfectProcMultiplier,
            relic.alternateSkill.maxSkillCount);
        #endregion

        #endregion

        skillUIManager.InitializeSkills(activeRelic); // Sets relics skills
                                           // Add relic and enemies to stored list
        turnOrder.Add(activeRelic);
    }
}

