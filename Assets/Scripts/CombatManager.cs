using System.Collections;
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
    private List<Unit> _enemies = new List<Unit>();
    private List<Unit> _allies = new List<Unit>();
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
    private List<SelectionInput> totalSelections = new List<SelectionInput>();
    [HideInInspector]
    public Unit activeRelic, activeEnemy;
    private int curSelections;
    private bool relicInitialized;
    private bool relicTurn;
    [HideInInspector]
    public int maxSelections;

    private DevManager _devManager;


    private void Awake()
    {
        _devManager = FindObjectOfType<DevManager>();

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

        for (int i = 0; i < room.enemies.Count; i++)
        {
            GameObject go = Instantiate(room.enemyGO[i]);
            go.name = room.enemies[i].name;
            go.transform.SetParent(_enemySpawnPoint.GetChild(i).transform);
            go.transform.position = _enemySpawnPoint.GetChild(i).transform.position;

            activeEnemy = go.AddComponent<Unit>();
            activeEnemy.unitType = Unit.UnitType.ENEMY;
            activeEnemy.name = room.enemies[i].name;
            activeEnemy.level = room.enemies[i].level;
            activeEnemy.color = room.enemies[i].color;
            activeEnemy.maxHealth = room.enemies[i].maxHealth;
            activeEnemy.curHealth = room.enemies[i].curHealth;
            activeEnemy.damage = room.enemies[i].damage;
            activeEnemy.speed = room.enemies[i].speed;
            activeEnemy.AdjustAttackChance(room.enemies[i].attackChance, true);

            InitializeUnitSkills(activeEnemy);

            #region Initialize Enemy Skills
            activeEnemy.passiveSkill.InitializeSkill(
                room.enemies[i].passiveSkill.skillType,
                room.enemies[i].passiveSkill.skillMode,
                room.enemies[i].passiveSkill.targetType,
                room.enemies[i].passiveSkill.attackSequence,
                room.enemies[i].passiveSkill.targetsAllowed,
                room.enemies[i].passiveSkill.inflictType,
                room.enemies[i].passiveSkill.targetCount,
                room.enemies[i].passiveSkill.name,
                room.enemies[i].passiveSkill.description,
                room.enemies[i].passiveSkill.turnCooldown,
                room.enemies[i].passiveSkill.damage,
                room.enemies[i].passiveSkill.missDamageMultiplier,
                room.enemies[i].passiveSkill.goodDamageMultiplier,
                room.enemies[i].passiveSkill.greatDamageMultiplier,
                room.enemies[i].passiveSkill.perfectDamageMultiplier,
                room.enemies[i].passiveSkill.procChance,
                room.enemies[i].passiveSkill.missProcMultiplier,
                room.enemies[i].passiveSkill.goodProcMultiplier,
                room.enemies[i].passiveSkill.greatProcMultiplier,
                room.enemies[i].passiveSkill.perfectProcMultiplier);

            activeEnemy.basicSkill.InitializeSkill(
                room.enemies[i].basicSkill.skillType,
                room.enemies[i].basicSkill.skillMode,
                room.enemies[i].basicSkill.targetType,
                room.enemies[i].basicSkill.attackSequence,
                room.enemies[i].basicSkill.targetsAllowed,
                room.enemies[i].basicSkill.inflictType,
                room.enemies[i].basicSkill.targetCount,
                room.enemies[i].basicSkill.name,
                room.enemies[i].basicSkill.description,
                room.enemies[i].basicSkill.turnCooldown,
                room.enemies[i].basicSkill.damage,
                room.enemies[i].basicSkill.missDamageMultiplier,
                room.enemies[i].basicSkill.goodDamageMultiplier,
                room.enemies[i].basicSkill.greatDamageMultiplier,
                room.enemies[i].basicSkill.perfectDamageMultiplier,
                room.enemies[i].basicSkill.procChance,
                room.enemies[i].basicSkill.missProcMultiplier,
                room.enemies[i].basicSkill.goodProcMultiplier,
                room.enemies[i].basicSkill.greatProcMultiplier,
                room.enemies[i].basicSkill.perfectProcMultiplier);

            activeEnemy.primarySkill.InitializeSkill(
                room.enemies[i].primarySkill.skillType,
                room.enemies[i].primarySkill.skillMode,
                room.enemies[i].primarySkill.targetType,
                room.enemies[i].primarySkill.attackSequence,
                room.enemies[i].primarySkill.targetsAllowed,
                room.enemies[i].primarySkill.inflictType,
                room.enemies[i].primarySkill.targetCount,
                room.enemies[i].primarySkill.name,
                room.enemies[i].primarySkill.description,
                room.enemies[i].primarySkill.turnCooldown,
                room.enemies[i].primarySkill.damage,
                room.enemies[i].primarySkill.missDamageMultiplier,
                room.enemies[i].primarySkill.goodDamageMultiplier,
                room.enemies[i].primarySkill.greatDamageMultiplier,
                room.enemies[i].primarySkill.perfectDamageMultiplier,
                room.enemies[i].primarySkill.procChance,
                room.enemies[i].primarySkill.missProcMultiplier,
                room.enemies[i].primarySkill.goodProcMultiplier,
                room.enemies[i].primarySkill.greatProcMultiplier,
                room.enemies[i].primarySkill.perfectProcMultiplier);

            activeEnemy.secondarySkill.InitializeSkill(
                room.enemies[i].secondarySkill.skillType,
                room.enemies[i].secondarySkill.skillMode,
                room.enemies[i].secondarySkill.targetType,
                room.enemies[i].secondarySkill.attackSequence,
                room.enemies[i].secondarySkill.targetsAllowed,
                room.enemies[i].secondarySkill.inflictType,
                room.enemies[i].secondarySkill.targetCount,
                room.enemies[i].secondarySkill.name,
                room.enemies[i].secondarySkill.description,
                room.enemies[i].secondarySkill.turnCooldown,
                room.enemies[i].secondarySkill.damage,
                room.enemies[i].secondarySkill.missDamageMultiplier,
                room.enemies[i].secondarySkill.goodDamageMultiplier,
                room.enemies[i].secondarySkill.greatDamageMultiplier,
                room.enemies[i].secondarySkill.perfectDamageMultiplier,
                room.enemies[i].secondarySkill.procChance,
                room.enemies[i].secondarySkill.missProcMultiplier,
                room.enemies[i].secondarySkill.goodProcMultiplier,
                room.enemies[i].secondarySkill.greatProcMultiplier,
                room.enemies[i].secondarySkill.perfectProcMultiplier);

            activeEnemy.alternateSkill.InitializeSkill(
                room.enemies[i].alternateSkill.skillType,
                room.enemies[i].alternateSkill.skillMode,
                room.enemies[i].alternateSkill.targetType,
                room.enemies[i].alternateSkill.attackSequence,
                room.enemies[i].alternateSkill.targetsAllowed,
                room.enemies[i].alternateSkill.inflictType,
                room.enemies[i].alternateSkill.targetCount,
                room.enemies[i].alternateSkill.name,
                room.enemies[i].alternateSkill.description,
                room.enemies[i].alternateSkill.turnCooldown,
                room.enemies[i].alternateSkill.damage,
                room.enemies[i].alternateSkill.missDamageMultiplier,
                room.enemies[i].alternateSkill.goodDamageMultiplier,
                room.enemies[i].alternateSkill.greatDamageMultiplier,
                room.enemies[i].alternateSkill.perfectDamageMultiplier,
                room.enemies[i].alternateSkill.procChance,
                room.enemies[i].alternateSkill.missProcMultiplier,
                room.enemies[i].alternateSkill.goodProcMultiplier,
                room.enemies[i].alternateSkill.greatProcMultiplier,
                room.enemies[i].alternateSkill.perfectProcMultiplier);

            activeEnemy.ultimateSkill.InitializeSkill(
                room.enemies[i].ultimateSkill.skillType,
                room.enemies[i].ultimateSkill.skillMode,
                room.enemies[i].ultimateSkill.targetType,
                room.enemies[i].ultimateSkill.attackSequence,
                room.enemies[i].ultimateSkill.targetsAllowed,
                room.enemies[i].ultimateSkill.inflictType,
                room.enemies[i].ultimateSkill.targetCount,
                room.enemies[i].ultimateSkill.name,
                room.enemies[i].ultimateSkill.description,
                room.enemies[i].ultimateSkill.turnCooldown,
                room.enemies[i].ultimateSkill.damage,
                room.enemies[i].ultimateSkill.missDamageMultiplier,
                room.enemies[i].ultimateSkill.goodDamageMultiplier,
                room.enemies[i].ultimateSkill.greatDamageMultiplier,
                room.enemies[i].ultimateSkill.perfectDamageMultiplier,
                room.enemies[i].ultimateSkill.procChance,
                room.enemies[i].ultimateSkill.missProcMultiplier,
                room.enemies[i].ultimateSkill.goodProcMultiplier,
                room.enemies[i].ultimateSkill.greatProcMultiplier,
                room.enemies[i].ultimateSkill.perfectProcMultiplier);

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
            relic.passiveSkill.skillType, 
            relic.passiveSkill.skillMode,
            relic.passiveSkill.targetType,
            relic.passiveSkill.attackSequence, 
            relic.passiveSkill.targetsAllowed,
            relic.passiveSkill.inflictType, 
            relic.passiveSkill.targetCount,
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
            relic.passiveSkill.perfectProcMultiplier);

        activeRelic.basicSkill.InitializeSkill(
            relic.basicSkill.skillType,
            relic.basicSkill.skillMode,
            relic.basicSkill.targetType,
            relic.basicSkill.attackSequence,
            relic.basicSkill.targetsAllowed,
            relic.basicSkill.inflictType,
            relic.basicSkill.targetCount,
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
            relic.basicSkill.perfectProcMultiplier);

        activeRelic.primarySkill.InitializeSkill(
            relic.primarySkill.skillType,
            relic.primarySkill.skillMode,
            relic.primarySkill.targetType,
            relic.primarySkill.attackSequence,
            relic.primarySkill.targetsAllowed,
            relic.primarySkill.inflictType,
            relic.primarySkill.targetCount,
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
            relic.primarySkill.perfectProcMultiplier);

        activeRelic.secondarySkill.InitializeSkill(
            relic.secondarySkill.skillType,
            relic.secondarySkill.skillMode,
            relic.secondarySkill.targetType,
            relic.secondarySkill.attackSequence,
            relic.secondarySkill.targetsAllowed,
            relic.secondarySkill.inflictType,
            relic.secondarySkill.targetCount,
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
            relic.secondarySkill.perfectProcMultiplier);

        activeRelic.alternateSkill.InitializeSkill(
            relic.alternateSkill.skillType,
            relic.alternateSkill.skillMode,
            relic.alternateSkill.targetType,
            relic.alternateSkill.attackSequence,
            relic.alternateSkill.targetsAllowed,
            relic.alternateSkill.inflictType,
            relic.alternateSkill.targetCount,
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
            relic.alternateSkill.perfectProcMultiplier);

        activeRelic.ultimateSkill.InitializeSkill(
            relic.ultimateSkill.skillType,
            relic.ultimateSkill.skillMode,
            relic.ultimateSkill.targetType,
            relic.ultimateSkill.attackSequence,
            relic.ultimateSkill.targetsAllowed,
            relic.ultimateSkill.inflictType,
            relic.ultimateSkill.targetCount,
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
            relic.ultimateSkill.perfectProcMultiplier);
        #endregion

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
    public void ManageSelectionCount(bool enabled, SelectionInput selection)
    {
        // If the current amount of selections equals the maximum amount of selections for the active unit's current skill
        if (curSelections == maxSelections)
        {
            if (enabled)    // If a selection is being enabled
            {
                selection.selectionImage.enabled = true;    // Enabled the image
                selection.selectEnabled = true; // Tell the image script that the image is enabled
                totalSelections.Add(selection); // Add select to list for storing
                totalSelections[0].selectionImage.enabled = false;  // Disable the oldest image in stored selection list
                totalSelections[0].selectEnabled = false;   // Tell the oldest image script in stored selection list that the image is disabled
                totalSelections.RemoveAt(0);    // Remove the oldest image from the stored active selection list

                UpdateRelicUIHider(_relicUIHiderOffVal);
            }
            else    // If a selection is being disabled
            {
                curSelections--;    // Remove a count from the current selections
                selection.selectionImage.enabled = false;   // Disable the image
                selection.selectEnabled = false;    // Tell the image script that the image is disabled
                totalSelections.Remove(selection);      // Remove selection from stored active selection list

                UpdateRelicUIHider(_relicUIHiderSelectVal);
                return;
            }
        }

        // If the current amount of selections equals the maximum amount of selections for the active unit's current skill
        else if (curSelections < maxSelections)
        {
            if (enabled)    // If a selection is being enabled
            {
                curSelections++;     // Add a count from the current selections
                selection.selectionImage.enabled = true;    // Enabled the image
                selection.selectEnabled = true;     // Tell the image script that the image is enabled
                totalSelections.Add(selection);     // Add selection into stored active selection list

                UpdateRelicUIHider(_relicUIHiderOffVal);
            }
            else    // If a selection is being disabled
            {
                curSelections--;    // Remove a count from the current selections
                selection.selectionImage.enabled = false;   // Disable the image
                selection.selectEnabled = false;
                totalSelections.Remove(selection);

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

