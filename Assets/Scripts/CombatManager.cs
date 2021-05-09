using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{


    [Header("General")]
    public Relic startingRelic;
    [SerializeField] private Transform _enemySpawnPoint;
    [SerializeField] private Transform _relicSpawnPoint;
    [SerializeField] private Room _activeRoom;
    public List<Unit> _enemies = new List<Unit>();
    [Tooltip("Original position of enemies in active room")]
    public List<Unit> _enemiesPosition = new List<Unit>();
    public List<Unit> _allies = new List<Unit>();
    [SerializeField] private AttackBar _attackBar;
    [SerializeField] private Button _fightButton;
    
    [Space(1)]
    [Header("Skill Keywords")]
    public string[] skillType;
    public string[] skillModes;
    public string[] targetType;
    public string[] attackSequence;
    public string[] targetsAllowed;
    public string[] inflictType;  

    [Header("Combat Settings")]
    [SerializeField] private float postEnemyAttackWait;
    [SerializeField] private float postAllyAttackWait;

    [Header("Relic Settings")]
    [Tooltip("The time that must elapse before another thing happens")]
    public float breatheTime = .25f;
    [Tooltip("The amount of time in seconds that must elapse after the enemy spawns before the next stage can begin")]
    public float enemyTimeWaitSpawn;

    [Header("Skill Value UI")]
    public GameObject skillUIValuesPar;

    [Header("Relic UI")]
    [SerializeField] private CanvasGroup _relicUIHider;
    [SerializeField] private float _relicUIHiderOffVal;
    [SerializeField] private float _relicUIHiderSelectVal;
    [SerializeField] private float _relicUIHiderOnVal;

    [Header("Unit UI")]
    public GameObject unitUI;

    [Header("Other Settings")]
    public int maxEffectsActive;
    public SkillData relicActiveSkill;
    public SkillData enemyActiveSkill;
    public List<Selector> targetSelections = new List<Selector>();
    public List<Selector> skillSelections = new List<Selector>();
    [HideInInspector]
    public Unit activeRelic, activeEnemy;
    private bool relicInitialized;
    private bool relicTurn;
    //[HideInInspector]
    public int maxTargetSelections;
    public int curTargetSelections;
    //[HideInInspector]
    public int maxSkillSelections;
    public int curSkillSelections;
    public int activeRoomMaxEnemiesCount;
    public int oldCurTargetSelections;

    private DevManager _devManager;
    public SkillUIManager skillUIManager;

    //[HideInInspector]
    public float relicActiveSkillValueModifier, relicActiveSkillProcModifier;

    private void Awake()
    {
        _devManager = FindObjectOfType<DevManager>();
        skillUIManager = FindObjectOfType<SkillUIManager>();

        InitialLaunch();
    }

    void InitialLaunch()
    {

    }

    public void StartBattle()
    {
        ToggleFightButton(false);
        SpawnRelic(startingRelic);
        SpawnEnemies(_activeRoom);
        DetermineTurnOrder();
    }

    private void ToggleFightButton(bool cond)
    {
        _fightButton.gameObject.SetActive(cond);
    }

    public bool CheckRelicUIHiderStatus()
    {
        // If the relic UI hider is off
        if (_relicUIHider.alpha == _relicUIHiderOffVal)
            return true;

        if (_relicUIHider.alpha == _relicUIHiderOnVal)
            return false;

        if (_relicUIHider.alpha == _relicUIHiderSelectVal)
            return false;

        else
            return false;
    }

    /// <summary>
    /// Spawn enemies in room
    /// </summary>
    /// <param name="room"></param>
    void SpawnEnemies(Room room)
    {
        activeRoomMaxEnemiesCount = room.roomMaxEnemies;

        for (int i = 0; i < room.roomEnemies.Count; i++)
        {
            GameObject go = Instantiate(room.roomEnemyGO[i]);
            go.name = room.roomEnemies[i].name;
            go.transform.SetParent(_enemySpawnPoint.GetChild(i).transform);
            go.transform.position = _enemySpawnPoint.GetChild(i).transform.position;

            #region Initialize Unit
            activeEnemy = go.AddComponent<Unit>();
            activeEnemy.skillUIValueParent = go.transform.Find("Skill UI Values");

            activeEnemy.UpdateUnitType(Unit.UnitType.ENEMY);
            activeEnemy.AssignUI(); // Initialize Unit UI
            activeEnemy.UpdateName(room.roomEnemies[i].name);
            activeEnemy.UpdateLevel(room.roomEnemies[i].level);
            activeEnemy.UpdateColour(room.roomEnemies[i].color);
            activeEnemy.UpdateMaxHealth(room.roomEnemies[i].maxHealth);
            activeEnemy.UpdateCurHealth(room.roomEnemies[i].maxHealth, false);
            activeEnemy.UpdatePower(room.roomEnemies[i].power);
            activeEnemy.UpdateSpeed(room.roomEnemies[i].speed);
            activeEnemy.UpdateAttackChance(room.roomEnemies[i].attackChance, true);

            InitializeUnitSkills(activeEnemy);

            #region Initialize Enemy Skills
            activeEnemy.passiveSkill.InitializeSkill(
                room.roomEnemies[i].passiveSkill.skillIconColour,
                room.roomEnemies[i].passiveSkill.skillBorderColour,
                room.roomEnemies[i].passiveSkill.skillSelectionColour,
                room.roomEnemies[i].passiveSkill.skillType,
                room.roomEnemies[i].passiveSkill.skillMode,
                room.roomEnemies[i].passiveSkill.targetType,
                room.roomEnemies[i].passiveSkill.targetsAllowed,
                room.roomEnemies[i].passiveSkill.effect,
                room.roomEnemies[i].passiveSkill.effectTarget,
                room.roomEnemies[i].passiveSkill.effectPower,
                room.roomEnemies[i].passiveSkill.effectDuration,
                room.roomEnemies[i].passiveSkill.effectHitEffect,
                room.roomEnemies[i].passiveSkill.effectDurationDecrease,
                room.roomEnemies[i].passiveSkill.counterSkill,
                room.roomEnemies[i].passiveSkill.stackValue,
                room.roomEnemies[i].passiveSkill.maxTargetSelections,
                room.roomEnemies[i].passiveSkill.name,
                room.roomEnemies[i].passiveSkill.description,
                room.roomEnemies[i].passiveSkill.turnCooldown,
                room.roomEnemies[i].passiveSkill.missValueMultiplier,
                room.roomEnemies[i].passiveSkill.goodValueMultiplier,
                room.roomEnemies[i].passiveSkill.greatValueMultiplier,
                room.roomEnemies[i].passiveSkill.perfectValueMultiplier,
                room.roomEnemies[i].passiveSkill.missProcMultiplier,
                room.roomEnemies[i].passiveSkill.goodProcMultiplier,
                room.roomEnemies[i].passiveSkill.greatProcMultiplier,
                room.roomEnemies[i].passiveSkill.perfectProcMultiplier);

            activeEnemy.basicSkill.InitializeSkill(
                room.roomEnemies[i].basicSkill.skillIconColour,
                room.roomEnemies[i].basicSkill.skillBorderColour,
                room.roomEnemies[i].basicSkill.skillSelectionColour,
                room.roomEnemies[i].basicSkill.skillType,
                room.roomEnemies[i].basicSkill.skillMode,
                room.roomEnemies[i].basicSkill.targetType,
                room.roomEnemies[i].basicSkill.targetsAllowed,
                room.roomEnemies[i].basicSkill.effect,
                room.roomEnemies[i].basicSkill.effectTarget,
                room.roomEnemies[i].basicSkill.effectPower,
                room.roomEnemies[i].basicSkill.effectDuration,
                room.roomEnemies[i].basicSkill.effectHitEffect,
                room.roomEnemies[i].basicSkill.effectDurationDecrease,
                room.roomEnemies[i].basicSkill.counterSkill,
                room.roomEnemies[i].basicSkill.stackValue,
                room.roomEnemies[i].basicSkill.maxTargetSelections,
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
                room.roomEnemies[i].primarySkill.skillType,
                room.roomEnemies[i].primarySkill.skillMode,
                room.roomEnemies[i].primarySkill.targetType,
                room.roomEnemies[i].primarySkill.targetsAllowed,
                room.roomEnemies[i].primarySkill.effect,
                room.roomEnemies[i].primarySkill.effectTarget,
                room.roomEnemies[i].primarySkill.effectPower,
                room.roomEnemies[i].primarySkill.effectDuration,
                room.roomEnemies[i].primarySkill.effectHitEffect,
                room.roomEnemies[i].primarySkill.effectDurationDecrease,
                room.roomEnemies[i].primarySkill.counterSkill,
                room.roomEnemies[i].primarySkill.stackValue,
                room.roomEnemies[i].primarySkill.maxTargetSelections,
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

            activeEnemy.secondarySkill.InitializeSkill(
                room.roomEnemies[i].secondarySkill.skillIconColour,
                room.roomEnemies[i].secondarySkill.skillBorderColour,
                room.roomEnemies[i].secondarySkill.skillSelectionColour,
                room.roomEnemies[i].secondarySkill.skillType,
                room.roomEnemies[i].secondarySkill.skillMode,
                room.roomEnemies[i].secondarySkill.targetType,
                room.roomEnemies[i].secondarySkill.targetsAllowed,
                room.roomEnemies[i].secondarySkill.effect,
                room.roomEnemies[i].secondarySkill.effectTarget,
                room.roomEnemies[i].secondarySkill.effectPower,
                room.roomEnemies[i].secondarySkill.effectDuration,
                room.roomEnemies[i].secondarySkill.effectHitEffect,
                room.roomEnemies[i].secondarySkill.effectDurationDecrease,
                room.roomEnemies[i].secondarySkill.counterSkill,
                room.roomEnemies[i].secondarySkill.stackValue,
                room.roomEnemies[i].secondarySkill.maxTargetSelections,
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
                room.roomEnemies[i].alternateSkill.skillType,
                room.roomEnemies[i].alternateSkill.skillMode,
                room.roomEnemies[i].alternateSkill.targetType,
                room.roomEnemies[i].alternateSkill.targetsAllowed,
                room.roomEnemies[i].alternateSkill.effect,
                room.roomEnemies[i].alternateSkill.effectTarget,
                room.roomEnemies[i].alternateSkill.effectPower,
                room.roomEnemies[i].alternateSkill.effectDuration,
                room.roomEnemies[i].alternateSkill.effectHitEffect,
                room.roomEnemies[i].alternateSkill.effectDurationDecrease,
                room.roomEnemies[i].alternateSkill.counterSkill,
                room.roomEnemies[i].alternateSkill.stackValue,
                room.roomEnemies[i].alternateSkill.maxTargetSelections,
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

            activeEnemy.ultimateSkill.InitializeSkill(
                room.roomEnemies[i].ultimateSkill.skillIconColour,
                room.roomEnemies[i].ultimateSkill.skillBorderColour,
                room.roomEnemies[i].ultimateSkill.skillSelectionColour,
                room.roomEnemies[i].ultimateSkill.skillType,
                room.roomEnemies[i].ultimateSkill.skillMode,
                room.roomEnemies[i].ultimateSkill.targetType,
                room.roomEnemies[i].ultimateSkill.targetsAllowed,
                room.roomEnemies[i].ultimateSkill.effect,
                room.roomEnemies[i].ultimateSkill.effectTarget,
                room.roomEnemies[i].ultimateSkill.effectPower,
                room.roomEnemies[i].ultimateSkill.effectDuration,
                room.roomEnemies[i].ultimateSkill.effectHitEffect,
                room.roomEnemies[i].ultimateSkill.effectDurationDecrease,
                room.roomEnemies[i].ultimateSkill.counterSkill,
                room.roomEnemies[i].ultimateSkill.stackValue,
                room.roomEnemies[i].ultimateSkill.maxTargetSelections,
                room.roomEnemies[i].ultimateSkill.name,
                room.roomEnemies[i].ultimateSkill.description,
                room.roomEnemies[i].ultimateSkill.turnCooldown,
                room.roomEnemies[i].ultimateSkill.missValueMultiplier,
                room.roomEnemies[i].ultimateSkill.goodValueMultiplier,
                room.roomEnemies[i].ultimateSkill.greatValueMultiplier,
                room.roomEnemies[i].ultimateSkill.perfectValueMultiplier,
                room.roomEnemies[i].ultimateSkill.missProcMultiplier,
                room.roomEnemies[i].ultimateSkill.goodProcMultiplier,
                room.roomEnemies[i].ultimateSkill.greatProcMultiplier,
                room.roomEnemies[i].ultimateSkill.perfectProcMultiplier);

            #endregion

            #endregion

            _enemies.Add(activeEnemy);
            _enemiesPosition.Add(activeEnemy);
            activeEnemy.transform.GetChild(0).GetComponent<Selector>().enemyCount = _enemiesPosition.IndexOf(activeEnemy);
            activeEnemy.targets.Add(activeRelic);
            activeEnemy.CalculateSpeedFinal();
        }
    }

    /// <summary>
    /// Spawns a relic
    /// </summary>
    /// <param name="relic"></param>
    void SpawnRelic(Relic relic)
    {
        if (relicInitialized)
            return;

        #region Relic Initialize

        GameObject go = new GameObject();
        go.name = relic.name;
        go.transform.SetParent(_relicSpawnPoint.transform);
        go.transform.position = _relicSpawnPoint.transform.position;

        Image image = go.AddComponent<Image>();
        image.color = relic.color;

        activeRelic = go.AddComponent<Unit>();
        GameObject skillUIPar = Instantiate(skillUIValuesPar, activeRelic.transform.position, Quaternion.identity);
        activeRelic.skillUIValueParent = skillUIPar.transform;

        skillUIPar.transform.SetParent(go.transform);

        activeRelic.skillUIValueParent = go.transform.Find("Skill UI Values");

        activeRelic.UpdateUnitType(Unit.UnitType.ALLY);
        activeRelic.AssignUI(); // Initialize Unit UI
        activeRelic.UpdateName(relic.name);
        activeRelic.UpdateColour(relic.color);
        activeRelic.UpdateMaxHealth(relic.maxHealth);
        activeRelic.UpdateCurHealth(relic.maxHealth, false);
        activeRelic.UpdatePower(relic.power);
        activeRelic.UpdateSpeed(relic.speed);

        InitializeUnitSkills(activeRelic);

        #region Initialize Relic Skills
        activeRelic.passiveSkill.InitializeSkill(
            relic.passiveSkill.skillIconColour,
            relic.passiveSkill.skillBorderColour,
            relic.passiveSkill.skillSelectionColour,
            relic.passiveSkill.skillType,
            relic.passiveSkill.skillMode,
            relic.passiveSkill.targetType,
            relic.passiveSkill.targetsAllowed,
            relic.passiveSkill.effect,
            relic.passiveSkill.effectTarget,
            relic.passiveSkill.effectPower,
            relic.passiveSkill.effectDuration,
            relic.passiveSkill.effectHitEffect,
            relic.passiveSkill.effectDurationDecrease,
            relic.passiveSkill.counterSkill,
            relic.passiveSkill.stackValue,
            relic.passiveSkill.maxTargetSelections,
            relic.passiveSkill.name,
            relic.passiveSkill.description,
            relic.passiveSkill.turnCooldown,
            relic.passiveSkill.missValueMultiplier,
            relic.passiveSkill.goodValueMultiplier,
            relic.passiveSkill.greatValueMultiplier,
            relic.passiveSkill.perfectValueMultiplier,
            relic.passiveSkill.missProcMultiplier,
            relic.passiveSkill.goodProcMultiplier,
            relic.passiveSkill.greatProcMultiplier,
            relic.passiveSkill.perfectProcMultiplier,
            relic.passiveSkill.maxSkillCount);

            skillUIManager.relicPassiveSelect.skillIconColour = relic.passiveSkill.skillIconColour;
            skillUIManager.relicPassiveSelect.skillBorderColour = relic.passiveSkill.skillBorderColour;
            skillUIManager.relicPassiveSelect.skillSelectionColour = relic.passiveSkill.skillSelectionColour;

        activeRelic.basicSkill.InitializeSkill(
            relic.basicSkill.skillIconColour,
            relic.basicSkill.skillBorderColour,
            relic.basicSkill.skillSelectionColour,
            relic.basicSkill.skillType,
            relic.basicSkill.skillMode,
            relic.basicSkill.targetType,
            relic.basicSkill.targetsAllowed,
            relic.basicSkill.effect,
            relic.basicSkill.effectTarget,
            relic.basicSkill.effectPower,
            relic.basicSkill.effectDuration,
            relic.basicSkill.effectHitEffect,
            relic.basicSkill.effectDurationDecrease,
            relic.basicSkill.counterSkill,
            relic.basicSkill.stackValue,
            relic.basicSkill.maxTargetSelections,
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

            skillUIManager.relicBasicSelect.skillIconColour = relic.basicSkill.skillIconColour;
            skillUIManager.relicBasicSelect.skillBorderColour = relic.basicSkill.skillBorderColour;
            skillUIManager.relicBasicSelect.skillSelectionColour = relic.basicSkill.skillSelectionColour;

        activeRelic.primarySkill.InitializeSkill(
            relic.primarySkill.skillIconColour,
            relic.primarySkill.skillBorderColour,
            relic.primarySkill.skillSelectionColour,
            relic.primarySkill.skillType,
            relic.primarySkill.skillMode,
            relic.primarySkill.targetType,
            relic.primarySkill.targetsAllowed,
            relic.primarySkill.effect,
            relic.primarySkill.effectTarget,
            relic.primarySkill.effectPower,
            relic.primarySkill.effectDuration,
            relic.primarySkill.effectHitEffect,
            relic.primarySkill.effectDurationDecrease,
            relic.primarySkill.counterSkill,
            relic.primarySkill.stackValue,
            relic.primarySkill.maxTargetSelections,
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

            skillUIManager.relicPrimarySelect.skillIconColour = relic.primarySkill.skillIconColour;
            skillUIManager.relicPrimarySelect.skillBorderColour = relic.primarySkill.skillBorderColour;
            skillUIManager.relicPrimarySelect.skillSelectionColour = relic.primarySkill.skillSelectionColour;

        activeRelic.secondarySkill.InitializeSkill(
            relic.secondarySkill.skillIconColour,
            relic.secondarySkill.skillBorderColour,
            relic.secondarySkill.skillSelectionColour,
            relic.secondarySkill.skillType,
            relic.secondarySkill.skillMode,
            relic.secondarySkill.targetType,
            relic.secondarySkill.targetsAllowed,
            relic.secondarySkill.effect,
            relic.secondarySkill.effectTarget,
            relic.secondarySkill.effectPower,
            relic.secondarySkill.effectDuration,
            relic.secondarySkill.effectHitEffect,
            relic.secondarySkill.effectDurationDecrease,
            relic.secondarySkill.counterSkill,
            relic.secondarySkill.stackValue,
            relic.secondarySkill.maxTargetSelections,
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

            skillUIManager.relicSecondarySelect.skillIconColour = relic.secondarySkill.skillIconColour;
            skillUIManager.relicSecondarySelect.skillBorderColour = relic.secondarySkill.skillBorderColour;
            skillUIManager.relicSecondarySelect.skillSelectionColour = relic.secondarySkill.skillSelectionColour;

        activeRelic.alternateSkill.InitializeSkill(
            relic.alternateSkill.skillIconColour,
            relic.alternateSkill.skillBorderColour,
            relic.alternateSkill.skillSelectionColour,
            relic.alternateSkill.skillType,
            relic.alternateSkill.skillMode,
            relic.alternateSkill.targetType,
            relic.alternateSkill.targetsAllowed,
            relic.alternateSkill.effect,
            relic.alternateSkill.effectTarget,
            relic.alternateSkill.effectPower,
            relic.alternateSkill.effectDuration,
            relic.alternateSkill.effectHitEffect,
            relic.alternateSkill.effectDurationDecrease,
            relic.alternateSkill.counterSkill,
            relic.alternateSkill.stackValue,
            relic.alternateSkill.maxTargetSelections,
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

            skillUIManager.relicAlternateSelect.skillIconColour = relic.alternateSkill.skillIconColour;
            skillUIManager.relicAlternateSelect.skillBorderColour = relic.alternateSkill.skillBorderColour;
            skillUIManager.relicAlternateSelect.skillSelectionColour = relic.alternateSkill.skillSelectionColour;

        activeRelic.ultimateSkill.InitializeSkill(
            relic.ultimateSkill.skillIconColour,
            relic.ultimateSkill.skillBorderColour,
            relic.ultimateSkill.skillSelectionColour,
            relic.ultimateSkill.skillType,
            relic.ultimateSkill.skillMode,
            relic.ultimateSkill.targetType,
            relic.ultimateSkill.targetsAllowed,
            relic.ultimateSkill.effect,
            relic.ultimateSkill.effectTarget,
            relic.ultimateSkill.effectPower,
            relic.ultimateSkill.effectDuration,
            relic.ultimateSkill.effectHitEffect,
            relic.ultimateSkill.effectDurationDecrease,
            relic.ultimateSkill.counterSkill,
            relic.ultimateSkill.stackValue,
            relic.ultimateSkill.maxTargetSelections,
            relic.ultimateSkill.name,
            relic.ultimateSkill.description,
            relic.ultimateSkill.turnCooldown,
            relic.ultimateSkill.missValueMultiplier,
            relic.ultimateSkill.goodValueMultiplier,
            relic.ultimateSkill.greatValueMultiplier,
            relic.ultimateSkill.perfectValueMultiplier,
            relic.ultimateSkill.missProcMultiplier,
            relic.ultimateSkill.goodProcMultiplier,
            relic.ultimateSkill.greatProcMultiplier,
            relic.ultimateSkill.perfectProcMultiplier,
            relic.ultimateSkill.maxSkillCount);

            skillUIManager.relicUltimateSelect.skillIconColour = relic.ultimateSkill.skillIconColour;
            skillUIManager.relicUltimateSelect.skillBorderColour = relic.ultimateSkill.skillBorderColour;
            skillUIManager.relicUltimateSelect.skillSelectionColour = relic.ultimateSkill.skillSelectionColour;
        #endregion

        #endregion

        skillUIManager.InitializeSkills(); // Sets relics skills

        _allies.Add(activeRelic);
        activeRelic.CalculateSpeedFinal();
    }

    private void InitializeUnitSkills(Unit unit)
    {
        unit.passiveSkill = unit.gameObject.AddComponent<SkillData>();
        unit.basicSkill = unit.gameObject.AddComponent<SkillData>();
        unit.primarySkill = unit.gameObject.AddComponent<SkillData>();
        unit.secondarySkill = unit.gameObject.AddComponent<SkillData>();
        unit.alternateSkill = unit.gameObject.AddComponent<SkillData>();
        unit.ultimateSkill = unit.gameObject.AddComponent<SkillData>();

    }
    private int ApplyEnemyTurnOrder(Unit a, Unit b)
    {
        if (a.turnSpeed < b.turnSpeed)
        {
            return -1;
        }
        else if (a.turnSpeed > b.turnSpeed)
        {
            return 1;
        }
        return 0;
    }

    /// <summary>
    /// Determine turn order in room
    /// </summary>
    void DetermineTurnOrder()
    {
        // Determine enemy turn order
        _enemies.Sort(ApplyEnemyTurnOrder);
        _enemies.Reverse();

        // Determine relic turn order
        for (int i = 0; i < _enemies.Count; i++)
        {
            if (_enemies[i].turnSpeed > activeRelic.turnSpeed)
            {
                relicTurn = false;
                break;
            }
            else
            {
                relicTurn = true;
            }
        }

        // If it's the enemy's starting turn
        if (!relicTurn)
            StartCoroutine(StartEnemysTurn());   // Start enemy turn 
        else
            StartCoroutine(StartRelicTurn());   // Start relic turn
    }

    public IEnumerator StartRelicTurn()
    {
        relicTurn = true;

        _attackBar.BeginHitMarkerStartingSequence();     // Start attack bar hit marker

        // Loop through room's allies for their turns
        for (int i = 0; i < _allies.Count; i++)
        {
            UpdateActiveRelic(_allies[i]);   // Update active enemy to i enemy

            skillUIManager.AssignFirstSkill(_allies[i].basicSkill);    // Assign relic's active skill  on the start of their turn

            //AddSkillSelectionsManual(_allies[i].basicSkill);
            skillUIManager.relicBasicSelect.ToggleSelectionImage();    // Apply basic skill 

            _attackBar.UpdateIfRelicCanAttack(true);

            yield return new WaitForSeconds(postAllyAttackWait);             // Wait time before continuing for loop
        }
    }

    public IEnumerator StartEnemysTurn()
    {
        // Loop through room's enemies for their turns
        for (int i = 0; i < _enemies.Count; i++)
        {
            UpdateActiveEnemy(_enemies[i]);     // Update active enemy to i enemy
            _enemies[i].DetermineUnitMoveChoice(_enemies[i], _enemies[i].basicSkill);        // Determine enemy move choice
            yield return new WaitForSeconds(postEnemyAttackWait);        // Wait time before continuing for loop
        }

        StartCoroutine(StartRelicTurn());
    }

    #region Selection Manager

    #region Edit Target Selections

    void ClearTargetSelections(ref List<Selector> targetSelections)
    {
        // Clear all current target selections
        for (int i = 0; i < curTargetSelections; i++)
        {
            targetSelections[i].transform.GetComponent<Selector>().selectEnabled = false; // Tell the oldest image script in stored selection list that the image is disabled
            targetSelections[i].transform.GetComponent<Image>().enabled = false; // Disable the oldest image in stored selection list
        }

        curTargetSelections = 0;
        targetSelections.Clear();
        UpdateRelicUIHider();
    }

    void AddTargetSelections(ref int maxTargetSelections, ref List<Unit> _enemiesPosition, ref List<Selector> targetSelections)
    {
        // Add current target selections
        if (maxTargetSelections > 0)
        {
            for (int i = 0; i < maxTargetSelections; i++)
            {
                curTargetSelections++;
                targetSelections.Add(_enemiesPosition[i].transform.GetChild(0).GetComponent<Selector>()); // Add target selection to stored list
                _enemiesPosition[i].transform.GetChild(0).GetComponent<Selector>().selectEnabled = true; // Tell the oldest image script in stored selection list that the image is disabled
                _enemiesPosition[i].transform.GetChild(0).GetComponent<Image>().enabled = true; // Disable the oldest image in stored selection list
            }

            UpdateRelicUIHider();
        }
    }

    public void UpdateTargetSelection(Selector selector, int selectedTargetIndex, bool cond)
    {
        if (activeRoomMaxEnemiesCount == curTargetSelections)
            return;

        if (cond)
        {
            targetSelections.Add(selector);
            curTargetSelections++;

            if (curTargetSelections > maxTargetSelections)
            {
                targetSelections[0].transform.GetComponent<Selector>().selectEnabled = false; // Tell the oldest image script in stored selection list that the image is disabled
                targetSelections[0].transform.GetComponent<Image>().enabled = false; // Disable the oldest image in stored selection list
                targetSelections.RemoveAt(0);
                curTargetSelections--;
            }
        }
        else
        {
            targetSelections.Remove(selector);
            curTargetSelections--;
        }

        _enemiesPosition[selectedTargetIndex].transform.GetChild(0).GetComponent<Selector>().selectEnabled = cond; // Tell the oldest image script in stored selection list that the image is disabled
        _enemiesPosition[selectedTargetIndex].transform.GetChild(0).GetComponent<Image>().enabled = cond; // Disable the oldest image in stored selection list

        UpdateRelicUIHider();
    }

    #endregion

    #region Edit Skill Selections
    void ClearSkillSelections(ref int curSkillSelections, ref List<Selector> skillSelections)
    {
        // Clear all current target selections
        for (int i = 0; i < curSkillSelections; i++)
        {
            curSkillSelections--;
   
            skillSelections[i].transform.GetComponent<Selector>().selectEnabled = false; // Tell the oldest image script in stored selection list that the image is disabled
            skillSelections[i].transform.GetComponent<Image>().enabled = false; // Disable the oldest image in stored selection list
        }

        skillSelections.Clear();
    }

    void AddSkillSelections(ref int maxSkillSelections, ref int curSkillSelections, ref List<Selector> skillSelections, ref List<Selector> targetSelections, 
        Selector selector, SkillData skillData)
    {
        // Add current target selections
        if (maxSkillSelections > 0)
        {
            for (int i = 0; i < maxSkillSelections; i++)
            {
                curSkillSelections++;

                skillSelections.Add(selector);

                skillSelections[i].transform.GetComponent<Selector>().selectEnabled = true; // Tell the oldest image script in stored selection list that the image is disabled
                skillSelections[i].transform.GetComponent<Image>().enabled = true; // Disable the oldest image in stored selection list
            }
        }
    }
    #endregion

    /// <summary>
    /// Updates selection images
    /// </summary>
    public void ManageSelectionCount(bool isSkill, Selector selectionInput, int curSkillSelections, int maxSkillSelections, 
        int curTargetSelections, int maxTargetSelections, SkillData skillData, bool relicBasicSkill)
    {

        this.curSkillSelections = curSkillSelections;
        this.maxSkillSelections = maxSkillSelections;
        this.curTargetSelections = curTargetSelections;
        this.maxTargetSelections = maxTargetSelections;

        if (isSkill)
        {
            if (!relicBasicSkill)
                ClearSkillSelections(ref this.curSkillSelections, ref skillSelections);

            AddSkillSelections(ref this.maxSkillSelections, ref this.curSkillSelections, ref skillSelections, ref targetSelections, selectionInput, skillData);
        }

        if (!relicBasicSkill)
            ClearTargetSelections(ref targetSelections);

        AddTargetSelections(ref maxTargetSelections, ref _enemiesPosition, ref targetSelections);
    }
    #endregion

    /// <summary>
    /// Toggles the attack bar hider's display
    /// </summary>
    /// <param name="cond"></param>
    private void UpdateRelicUIHider()
    {
        if (relicTurn)
        {
            if (curTargetSelections == maxTargetSelections)
                _relicUIHider.alpha = _relicUIHiderOffVal;

            else if (curTargetSelections <= maxTargetSelections)
                _relicUIHider.alpha = _relicUIHiderSelectVal;
        }

        else
            _relicUIHider.alpha = _relicUIHiderOnVal;
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

    #region Utility

    #endregion
}

