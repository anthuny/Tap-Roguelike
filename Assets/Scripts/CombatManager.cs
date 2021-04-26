using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private AttackBar _attackBar;
    [SerializeField] private Button _fightButton;

    [HideInInspector]
    public DevManager _devManager;

    [Header("Relic Settings")]
    [Tooltip("The time that must elapse before another thing happens")]
    public float breatheTime = .25f;
    [Tooltip("The amount of time in seconds that must elapse after the enemy spawns before the next stage can begin")]
    public float enemyTimeWaitSpawn;

    [SerializeField] private Transform _enemySpawnPoint;
    [SerializeField] private Transform _RelicSpawnPoint;
    [SerializeField] private Room _activeRoom;
    [SerializeField] private List<Unit> _enemies = new List<Unit>();
    [SerializeField] private Relic _startingRelic;
    [SerializeField] private float PostEnemyAttackWait;

    private bool relicInitialized;
    [HideInInspector]
    public bool relicTurn;
    public Unit activeRelic;

    public void StartBattle()
    {
        ToggleFightButton(false);

        SpawnRelic(_startingRelic);
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

            //Image image = go.AddComponent<Image>();
            //image.color = room.enemies[i].color;

            Unit unit = go.AddComponent<Unit>();
            unit.name = room.enemies[i].name;
            unit.level = room.enemies[i].level;
            unit.color = room.enemies[i].color;
            unit.maxHealth = room.enemies[i].maxHealth;
            unit.curHealth = room.enemies[i].curHealth;
            unit.damage = room.enemies[i].damage;
            unit.speed = room.enemies[i].speed;
            unit.AdjustAttackChance(room.enemies[i].attackChance, true);

            #region Set Skills
            unit.passiveSkills = room.enemies[i].passiveSkills;
            unit.primarySkills = room.enemies[i].primarySkills;
            unit.secondarySkills = room.enemies[i].secondarySkills;
            unit.alternateSkills = room.enemies[i].alternateSkills;
            unit.ultimateSkills = room.enemies[i].ultimateSkills;
            #endregion

            _enemies.Add(unit);
            unit.targets.Add(activeRelic);
            unit.CalculateSpeedFinal();
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
        activeRelic.name = relic.name;
        activeRelic.level = 1;
        activeRelic.color = relic.color;
        activeRelic.maxHealth = relic.maxHealth;
        activeRelic.curHealth = relic.curHealth;
        activeRelic.damage = relic.damage;
        activeRelic.speed = relic.speed;

        #region Set Skills
        activeRelic.passiveSkills = relic.passiveSkills;
        activeRelic.primarySkills = relic.primarySkills;
        activeRelic.secondarySkills = relic.secondarySkills;
        activeRelic.alternateSkills = relic.alternateSkills;
        activeRelic.ultimateSkills = relic.ultimateSkills;
        #endregion

        activeRelic.CalculateSpeedFinal();
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
            StartRelicTurn();   // Start relic turn
    }

    void StartRelicTurn()
    {
        StartCoroutine(_devManager.FlashText("Starting Relic's turn"));

        // Start attack bar hit marker
        _attackBar.BeginHitMarkerStartingSequence();

        relicTurn = true;
    }

    IEnumerator StartEnemysTurn()
    {
        StartCoroutine(_devManager.FlashText("Starting enemy(s) turn"));

        // If it's the relic's turn
        if (relicTurn)
            _attackBar.StartCoroutine("BeginHitMarkerStoppingSequence"); // Stop the hit marker


        // Toggle on the attack bar hider
        _attackBar.UpdateAttackBarHider(_attackBar._attackBarHiderOnVal);

        // Loop through room's enemies
        for (int i = 0; i < _enemies.Count; i++)
        {
            // If i enemy's attack chance is greater then their skill chance
            if (_enemies[i].attackChance > _enemies[i].skillChance)
            {
                // Enemy damages relic with regular attack
                activeRelic.AdjustCurHealth(RoundFloatToInt(-(_enemies[i].damage)), _enemies[i], activeRelic);
            }
            // If i enemy's skill chance is greater then their skill chance
            else
            {
                // Enemy casts a random skill
                _enemies[i].SkillFunctionality(_enemies[i].primarySkills[Random.Range(0, _enemies[i].primarySkills.Count)]);
            }

            yield return new WaitForSeconds(PostEnemyAttackWait);
        }

        StartRelicTurn();
    }

    public void SelectTarget(Unit target, Image selectionImage)
    {

    }

    #region Utility
    private int RoundFloatToInt(float f)
    {
        return Mathf.RoundToInt(f);
    }

    #endregion




    /*
    // Determine what action the enemy is going to do this turn
    //float attackRoll = Random.Range(0, _activeEnemy.attackChance);
    //float skillRoll = Random.Range(0, _activeEnemy.skillChance);


    if (attackRoll > skillRoll)
    {
        // Debug
        StartCoroutine(_devManager.FlashText("Enemy used Attack " + "AttackRoll: " + attackRoll + " > " + "SkillRoll: " + skillRoll));

        currentEnemyState = EnemyState.ATTACKING;

        Invoke("EnemyAttack", breatheTime);
    }

    else
    {
        // Debug
        StartCoroutine(_devManager.FlashText("Enemy used Skill " + "SkillRoll: " + skillRoll + " > " + "AttackRoll: " + attackRoll));

        currentEnemyState = EnemyState.CASTSKILL;

        Invoke("EnemyChooseSkill", breatheTime);
    }
    */
}


