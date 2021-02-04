using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    private Gamemode gm;
    public Transform enemySpawnPoint;
    public Enemy activeEnemy;
    public Enemy[] enemy;

    [HideInInspector]
    public enum EnemyState {ATTACKING, CASTSKILL };
    [HideInInspector]
    public EnemyState currentEnemyState;

    [HideInInspector]
    public enum EnemySelectedSkill { HEAL, DMGAMP };
    [HideInInspector]
    public EnemySelectedSkill currentSelectedSkill;

    private IEnumerator coroutine;

    private void Awake()
    {
        gm = FindObjectOfType<Gamemode>();
    }
    // Start is called before the first frame update
    void Start()
    {
        activeEnemy = enemy[Random.Range(0, enemy.Length)];
    }

    /// <summary>
    /// Spawn an enemy
    /// </summary>
    /// <returns></returns>
    public IEnumerator EnemySpawn()
    {
        StartCoroutine(gm.dm.FlashText("Spawning " + activeEnemy.enemyName));

        // Spawn an enemy, and get a reference to the dev text
        GameObject go = Instantiate(activeEnemy.enemyVisual, enemySpawnPoint.position, Quaternion.identity);
        
        yield return new WaitForSeconds(gm.EnemyTimeWaitSpawn);

        // Choose whether the enemy will get their turn first, or the player 
        Invoke("EnemyDecideAttackOrder", gm.breatheTime);
    }

    /// <summary>
    /// Decides whether the player or enemy attacks first
    /// </summary>
    void EnemyDecideAttackOrder()
    {
        int startSuccess = Random.Range(0, 101);

        if (activeEnemy.enemySpeed > startSuccess)
        {
            // Debug
            StartCoroutine(gm.dm.FlashText(activeEnemy.enemyName + "'s wins start success roll with " + activeEnemy.enemySpeed + " > Player's " + startSuccess));

            // Enemy's turn first
            StartCoroutine(DetermineEnemyMove(gm.breatheTime));
        }
        else
        {
            // Debug
            StartCoroutine(gm.dm.FlashText(activeEnemy.enemyName + "'s loses start success roll with Player's " + startSuccess + " > " + activeEnemy.enemySpeed));

            // Enemy's turn first
            Invoke("BeginPlayerTurn", gm.breatheTime);
        }
    }

    void BeginPlayerTurn()
    {
        // Debug
        StartCoroutine(gm.dm.FlashText("Initiating Player Turn"));

        // Begin player's active hit bar attack
        gm.ab.Invoke("BeginAttackBarPattern", gm.breatheTime);
    }

    public IEnumerator DetermineEnemyMove(float timer = 0)
    {
        yield return new WaitForSeconds(timer);

        // Debug
        StartCoroutine(gm.dm.FlashText("Initiating Enemy Turn"));

        // Determine what action the enemy is going to do this turn
        float attackRoll = Random.Range(0, activeEnemy.attackChance);
        float skillRoll = Random.Range(0, activeEnemy.skillChance);

        yield return new WaitForSeconds(timer);

        if (attackRoll > skillRoll)
        {
            // Debug
            StartCoroutine(gm.dm.FlashText("Enemy used Attack " + "AttackRoll: " + attackRoll + " > " + "SkillRoll: " + skillRoll));

            currentEnemyState = EnemyState.ATTACKING;

            Invoke("EnemyAttack", gm.breatheTime);
        }

        else
        {
            // Debug
            StartCoroutine(gm.dm.FlashText("Enemy used Skill " + "SkillRoll: " + skillRoll + " > " + "AttackRoll: " + attackRoll));

            currentEnemyState = EnemyState.CASTSKILL;

            Invoke("EnemyChooseSkill", gm.breatheTime);
        }
    }

    void EnemyAttack()
    {
        // Debug
        StartCoroutine(gm.dm.FlashText("Player damaged for " + activeEnemy.enemyDamage));

        gm.p.RecieveDamage(activeEnemy.enemyDamage);

        // Player's turn first
        Invoke("BeginPlayerTurn", gm.breatheTime);
    }

    void EnemyChooseSkill()
    {
        currentSelectedSkill = (CombatManager.EnemySelectedSkill)Random.Range(0, 2);

        // Debug
        StartCoroutine(gm.dm.FlashText("Enemy selected skill is " + currentSelectedSkill));

        // Player's turn first
        Invoke("BeginPlayerTurn", gm.breatheTime);
    }


    void CastSkill()
    {

    }
}
