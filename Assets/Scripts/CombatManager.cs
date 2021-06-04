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
    private UnitHUDInfo _unitHudInfo;
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
    public GameObject relicGO;

    [Space(3)]
    [Header("Combat Settings")]
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
    public float postHitTime;
    public float enemySelectSkillPostTime;
    public float enemyChooseSkillTime;
    public float enemySkillAnimationTime;
    public float ManaUpdateInterval;
    public float enemyDeathTime;

    [Space(3)]

    [Header("Effects Settings")]
    public int maxEffectsActive;
    //[HideInInspector]
    public SkillData relicActiveSkill;
    [HideInInspector]
    public SkillData enemyActiveSkill;
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
    public float activeSkillValueModifier, relicActiveSkillProcModifier;

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
    }

    public void StartBattle()
    {
        ToggleFightButton(false);
        SpawnRelic(startingRelic);
        SpawnEnemies(_activeRoom);
        _unitHudInfo.AssignUnitSkillsToSkillIcon();
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
    public IEnumerator BeginUnitTurn(bool enemyTeam)
    {
        // Disable relic attack bar + skills UI if its not relic turn
        if (enemyTeam)
        {
            StartCoroutine(uIManager.ToggleImage(uIManager.attackBarGO, false));
            //StartCoroutine(uIManager.ToggleImage(uIManager.relicSkillGO, false));
            StartCoroutine(uIManager.ToggleImage(uIManager.endTurnGO, false));

            // Display unit HUD info
            _unitHudInfo.SetValues(GetNextTurnUnit());
        }

        // Enable relic attack bar + skills UI on relic turn
        else
        {
            StartCoroutine(uIManager.ToggleImage(uIManager.attackBarGO, true, 0.3f));
            //StartCoroutine(uIManager.ToggleImage(uIManager.relicSkillGO, true, 0.3f));
            StartCoroutine(uIManager.ToggleImage(uIManager.endTurnGO, true, 0.3f));
            SetMaxUnitTargets(1);

            // Display unit HUD info
            _unitHudInfo.SetValues(GetNextTurnUnit());
        }

        // Update CDs
        //skillUIManager.AssignSkills(GetNextTurnUnit());     // assign skills for cd update
        //skillUIManager.DecrementSkillCooldown();    //Decrease all skill cooldowns

        activeUnit.UpdateTurnEnergy(0);   // Reset active unit's turn mana
        GetNextTurnUnit().ToggleTurnImage(true);    // Enable next unit's turn image

        _unitHudInfo.SetUnit(GetNextTurnUnit());

        if (enemyTeam)    
            yield return new WaitForSeconds(enemyUnitStartWait);    // Time to wait before unit is targeted as the active unit

        // Give next unit's mana for the start of their turn           
        StartCoroutine(GetNextTurnUnit().UpdateCurMana(GetNextTurnUnit().manaGainTurn));

        // Begin the next unit's turn, Trigger determine move
        StartCoroutine(GetNextTurnUnit().DetermineUnitMoveChoice(GetNextTurnUnit()));
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
        UpdateTurnOrder();     // Update turn orders
        ToggleAllTurnImages(false);  // Disable turn image 
        ClearUnitTargets();

        // Toggle off all unit Skill Icon UI Off
        uIManager.ToggleImage(uIManager.attackBarGO, false);
        _unitHudInfo.TogglePanel(_unitHudInfo.eAllSkillPanel, false);
        _unitHudInfo.TogglePanel(_unitHudInfo.eActiveSkillPanel, false);
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
        unit.passiveSkill = unit.gameObject.AddComponent<SkillData>();
        unit.basicSkill = unit.gameObject.AddComponent<SkillData>();
        unit.primarySkill = unit.gameObject.AddComponent<SkillData>();
        unit.secondarySkill = unit.gameObject.AddComponent<SkillData>();
        unit.alternateSkill = unit.gameObject.AddComponent<SkillData>();
        unit.ultimateSkill = unit.gameObject.AddComponent<SkillData>();
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

    public void UpdateActiveSkillTarget(Target target)
    {
        activeSkillTarget = target;
    }

    public void ClearUnitTargets()
    {
        // Clear all current target selections
        for (int i = 0; i < unitTargets.Count; i++)
        {
            unitTargets[i].selectEnabled = false; // Tell the oldest image script in stored selection list that the image is disabled
            unitTargets[i].selectionImage.enabled = false; // Disable the oldest image in stored selection list
        }

        curUnitTargets = 0;
        if (relicActiveSkill)
            relicActiveSkill.curTargetCount = 0;
        unitTargets.Clear();
        //_attackBar.UpdateUIAlpha(_attackBar.relicUIHider, _attackBar._relicUIHiderSelectVal);
    }

    void AddUnitTargets(int maxTargetSelections, bool enemy)
    {
        if (maxTargetSelections == 0)
        {
            if (enemy)
            {
                unitTargets.Add(activeRelic.target);    // Add relic target to target selections
                unitTargets[0].selectEnabled = true;    // update image enabled to true
                unitTargets[0].selectionImage.enabled = true;    // Disable the oldest image in stored selection list
                return;
            }
        }
        // Add current target selections
        if (maxTargetSelections > 0)
        {
            if (enemy)
            {
                curUnitTargets++;

                unitTargets.Add(activeRelic.transform.GetChild(0).GetComponent<Target>());    // Add relic target to target selections
                unitTargets[0].transform.GetChild(0).GetComponent<Target>().selectEnabled = true;    // update image enabled to true
                unitTargets[0].transform.GetChild(0).GetComponent<Image>().enabled = true;    // Disable the oldest image in stored selection list
            }
            else
            {
                for (int i = 0; i < maxTargetSelections; i++)
                {
                    curUnitTargets++;
                    relicActiveSkill.curTargetCount++;
                    unitTargets.Add(_enemiesPosition[i].target);

                    _enemiesPosition[i].target.selectEnabled = true;
                    _enemiesPosition[i].target.selectionImage.enabled = true;
                }
            }
        }
    }

    public void AddUnitTarget(Target targetUnit)
    {
        // If not currently at max unit targets
        if (curUnitTargets < maxUnitTargets)
        {
            AddCurrentUnitTargets(1);
            AddUnitTarget2(targetUnit);
            unitTargets[unitTargets.IndexOf(targetUnit)].selectEnabled = true;    // update image enabled to true
            unitTargets[unitTargets.IndexOf(targetUnit)].selectionImage.enabled = true;    // Disable the oldest image in stored selection list
        }
        // If current unit targets is equal to max unit targets
        else if (curUnitTargets == maxUnitTargets && curUnitTargets != 0)
        {
            unitTargets[0].selectEnabled = false;
            unitTargets[0].selectionImage.enabled = false;
            unitTargets.RemoveAt(0);

            unitTargets.Add(targetUnit);
            unitTargets[unitTargets.IndexOf(targetUnit)].selectEnabled = true;    // update image enabled to true
            unitTargets[unitTargets.IndexOf(targetUnit)].selectionImage.enabled = true;    // Disable the oldest image in stored selection list
        }
        // Add normally
        else if (curUnitTargets == maxUnitTargets && curUnitTargets == 0)
        {
            unitTargets.Add(targetUnit);
            unitTargets[unitTargets.IndexOf(targetUnit)].selectEnabled = true;    // update image enabled to true
            unitTargets[unitTargets.IndexOf(targetUnit)].selectionImage.enabled = true;    // Disable the oldest image in stored selection list
        }
    }

    public void RemoveUnitTarget(Target targetUnit)
    {
        curUnitTargets--;
        unitTargets[unitTargets.IndexOf(targetUnit)].selectEnabled = false;    // update image enabled to true
        unitTargets[unitTargets.IndexOf(targetUnit)].selectionImage.enabled = false;    // Disable the oldest image in stored selection list
        unitTargets.RemoveAt(unitTargets.IndexOf(targetUnit));
    }

    public void UpdateUnitTargets(Target selector, int selectedTargetIndex, bool cond, bool fromEnemy = false)
    {
        // False
        if (!cond)
        {
            if (unitTargets.Count != 0)
            {
                if (unitTargets.Count != 1)
                {
                    unitTargets.Remove(selector);
                    curUnitTargets--;
                    _enemiesPosition[selectedTargetIndex].transform.GetChild(0).GetComponent<Target>().selectEnabled = cond; // Tell the oldest image script in stored selection list that the image is disabled
                    _enemiesPosition[selectedTargetIndex].transform.GetChild(0).GetComponent<Image>().enabled = cond; // Disable the oldest image in stored selection list
                }
            }
        }
        // True
        else
        {
            // If player has 1 max selection and 1 already active, replace the current selection with the new one.
            if (curUnitTargets == maxUnitTargets && curUnitTargets == 1)
            {
                unitTargets[0].transform.GetComponent<Target>().selectEnabled = false;
                unitTargets[0].transform.GetComponent<Image>().enabled = false;
                _enemiesPosition[selectedTargetIndex].transform.GetChild(0).GetComponent<Target>().selectEnabled = cond; // Tell the oldest image script in stored selection list that the image is disabled
                _enemiesPosition[selectedTargetIndex].transform.GetChild(0).GetComponent<Image>().enabled = cond; // Disable the oldest image in stored selection list
                unitTargets.RemoveAt(0);
                unitTargets.Add(selector);

                //_attackBar.UpdateUIAlpha(_attackBar.relicUIHider, _attackBar._relicUIHiderOffVal);
                return;
            }

            if (curUnitTargets < maxUnitTargets)
            {
                curUnitTargets++;
                unitTargets.Add(selector);
                _enemiesPosition[selectedTargetIndex].transform.GetChild(0).GetComponent<Target>().selectEnabled = cond; // Tell the oldest image script in stored selection list that the image is disabled
                _enemiesPosition[selectedTargetIndex].transform.GetChild(0).GetComponent<Image>().enabled = cond; // Disable the oldest image in stored selection list
                return;
            }

            if (curUnitTargets == maxUnitTargets)
            {
                unitTargets[0].transform.GetComponent<Target>().selectEnabled = false;
                unitTargets[0].transform.GetComponent<Image>().enabled = false;
                unitTargets.RemoveAt(0);
                unitTargets.Add(selector);
                _enemiesPosition[selectedTargetIndex].transform.GetChild(0).GetComponent<Target>().selectEnabled = cond; // Tell the oldest image script in stored selection list that the image is disabled
                _enemiesPosition[selectedTargetIndex].transform.GetChild(0).GetComponent<Image>().enabled = cond; // Disable the oldest image in stored selection list
            }
        }
        //_attackBar.UpdateUIAlpha(_attackBar.relicUIHider, _attackBar._relicUIHiderOffVal);
    }

    #region Edit Skill Selections
    public void ClearSkillTargets()
    {
        // Clear all current target selections
        for (int i = 0; i < curSkillTargets; i++)
        {
            curSkillTargets--;
   
            skillTargets[i].transform.GetComponent<Target>().selectEnabled = false; // Tell the oldest image script in stored selection list that the image is disabled
            skillTargets[i].transform.GetComponent<Image>().enabled = false; // Disable the oldest image in stored selection list
        }

        skillTargets.Clear();
    }

    public void AddSkillTarget(Target target, SkillData skillData, bool spawnHitMarker)
    {
        curSkillTargets++;

        skillTargets.Add(target);
        target.transform.GetComponent<Target>().selectEnabled = true; // Tell the oldest image script in stored selection list that the image is disabled
        target.transform.GetComponent<Image>().enabled = true; // Disable the oldest image in stored selection list

        if (spawnHitMarker)
            _attackBar.SpawnHitMarker(skillData);
    }
    #endregion

    /// <summary>
    /// Updates selection images
    /// </summary>
    public void ManageTargets(bool isSkill, bool enemy, Target selectionInput, 
        int curTargetSelections, int maxTargetSelections = 0, SkillData skillData = null)
    {
        //this.curSkillSelections = curSkillSelections;
        //this.maxSkillSelections = maxSkillSelections;
        this.curUnitTargets = curTargetSelections;
        this.maxUnitTargets = maxTargetSelections;

        if (isSkill)
        {
            ClearSkillTargets();
            AddSkillTarget(selectionInput, skillData, true);
        }

        ClearUnitTargets();
        AddUnitTargets(maxTargetSelections, enemy);
    }

    #region Adjusting Unit Targets list / CurUnitTargets / MaxUnitTargets
    void SetCurrentUnitTargets(int val)
    {
        curUnitTargets = val;
    }

    void AddCurrentUnitTargets(int val)
    {
        curUnitTargets += val;
    }

    void RemoveCurrentUnitTargets(int val)
    {
        curUnitTargets -= val;
    }

    void SetMaxUnitTargets(int val)
    {
        maxUnitTargets = val;
    }

    void SetUnitTargets(List<Target> tars)
    {
        for (int i = 0; i < tars.Count; i++)
        {
            unitTargets.Add(tars[i]);
        }
    }

    void AddUnitTarget2(Target t)
    {
        unitTargets.Add(t);
    }

    void RemoveUnitTarget2(Target t)
    {
        unitTargets.Remove(t);
    }

    void RemoveAllUnitTargets()
    {
        unitTargets.Clear();
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
    #endregion

    public float CalculateDamageDealt(float value = 0, float valueModifier = 1, float targetCountPowerInc = 1, float targetCountValAmp = 1)
    {
        return (value * valueModifier) * (targetCountValAmp * targetCountPowerInc);
    }

    #region Utility

    #endregion

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
            activeEnemy.passiveSkill.InitializeSkill(
                room.roomEnemies[i].passiveSkill.skillIconColour,
                room.roomEnemies[i].passiveSkill.skillBorderColour,
                room.roomEnemies[i].passiveSkill.skillSelectionColour,
                room.roomEnemies[i].passiveSkill.skillType,
                room.roomEnemies[i].passiveSkill.skillMode,
                room.roomEnemies[i].passiveSkill.targetType,
                room.roomEnemies[i].passiveSkill.targetsAllowed,
                room.roomEnemies[i].passiveSkill.hitsRequired,
                room.roomEnemies[i].passiveSkill.manaRequired,
                room.roomEnemies[i].passiveSkill.timeBetweenHitUI,
                room.roomEnemies[i].passiveSkill.timeTillEffectInflict,
                room.roomEnemies[i].passiveSkill.timeForNextHitMarker,
                room.roomEnemies[i].passiveSkill.effect,
                room.roomEnemies[i].passiveSkill.effectTarget,
                room.roomEnemies[i].passiveSkill.effectPower,
                room.roomEnemies[i].passiveSkill.effectDuration,
                room.roomEnemies[i].passiveSkill.effectHitEffect,
                room.roomEnemies[i].passiveSkill.effectDurationDecrease,
                room.roomEnemies[i].passiveSkill.counterSkill,
                room.roomEnemies[i].passiveSkill.stackValue,
                room.roomEnemies[i].passiveSkill.targetAmountPowerInc,
                room.roomEnemies[i].passiveSkill.isTargetCountValAmp,
                room.roomEnemies[i].passiveSkill.maxTargetCount,
                room.roomEnemies[i].passiveSkill.activatable,
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

            activeEnemy.secondarySkill.InitializeSkill(
                room.roomEnemies[i].secondarySkill.skillIconColour,
                room.roomEnemies[i].secondarySkill.skillBorderColour,
                room.roomEnemies[i].secondarySkill.skillSelectionColour,
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

            activeEnemy.ultimateSkill.InitializeSkill(
                room.roomEnemies[i].ultimateSkill.skillIconColour,
                room.roomEnemies[i].ultimateSkill.skillBorderColour,
                room.roomEnemies[i].ultimateSkill.skillSelectionColour,
                room.roomEnemies[i].ultimateSkill.skillType,
                room.roomEnemies[i].ultimateSkill.skillMode,
                room.roomEnemies[i].ultimateSkill.targetType,
                room.roomEnemies[i].ultimateSkill.targetsAllowed,
                room.roomEnemies[i].ultimateSkill.hitsRequired,
                room.roomEnemies[i].ultimateSkill.manaRequired,
                room.roomEnemies[i].ultimateSkill.timeBetweenHitUI,
                room.roomEnemies[i].ultimateSkill.timeTillEffectInflict,
                room.roomEnemies[i].ultimateSkill.timeForNextHitMarker,
                room.roomEnemies[i].ultimateSkill.effect,
                room.roomEnemies[i].ultimateSkill.effectTarget,
                room.roomEnemies[i].ultimateSkill.effectPower,
                room.roomEnemies[i].ultimateSkill.effectDuration,
                room.roomEnemies[i].ultimateSkill.effectHitEffect,
                room.roomEnemies[i].ultimateSkill.effectDurationDecrease,
                room.roomEnemies[i].ultimateSkill.counterSkill,
                room.roomEnemies[i].ultimateSkill.stackValue,
                room.roomEnemies[i].ultimateSkill.targetAmountPowerInc,
                room.roomEnemies[i].ultimateSkill.isTargetCountValAmp,
                room.roomEnemies[i].ultimateSkill.maxTargetCount,
                room.roomEnemies[i].ultimateSkill.activatable,
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

        Image image = go.AddComponent<Image>();
        image.color = relic.color;

        UpdateActiveRelic(go.GetComponent<Unit>());

        GameObject skillUIPar = Instantiate(skillUIValuesPar, activeRelic.transform.position, Quaternion.identity);
        activeRelic.skillUIValueParent = skillUIPar.transform;

        skillUIPar.transform.SetParent(go.transform);

        activeRelic.skillUIValueParent = go.transform.Find("Skill UI Values");

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
        activeRelic.passiveSkill.InitializeSkill(
            relic.passiveSkill.skillIconColour,
            relic.passiveSkill.skillBorderColour,
            relic.passiveSkill.skillSelectionColour,
            relic.passiveSkill.skillType,
            relic.passiveSkill.skillMode,
            relic.passiveSkill.targetType,
            relic.passiveSkill.targetsAllowed,
            relic.passiveSkill.hitsRequired,
            relic.passiveSkill.manaRequired,
            relic.passiveSkill.timeBetweenHitUI,
            relic.passiveSkill.timeTillEffectInflict,
            relic.passiveSkill.timeForNextHitMarker,
            relic.passiveSkill.effect,
            relic.passiveSkill.effectTarget,
            relic.passiveSkill.effectPower,
            relic.passiveSkill.effectDuration,
            relic.passiveSkill.effectHitEffect,
            relic.passiveSkill.effectDurationDecrease,
            relic.passiveSkill.counterSkill,
            relic.passiveSkill.stackValue,
            relic.passiveSkill.targetAmountPowerInc,
            relic.passiveSkill.isTargetCountValAmp,
            relic.passiveSkill.maxTargetCount,
            relic.passiveSkill.activatable,
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

        activeRelic.basicSkill.InitializeSkill(
            relic.basicSkill.skillIconColour,
            relic.basicSkill.skillBorderColour,
            relic.basicSkill.skillSelectionColour,
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

        activeRelic.ultimateSkill.InitializeSkill(
            relic.ultimateSkill.skillIconColour,
            relic.ultimateSkill.skillBorderColour,
            relic.ultimateSkill.skillSelectionColour,
            relic.ultimateSkill.skillType,
            relic.ultimateSkill.skillMode,
            relic.ultimateSkill.targetType,
            relic.ultimateSkill.targetsAllowed,
            relic.ultimateSkill.hitsRequired,
            relic.ultimateSkill.manaRequired,
            relic.ultimateSkill.timeBetweenHitUI,
            relic.ultimateSkill.timeTillEffectInflict,
            relic.ultimateSkill.timeForNextHitMarker,
            relic.ultimateSkill.effect,
            relic.ultimateSkill.effectTarget,
            relic.ultimateSkill.effectPower,
            relic.ultimateSkill.effectDuration,
            relic.ultimateSkill.effectHitEffect,
            relic.ultimateSkill.effectDurationDecrease,
            relic.ultimateSkill.counterSkill,
            relic.ultimateSkill.stackValue,
            relic.ultimateSkill.targetAmountPowerInc,
            relic.ultimateSkill.isTargetCountValAmp,
            relic.ultimateSkill.maxTargetCount,
            relic.ultimateSkill.activatable,
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
        #endregion

        #endregion

        skillUIManager.InitializeSkills(activeRelic); // Sets relics skills
                                           // Add relic and enemies to stored list
        turnOrder.Add(activeRelic);

        //activeRelic.selector.enemyIndex = _enemiesPosition.Count + 1;

        //activeRelic.CalculateEnergy();
    }
}