/*
public void BeginEnemyTurnSequence()
{
    DetermineEnemyMove(breatheTime);
}

public IEnumerator DetermineEnemyMove(float timer = 0)
{
    yield return new WaitForSeconds(timer);

    // Debug
    StartCoroutine(_devManager.FlashText("Initiating Enemy Turn"));

    // Determine what action the enemy is going to do this turn
    float attackRoll = Random.Range(0, _activeEnemy.attackChance);
    float skillRoll = Random.Range(0, _activeEnemy.skillChance);

    yield return new WaitForSeconds(timer);

    if (attackRoll > skillRoll)
    {
        // Debug
        StartCoroutine(_devManager.FlashText("Enemy used Attack " + "AttackRoll: " + attackRoll + " > " + "SkillRoll: " + skillRoll));

        currentEnemyState = EnemyState.ATTACKING;

        Invoke("EnemyAttack", breatheTime);
    }

    else
    {
        // Debug
        StartCoroutine(_devManager.FlashText("Enemy used Skill " + "SkillRoll: " + skillRoll + " > " + "AttackRoll: " + attackRoll));

        currentEnemyState = EnemyState.CASTSKILL;

        Invoke("EnemyChooseSkill", breatheTime);
    }
}

void EnemyAttack()
{
    // Debug
    StartCoroutine(_devManager.FlashText("Player damaged for " + _activeEnemy.enemyDamage));

    _relicManager.RecieveDamage(_activeEnemy.enemyDamage);

    // Player's turn first
    Invoke("BeginPlayerTurn", breatheTime);
}

void EnemyChooseSkill()
{
    currentSelectedSkill = (CombatManager.EnemySelectedSkill)Random.Range(0, 2);

    // Debug
    StartCoroutine(_devManager.FlashText("Enemy selected skill is " + currentSelectedSkill));

    // Player's turn first
    Invoke("BeginPlayerTurn", breatheTime);
}


void CastSkill()
{

}

void BeginPlayerTurn()
{
    // Debug
    StartCoroutine(_devManager.FlashText("Initiating Player Turn"));

    // Begin player's active hit bar attack
    _curAttackBar.Invoke("BeginHitMarkerStartingSequence", 0);
}

/// <summary>
/// Decides whether the player or enemy attacks first
/// </summary>
void EnemyDecideAttackOrder()
{
    int startSuccess = Random.Range(0, 101);

    if (_activeEnemy.enemySpeed > startSuccess)
    {
        // Debug
        StartCoroutine(_devManager.FlashText(_activeEnemy.enemyName + "'s wins start success roll with " + _activeEnemy.enemySpeed + " > Player's " + startSuccess));

        // Enemy's turn first
        StartCoroutine(DetermineEnemyMove(breatheTime));
    }
    else
    {
        // Debug
        StartCoroutine(_devManager.FlashText(_activeEnemy.enemyName + "'s loses start success roll with Player's " + startSuccess + " > " + _activeEnemy.enemySpeed));

        // Enemy's turn first
        Invoke("BeginPlayerTurn", breatheTime);
    }
}

public void SpawnEnemy()
{
    // Spawn enemy
    StartCoroutine(EnemySpawn());
}
*/

