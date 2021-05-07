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

    //[HideInInspector]
    public float relicActiveSkillValueModifier, relicActiveSkillProcModifier;

    private void Awake()
    {
        _devManager = FindObjectOfType<DevManager>();
        _skillUIManager = FindObjectOfType<SkillUIManager>();

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

            activeEnemy.UpdateUnitType(Unit.UnitType.ENEMY);
            activeEnemy.UpdateName(room.roomEnemies[i].name);
            activeEnemy.UpdateLevel(room.roomEnemies[i].level);
            activeEnemy.UpdateColour(room.roomEnemies[i].color);
            activeEnemy.UpdateMaxHealth(room.roomEnemies[i].maxHealth);
            activeEnemy.UpdateCurHealth(room.roomEnemies[i].curHealth, false);
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
                room.roomEnemies[i].passiveSkill.attackSequence,
                room.roomEnemies[i].passiveSkill.targetsAllowed,
                room.roomEnemies[i].passiveSkill.inflictType,
                room.roomEnemies[i].passiveSkill.maxTargetSelections,
                room.roomEnemies[i].passiveSkill.name,
                room.roomEnemies[i].passiveSkill.description,
                room.roomEnemies[i].passiveSkill.turnCooldown,
                room.roomEnemies[i].passiveSkill.power,
                room.roomEnemies[i].passiveSkill.missValueMultiplier,
                room.roomEnemies[i].passiveSkill.goodValueMultiplier,
                room.roomEnemies[i].passiveSkill.greatValueMultiplier,
                room.roomEnemies[i].passiveSkill.perfectValueMultiplier,
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
                room.roomEnemies[i].basicSkill.power,
                room.roomEnemies[i].basicSkill.missValueMultiplier,
                room.roomEnemies[i].basicSkill.goodValueMultiplier,
                room.roomEnemies[i].basicSkill.greatValueMultiplier,
                room.roomEnemies[i].basicSkill.perfectValueMultiplier,
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
                room.roomEnemies[i].primarySkill.power,
                room.roomEnemies[i].primarySkill.missValueMultiplier,
                room.roomEnemies[i].primarySkill.goodValueMultiplier,
                room.roomEnemies[i].primarySkill.greatValueMultiplier,
                room.roomEnemies[i].primarySkill.perfectValueMultiplier,
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
                room.roomEnemies[i].secondarySkill.power,
                room.roomEnemies[i].secondarySkill.missValueMultiplier,
                room.roomEnemies[i].secondarySkill.goodValueMultiplier,
                room.roomEnemies[i].secondarySkill.greatValueMultiplier,
                room.roomEnemies[i].secondarySkill.perfectValueMultiplier,
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
                room.roomEnemies[i].alternateSkill.power,
                room.roomEnemies[i].alternateSkill.missValueMultiplier,
                room.roomEnemies[i].alternateSkill.goodValueMultiplier,
                room.roomEnemies[i].alternateSkill.greatValueMultiplier,
                room.roomEnemies[i].alternateSkill.perfectValueMultiplier,
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
                room.roomEnemies[i].ultimateSkill.power,
                room.roomEnemies[i].ultimateSkill.missValueMultiplier,
                room.roomEnemies[i].ultimateSkill.goodValueMultiplier,
                room.roomEnemies[i].ultimateSkill.greatValueMultiplier,
                room.roomEnemies[i].ultimateSkill.perfectValueMultiplier,
                room.roomEnemies[i].ultimateSkill.procChance,
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
        activeRelic.UpdateUnitType(Unit.UnitType.ALLY);
        activeRelic.UpdateName(relic.name);
        activeRelic.UpdateColour(relic.color);
        activeRelic.UpdateMaxHealth(relic.maxHealth);
        activeRelic.UpdateCurHealth(relic.curHealth, false);
        activeRelic.UpdatePower(relic.damage);
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
            relic.passiveSkill.attackSequence,
            relic.passiveSkill.targetsAllowed,
            relic.passiveSkill.inflictType,
            relic.passiveSkill.maxTargetSelections,
            relic.passiveSkill.name,
            relic.passiveSkill.description,
            relic.passiveSkill.turnCooldown,
            relic.passiveSkill.power,
            relic.passiveSkill.missValueMultiplier,
            relic.passiveSkill.goodValueMultiplier,
            relic.passiveSkill.greatValueMultiplier,
            relic.passiveSkill.perfectValueMultiplier,
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
            relic.basicSkill.power,
            relic.basicSkill.missValueMultiplier,
            relic.basicSkill.goodValueMultiplier,
            relic.basicSkill.greatValueMultiplier,
            relic.basicSkill.perfectValueMultiplier,
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
            relic.primarySkill.power,
            relic.primarySkill.missValueMultiplier,
            relic.primarySkill.goodValueMultiplier,
            relic.primarySkill.greatValueMultiplier,
            relic.primarySkill.perfectValueMultiplier,
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
            relic.secondarySkill.power,
            relic.secondarySkill.missValueMultiplier,
            relic.secondarySkill.goodValueMultiplier,
            relic.secondarySkill.greatValueMultiplier,
            relic.secondarySkill.perfectValueMultiplier,
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
            relic.alternateSkill.power,
            relic.alternateSkill.missValueMultiplier,
            relic.alternateSkill.goodValueMultiplier,
            relic.alternateSkill.greatValueMultiplier,
            relic.alternateSkill.perfectValueMultiplier,
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
            relic.ultimateSkill.power,
            relic.ultimateSkill.missValueMultiplier,
            relic.ultimateSkill.goodValueMultiplier,
            relic.ultimateSkill.greatValueMultiplier,
            relic.ultimateSkill.perfectValueMultiplier,
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

        #endregion

        _skillUIManager.InitializeSkills(); // Sets relics skills

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

            _skillUIManager.AssignFirstSkill(_allies[i].basicSkill);    // Assign relic's active skill  on the start of their turn

            //AddSkillSelectionsManual(_allies[i].basicSkill);
            _skillUIManager.relicBasicSelect.ToggleSelectionImage();    // Apply basic skill 

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

    public void UpdateSkillUI()
    {

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

