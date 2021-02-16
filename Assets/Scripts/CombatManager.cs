using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    private enum EnemyState { ATTACKING, CASTSKILL };
    private EnemyState currentEnemyState;
    private enum EnemySelectedSkill { HEAL, DMGAMP };
    private EnemySelectedSkill currentSelectedSkill;

    [SerializeField] private Gamemode _gamemode;
    [SerializeField] private AttackBar _curAttackBar;
    [SerializeField] private RelicManager _relicManager;
    [SerializeField] private DevManager _devManager;
    [SerializeField] private Enemy[] _enemy;

    [Header("Relic Settings")]
    [Tooltip("The time that must elapse before another thing happens")]
    public float breatheTime = .25f;

    [Tooltip("The amount of time in seconds that must elapse after the enemy spawns before the next stage can begin")]
    public float enemyTimeWaitSpawn;

    [SerializeField] private Transform _enemySpawnPoint;
    private Enemy _activeEnemy;

    // Start is called before the first frame update
    void Start()
    {
        _activeEnemy = _enemy[Random.Range(0, _enemy.Length)];
    }

    public void BeginEnemyTurnSequence()
    {
        DetermineEnemyMove(breatheTime);
    }

    /// <summary>
    /// Spawn an enemy
    /// </summary>
    /// <returns></returns>
    public IEnumerator EnemySpawn()
    {
        StartCoroutine(_devManager.FlashText("Spawning " + _activeEnemy.enemyName));

        // Spawn an enemy, and get a reference to the dev text
        GameObject go = Instantiate(_activeEnemy.enemyVisual, _enemySpawnPoint.position, Quaternion.identity);
        
        yield return new WaitForSeconds(enemyTimeWaitSpawn);

        // Choose whether the enemy will get their turn first, or the player 
        Invoke("EnemyDecideAttackOrder", breatheTime);
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
}
