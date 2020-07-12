using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
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

    private void Awake()
    {
        gm = FindObjectOfType<Gamemode>();
    }
    // Start is called before the first frame update
    void Start()
    {
        activeEnemy = enemy[Random.Range(0, enemy.Length)];
    }

    public IEnumerator SpawnEnemy()
    {
        GameObject go = Instantiate(activeEnemy.enemyVisual, enemySpawnPoint.position, Quaternion.identity);

        yield return new WaitForSeconds(gm.EnemyTimeWaitSpawn);

        // Choose whether the enemy will get their turn first, or the player 
        AttackOrder();
    }

    void AttackOrder()
    {
        int startSuccess = Random.Range(0, 101);
        Debug.Log(startSuccess);
        if (startSuccess < activeEnemy.enemySpeed)
        {
            // Enemy's turn first
            BeginEnemyTurn();
        }
        else
        {
            // Player's turn first
            BeginPlayerTurn();
        }
    }

    void BeginPlayerTurn()
    {
        Debug.Log("Starting Player Turn");

        // Begin player's active hit bar attack
        gm.ab.BeginAttackBarPattern();
    }

    public void BeginEnemyTurn()
    {
        Debug.Log("Enemy's turn");

        // Determine what action the enemy is going to do this turn
        float attackRoll = Random.Range(0, activeEnemy.attackChance);
        float skillRoll = Random.Range(0, activeEnemy.skillChance);

        DetermineEnemyMove(attackRoll, skillRoll);

        switch (currentEnemyState)
        {
            case EnemyState.ATTACKING:

                Attack();

                break;

            case EnemyState.CASTSKILL:

                ChooseSkill();

                break;
        }
    }
    
    void DetermineEnemyMove(float attackRoll, float skillRoll)
    {
        attackRoll /= activeEnemy.attackChance;
        skillRoll /= activeEnemy.skillChance;

        Debug.Log(attackRoll);
        Debug.Log(skillRoll);

        if (attackRoll > skillRoll)
        {
            currentEnemyState = EnemyState.ATTACKING;
        }

        else
        {
            currentEnemyState = EnemyState.CASTSKILL;
        }
    }

    void Attack()
    {
        Debug.Log("damaging player");
        gm.p.RecieveDamage(activeEnemy.enemyDamage);

        BeginPlayerTurn();
    }

    void ChooseSkill()
    {
        currentSelectedSkill = (EnemyManager.EnemySelectedSkill)Random.Range(0, 2);
        Debug.Log("selected skill is " + currentSelectedSkill);

        BeginPlayerTurn();
    }


    void CastSkill()
    {

    }
}
