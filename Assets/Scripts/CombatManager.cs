﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{


    [Header("General")]
    public Relic startingRelic;
    [SerializeField] private Transform _enemySpawnPoint;
    [SerializeField] private Transform _RelicSpawnPoint;
    [SerializeField] private Room _activeRoom;
    public List<Unit> _enemies = new List<Unit>();
    public List<Unit> _allies = new List<Unit>();
    [SerializeField] private AttackBar _attackBar;
    [SerializeField] private Button _fightButton;

    [Header("Combat Settings")]
    [SerializeField] private float postEnemyAttackWait;
    [SerializeField] private float postAllyAttackWait;

    [Header("Relic Settings")]
    [Tooltip("The time that must elapse before another thing happens")]
    public float breatheTime = .25f;
    [Tooltip("The amount of time in seconds that must elapse after the enemy spawns before the next stage can begin")]
    public float enemyTimeWaitSpawn;

    [Header("Relic UI")]
    [SerializeField] private CanvasGroup _relicUIHider;
    [SerializeField] private float _relicUIHiderOffVal;
    [SerializeField] private float _relicUIHiderSelectVal;
    [SerializeField] private float _relicUIHiderOnVal;

    [Header("Other Settings")]
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
    private SkillUIManager _skillUIManager;


    private void Awake()
    {
        _devManager = FindObjectOfType<DevManager>();
        _skillUIManager = FindObjectOfType<SkillUIManager>();

        InitialLaunch();
    }

    void InitialLaunch()
    {
        UpdateRelicUIHider(_relicUIHiderOnVal);
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

    /// <summary>
    /// Spawn enemies in room
    /// </summary>
    /// <param name="room"></param>
    void SpawnEnemies(Room room)
    {
        StartCoroutine(_devManager.FlashText("Spawning Enemies"));

        activeRoomMaxEnemiesCount = room.roomMaxEnemies;

        for (int i = 0; i < room.roomEnemies.Count; i++)
        {
            GameObject go = Instantiate(room.roomEnemyGO[i]);
            go.name = room.roomEnemies[i].name;
            go.transform.SetParent(_enemySpawnPoint.GetChild(i).transform);
            go.transform.position = _enemySpawnPoint.GetChild(i).transform.position;

            activeEnemy = go.AddComponent<Unit>();
            activeEnemy.unitType = Unit.UnitType.ENEMY;
            activeEnemy.name = room.roomEnemies[i].name;
            activeEnemy.level = room.roomEnemies[i].level;
            activeEnemy.color = room.roomEnemies[i].color;
            activeEnemy.maxHealth = room.roomEnemies[i].maxHealth;
            activeEnemy.curHealth = room.roomEnemies[i].curHealth;
            activeEnemy.damage = room.roomEnemies[i].damage;
            activeEnemy.speed = room.roomEnemies[i].speed;
            activeEnemy.AdjustAttackChance(room.roomEnemies[i].attackChance, true);

            InitializeUnitSkills(activeEnemy);

            #region Initialize Enemy Skills
            activeEnemy.passiveSkill.InitializeSkill(
                room.roomEnemies[i].passiveSkill.skillIconColour,
                room.roomEnemies[i].passiveSkill.skillBorderColour,
                room.roomEnemies[i].passiveSkill.skillSelectionColour,
                room.roomEnemies[i].passiveSkill.skillType,
                room.roomEnemies[i].passiveSkill.skillMode,
                room.roomEnemies[i].passiveSkill.targetType,
                room.roomEnemies[i].passiveSkill.attackSequence,
                room.roomEnemies[i].passiveSkill.targetsAllowed,
                room.roomEnemies[i].passiveSkill.inflictType,
                room.roomEnemies[i].passiveSkill.maxTargetSelections,
                room.roomEnemies[i].passiveSkill.name,
                room.roomEnemies[i].passiveSkill.description,
                room.roomEnemies[i].passiveSkill.turnCooldown,
                room.roomEnemies[i].passiveSkill.damage,
                room.roomEnemies[i].passiveSkill.missDamageMultiplier,
                room.roomEnemies[i].passiveSkill.goodDamageMultiplier,
                room.roomEnemies[i].passiveSkill.greatDamageMultiplier,
                room.roomEnemies[i].passiveSkill.perfectDamageMultiplier,
                room.roomEnemies[i].passiveSkill.procChance,
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
                room.roomEnemies[i].basicSkill.attackSequence,
                room.roomEnemies[i].basicSkill.targetsAllowed,
                room.roomEnemies[i].basicSkill.inflictType,
                room.roomEnemies[i].basicSkill.maxTargetSelections,
                room.roomEnemies[i].basicSkill.name,
                room.roomEnemies[i].basicSkill.description,
                room.roomEnemies[i].basicSkill.turnCooldown,
                room.roomEnemies[i].basicSkill.damage,
                room.roomEnemies[i].basicSkill.missDamageMultiplier,
                room.roomEnemies[i].basicSkill.goodDamageMultiplier,
                room.roomEnemies[i].basicSkill.greatDamageMultiplier,
                room.roomEnemies[i].basicSkill.perfectDamageMultiplier,
                room.roomEnemies[i].basicSkill.procChance,
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
                room.roomEnemies[i].primarySkill.attackSequence,
                room.roomEnemies[i].primarySkill.targetsAllowed,
                room.roomEnemies[i].primarySkill.inflictType,
                room.roomEnemies[i].primarySkill.maxTargetSelections,
                room.roomEnemies[i].primarySkill.name,
                room.roomEnemies[i].primarySkill.description,
                room.roomEnemies[i].primarySkill.turnCooldown,
                room.roomEnemies[i].primarySkill.damage,
                room.roomEnemies[i].primarySkill.missDamageMultiplier,
                room.roomEnemies[i].primarySkill.goodDamageMultiplier,
                room.roomEnemies[i].primarySkill.greatDamageMultiplier,
                room.roomEnemies[i].primarySkill.perfectDamageMultiplier,
                room.roomEnemies[i].primarySkill.procChance,
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
                room.roomEnemies[i].secondarySkill.attackSequence,
                room.roomEnemies[i].secondarySkill.targetsAllowed,
                room.roomEnemies[i].secondarySkill.inflictType,
                room.roomEnemies[i].secondarySkill.maxTargetSelections,
                room.roomEnemies[i].secondarySkill.name,
                room.roomEnemies[i].secondarySkill.description,
                room.roomEnemies[i].secondarySkill.turnCooldown,
                room.roomEnemies[i].secondarySkill.damage,
                room.roomEnemies[i].secondarySkill.missDamageMultiplier,
                room.roomEnemies[i].secondarySkill.goodDamageMultiplier,
                room.roomEnemies[i].secondarySkill.greatDamageMultiplier,
                room.roomEnemies[i].secondarySkill.perfectDamageMultiplier,
                room.roomEnemies[i].secondarySkill.procChance,
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
                room.roomEnemies[i].alternateSkill.attackSequence,
                room.roomEnemies[i].alternateSkill.targetsAllowed,
                room.roomEnemies[i].alternateSkill.inflictType,
                room.roomEnemies[i].alternateSkill.maxTargetSelections,
                room.roomEnemies[i].alternateSkill.name,
                room.roomEnemies[i].alternateSkill.description,
                room.roomEnemies[i].alternateSkill.turnCooldown,
                room.roomEnemies[i].alternateSkill.damage,
                room.roomEnemies[i].alternateSkill.missDamageMultiplier,
                room.roomEnemies[i].alternateSkill.goodDamageMultiplier,
                room.roomEnemies[i].alternateSkill.greatDamageMultiplier,
                room.roomEnemies[i].alternateSkill.perfectDamageMultiplier,
                room.roomEnemies[i].alternateSkill.procChance,
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
                room.roomEnemies[i].ultimateSkill.attackSequence,
                room.roomEnemies[i].ultimateSkill.targetsAllowed,
                room.roomEnemies[i].ultimateSkill.inflictType,
                room.roomEnemies[i].ultimateSkill.maxTargetSelections,
                room.roomEnemies[i].ultimateSkill.name,
                room.roomEnemies[i].ultimateSkill.description,
                room.roomEnemies[i].ultimateSkill.turnCooldown,
                room.roomEnemies[i].ultimateSkill.damage,
                room.roomEnemies[i].ultimateSkill.missDamageMultiplier,
                room.roomEnemies[i].ultimateSkill.goodDamageMultiplier,
                room.roomEnemies[i].ultimateSkill.greatDamageMultiplier,
                room.roomEnemies[i].ultimateSkill.perfectDamageMultiplier,
                room.roomEnemies[i].ultimateSkill.procChance,
                room.roomEnemies[i].ultimateSkill.missProcMultiplier,
                room.roomEnemies[i].ultimateSkill.goodProcMultiplier,
                room.roomEnemies[i].ultimateSkill.greatProcMultiplier,
                room.roomEnemies[i].ultimateSkill.perfectProcMultiplier);

            #endregion

            _enemies.Add(activeEnemy);
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
        StartCoroutine(_devManager.FlashText("Spawning Relic"));

        if (relicInitialized)
            return;

        GameObject go = new GameObject();
        go.name = relic.name;
        go.transform.SetParent(_RelicSpawnPoint.transform);
        go.transform.position = _RelicSpawnPoint.transform.position;

        Image image = go.AddComponent<Image>();
        image.color = relic.color;

        activeRelic = go.AddComponent<Unit>();
        activeRelic.unitType = Unit.UnitType.ALLY;
        activeRelic.name = relic.name;
        activeRelic.level = 1;
        activeRelic.color = relic.color;
        activeRelic.maxHealth = relic.maxHealth;
        activeRelic.curHealth = relic.curHealth;
        activeRelic.damage = relic.damage;
        activeRelic.speed = relic.speed;

        InitializeUnitSkills(activeRelic);

        #region Initialize Relic Skills
        activeRelic.passiveSkill.InitializeSkill(
            relic.passiveSkill.skillIconColour,
            relic.passiveSkill.skillBorderColour,
            relic.passiveSkill.skillSelectionColour,
            relic.passiveSkill.skillType,
            relic.passiveSkill.skillMode,
            relic.passiveSkill.targetType,
            relic.passiveSkill.attackSequence,
            relic.passiveSkill.targetsAllowed,
            relic.passiveSkill.inflictType,
            relic.passiveSkill.maxTargetSelections,
            relic.passiveSkill.name,
            relic.passiveSkill.description,
            relic.passiveSkill.turnCooldown,
            relic.passiveSkill.damage,
            relic.passiveSkill.missDamageMultiplier,
            relic.passiveSkill.goodDamageMultiplier,
            relic.passiveSkill.greatDamageMultiplier,
            relic.passiveSkill.perfectDamageMultiplier,
            relic.passiveSkill.procChance,
            relic.passiveSkill.missProcMultiplier,
            relic.passiveSkill.goodProcMultiplier,
            relic.passiveSkill.greatProcMultiplier,
            relic.passiveSkill.perfectProcMultiplier,
            relic.passiveSkill.maxSkillCount);

            _skillUIManager.relicPassiveSelect.skillIconColour = relic.passiveSkill.skillIconColour;
            _skillUIManager.relicPassiveSelect.skillBorderColour = relic.passiveSkill.skillBorderColour;
            _skillUIManager.relicPassiveSelect.skillSelectionColour = relic.passiveSkill.skillSelectionColour;

        activeRelic.basicSkill.InitializeSkill(
            relic.basicSkill.skillIconColour,
            relic.basicSkill.skillBorderColour,
            relic.basicSkill.skillSelectionColour,
            relic.basicSkill.skillType,
            relic.basicSkill.skillMode,
            relic.basicSkill.targetType,
            relic.basicSkill.attackSequence,
            relic.basicSkill.targetsAllowed,
            relic.basicSkill.inflictType,
            relic.basicSkill.maxTargetSelections,
            relic.basicSkill.name,
            relic.basicSkill.description,
            relic.basicSkill.turnCooldown,
            relic.basicSkill.damage,
            relic.basicSkill.missDamageMultiplier,
            relic.basicSkill.goodDamageMultiplier,
            relic.basicSkill.greatDamageMultiplier,
            relic.basicSkill.perfectDamageMultiplier,
            relic.basicSkill.procChance,
            relic.basicSkill.missProcMultiplier,
            relic.basicSkill.goodProcMultiplier,
            relic.basicSkill.greatProcMultiplier,
            relic.basicSkill.perfectProcMultiplier,
            relic.basicSkill.maxSkillCount);

            _skillUIManager.relicBasicSelect.skillIconColour = relic.basicSkill.skillIconColour;
            _skillUIManager.relicBasicSelect.skillBorderColour = relic.basicSkill.skillBorderColour;
            _skillUIManager.relicBasicSelect.skillSelectionColour = relic.basicSkill.skillSelectionColour;

        activeRelic.primarySkill.InitializeSkill(
            relic.primarySkill.skillIconColour,
            relic.primarySkill.skillBorderColour,
            relic.primarySkill.skillSelectionColour,
            relic.primarySkill.skillType,
            relic.primarySkill.skillMode,
            relic.primarySkill.targetType,
            relic.primarySkill.attackSequence,
            relic.primarySkill.targetsAllowed,
            relic.primarySkill.inflictType,
            relic.primarySkill.maxTargetSelections,
            relic.primarySkill.name,
            relic.primarySkill.description,
            relic.primarySkill.turnCooldown,
            relic.primarySkill.damage,
            relic.primarySkill.missDamageMultiplier,
            relic.primarySkill.goodDamageMultiplier,
            relic.primarySkill.greatDamageMultiplier,
            relic.primarySkill.perfectDamageMultiplier,
            relic.primarySkill.procChance,
            relic.primarySkill.missProcMultiplier,
            relic.primarySkill.goodProcMultiplier,
            relic.primarySkill.greatProcMultiplier,
            relic.primarySkill.perfectProcMultiplier,
            relic.primarySkill.maxSkillCount);

            _skillUIManager.relicPrimarySelect.skillIconColour = relic.primarySkill.skillIconColour;
            _skillUIManager.relicPrimarySelect.skillBorderColour = relic.primarySkill.skillBorderColour;
            _skillUIManager.relicPrimarySelect.skillSelectionColour = relic.primarySkill.skillSelectionColour;

        activeRelic.secondarySkill.InitializeSkill(
            relic.secondarySkill.skillIconColour,
            relic.secondarySkill.skillBorderColour,
            relic.secondarySkill.skillSelectionColour,
            relic.secondarySkill.skillType,
            relic.secondarySkill.skillMode,
            relic.secondarySkill.targetType,
            relic.secondarySkill.attackSequence,
            relic.secondarySkill.targetsAllowed,
            relic.secondarySkill.inflictType,
            relic.secondarySkill.maxTargetSelections,
            relic.secondarySkill.name,
            relic.secondarySkill.description,
            relic.secondarySkill.turnCooldown,
            relic.secondarySkill.damage,
            relic.secondarySkill.missDamageMultiplier,
            relic.secondarySkill.goodDamageMultiplier,
            relic.secondarySkill.greatDamageMultiplier,
            relic.secondarySkill.perfectDamageMultiplier,
            relic.secondarySkill.procChance,
            relic.secondarySkill.missProcMultiplier,
            relic.secondarySkill.goodProcMultiplier,
            relic.secondarySkill.greatProcMultiplier,
            relic.secondarySkill.perfectProcMultiplier,
            relic.secondarySkill.maxSkillCount);

            _skillUIManager.relicSecondarySelect.skillIconColour = relic.secondarySkill.skillIconColour;
            _skillUIManager.relicSecondarySelect.skillBorderColour = relic.secondarySkill.skillBorderColour;
            _skillUIManager.relicSecondarySelect.skillSelectionColour = relic.secondarySkill.skillSelectionColour;

        activeRelic.alternateSkill.InitializeSkill(
            relic.alternateSkill.skillIconColour,
            relic.alternateSkill.skillBorderColour,
            relic.alternateSkill.skillSelectionColour,
            relic.alternateSkill.skillType,
            relic.alternateSkill.skillMode,
            relic.alternateSkill.targetType,
            relic.alternateSkill.attackSequence,
            relic.alternateSkill.targetsAllowed,
            relic.alternateSkill.inflictType,
            relic.alternateSkill.maxTargetSelections,
            relic.alternateSkill.name,
            relic.alternateSkill.description,
            relic.alternateSkill.turnCooldown,
            relic.alternateSkill.damage,
            relic.alternateSkill.missDamageMultiplier,
            relic.alternateSkill.goodDamageMultiplier,
            relic.alternateSkill.greatDamageMultiplier,
            relic.alternateSkill.perfectDamageMultiplier,
            relic.alternateSkill.procChance,
            relic.alternateSkill.missProcMultiplier,
            relic.alternateSkill.goodProcMultiplier,
            relic.alternateSkill.greatProcMultiplier,
            relic.alternateSkill.perfectProcMultiplier,
            relic.alternateSkill.maxSkillCount);

            _skillUIManager.relicAlternateSelect.skillIconColour = relic.alternateSkill.skillIconColour;
            _skillUIManager.relicAlternateSelect.skillBorderColour = relic.alternateSkill.skillBorderColour;
            _skillUIManager.relicAlternateSelect.skillSelectionColour = relic.alternateSkill.skillSelectionColour;

        activeRelic.ultimateSkill.InitializeSkill(
            relic.ultimateSkill.skillIconColour,
            relic.ultimateSkill.skillBorderColour,
            relic.ultimateSkill.skillSelectionColour,
            relic.ultimateSkill.skillType,
            relic.ultimateSkill.skillMode,
            relic.ultimateSkill.targetType,
            relic.ultimateSkill.attackSequence,
            relic.ultimateSkill.targetsAllowed,
            relic.ultimateSkill.inflictType,
            relic.ultimateSkill.maxTargetSelections,
            relic.ultimateSkill.name,
            relic.ultimateSkill.description,
            relic.ultimateSkill.turnCooldown,
            relic.ultimateSkill.damage,
            relic.ultimateSkill.missDamageMultiplier,
            relic.ultimateSkill.goodDamageMultiplier,
            relic.ultimateSkill.greatDamageMultiplier,
            relic.ultimateSkill.perfectDamageMultiplier,
            relic.ultimateSkill.procChance,
            relic.ultimateSkill.missProcMultiplier,
            relic.ultimateSkill.goodProcMultiplier,
            relic.ultimateSkill.greatProcMultiplier,
            relic.ultimateSkill.perfectProcMultiplier,
            relic.ultimateSkill.maxSkillCount);

            _skillUIManager.relicUltimateSelect.skillIconColour = relic.ultimateSkill.skillIconColour;
            _skillUIManager.relicUltimateSelect.skillBorderColour = relic.ultimateSkill.skillBorderColour;
            _skillUIManager.relicUltimateSelect.skillSelectionColour = relic.ultimateSkill.skillSelectionColour;
        #endregion

        _skillUIManager.InitializeSkills();

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
        StartCoroutine(_devManager.FlashText("Determining turn order"));

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

    IEnumerator StartRelicTurn()
    {
        StartCoroutine(_devManager.FlashText("Starting Relic's turn"));
        relicTurn = true;
        UpdateRelicUIHider(_relicUIHiderSelectVal);     // Toggle off attack bar hider 
        _attackBar.BeginHitMarkerStartingSequence();     // Start attack bar hit marker

        // Loop through room's allies for their turns
        for (int i = 0; i < _allies.Count; i++)
        {
            UpdateActiveRelic(_allies[i]);     // Update active enemy to i enemy
            _allies[i].DetermineUnitMoveChoice(_allies[i], _allies[i].basicSkill);      // Determine ally move choice
            yield return new WaitForSeconds(postEnemyAttackWait);             // Wait time before continuing for loop
        }
    }

    IEnumerator StartEnemysTurn()
    {
        StartCoroutine(_devManager.FlashText("Starting enemy(s) turn"));
        // If it's the relic's turn
        if (relicTurn)
            _attackBar.StartCoroutine("BeginHitMarkerStoppingSequence"); // Stop the hit marker
        UpdateRelicUIHider(_relicUIHiderOnVal);        // Toggle on the attack bar hider

        // Loop through room's enemies for their turns
        for (int i = 0; i < _enemies.Count; i++)
        {
            UpdateActiveEnemy(_enemies[i]);     // Update active enemy to i enemy
            _enemies[i].DetermineUnitMoveChoice(_enemies[i], _enemies[i].basicSkill);        // Determine enemy move choice
            yield return new WaitForSeconds(postEnemyAttackWait);        // Wait time before continuing for loop
        }

        StartCoroutine(StartRelicTurn());
    }

    /// <summary>
    /// Updates selection images
    /// </summary>
    /// <param name="enabled"></param>
    /// <param name="selection"></param>
    public void ManageSelectionCount(bool enabled, bool isSkill, Selector selectionInput, Image selectImage, 
        RectTransform iconBorderRT, Vector2 sizeDelta)
    {
        int curSelections;
        int maxSelections;
        List<Selector> targetList = new List<Selector>();

        if (isSkill)
        {
            targetList = targetSelections;

            if (maxTargetSelections > 0)
            {
                for (int i = 0; i < maxTargetSelections; i++)
                {
                    if (curTargetSelections < maxTargetSelections)
                    {
                        curTargetSelections++;
                        targetSelections.Add(_enemies[i].transform.GetChild(0).GetComponent<Selector>());
                        _enemies[i].transform.GetChild(0).GetComponent<Selector>().selectEnabled = true;
                        _enemies[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                    }
                }
            }

            #region Remove over capped selections
            if (oldCurTargetSelections > maxTargetSelections && oldCurTargetSelections != 0)
            {
                int val = (oldCurTargetSelections - maxTargetSelections);

                for (int i = 0; i < val; i++)
                {
                    targetList[0].GetComponent<Image>().enabled = false;
                    targetList[0].selectEnabled = false;
                    targetList.RemoveAt(0);
                }

                curTargetSelections = targetList.Count;
            }
            #endregion

            // If a skill can hit all enemies in the room
            if (maxTargetSelections == activeRoomMaxEnemiesCount)
            {
                curTargetSelections = 0;
                targetSelections.Clear();

                // Enable selections on all room enemies
                for (int i = 0; i < _enemies.Count; i++)
                {
                    curTargetSelections++;
                    targetSelections.Add(_enemies[i].transform.GetChild(0).GetComponent<Selector>());
                    _enemies[i].transform.GetChild(0).GetComponent<Selector>().selectEnabled = true;
                    _enemies[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                }
            }



            curSelections = curSkillSelections;
            maxSelections = maxSkillSelections;
            targetList = skillSelections;
        }
        else
        {
            curSelections = curTargetSelections;
            maxSelections = maxTargetSelections;
            targetList = targetSelections;
        }

        // If the current amount of selections equals the maximum amount of selections for the active unit's current skill
        if (curSelections == maxSelections)
        {
            if (enabled)    // If a selection is being enabled
            {
                selectImage.enabled = true;    // Enabled the image
                selectionInput.selectEnabled = true; // Tell the image script that the image is enabled

                targetList.Add(selectionInput); // Add select to list for storing
                targetList[0].selectionImage.enabled = false;  // Disable the oldest image in stored selection list
                targetList[0].selectEnabled = false;   // Tell the oldest image script in stored selection list that the image is disabled
                targetList.RemoveAt(0);    // Remove the oldest image from the stored active selection list

                UpdateRelicUIHider(_relicUIHiderOffVal);
            }
            else    // If a selection is being disabled
            {
                if (isSkill)
                    curSkillSelections--;
                else
                    curTargetSelections--;    // Remove a count from the current selections

                selectImage.enabled = false;   // Disable the image
                selectionInput.selectEnabled = false;    // Tell the image script that the image is disabled
                targetList.Remove(selectionInput);      // Remove selection from stored active selection list

                UpdateRelicUIHider(_relicUIHiderSelectVal);
                return;
            }
        }

        // If the current amount of selections equals the maximum amount of selections for the active unit's current skill
        else if (curSelections < maxSelections)
        {
            if (enabled)    // If a selection is being enabled
            {
                if (isSkill)
                    curSkillSelections++;
                else
                    curTargetSelections++;    // Remove a count from the current selections

                selectImage.enabled = true;    // Enabled the image
                selectionInput.selectEnabled = true;     // Tell the image script that the image is enabled
                targetList.Add(selectionInput);     // Add selection into stored active selection list

                UpdateRelicUIHider(_relicUIHiderOffVal);
            }
            else    // If a selection is being disabled
            {
                if (isSkill)
                    curSkillSelections--;
                else
                    curTargetSelections--;    // Remove a count from the current selections

                selectImage.enabled = false;   // Disable the image
                selectionInput.selectEnabled = false;
                targetList.Remove(selectionInput);

                UpdateRelicUIHider(_relicUIHiderSelectVal);
            }
        }
    }

    /// <summary>
    /// Toggles the attack bar hider's display
    /// </summary>
    /// <param name="cond"></param>
    private void UpdateRelicUIHider(float alpha)
    {
        // If the relic ui hider is not the called value
        if (_relicUIHider.alpha != alpha)
            _relicUIHider.alpha = alpha; // Update the relic ui hider alpha
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

