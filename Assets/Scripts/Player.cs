using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector]
    public Gamemode gm;
    [HideInInspector]
    public CombatManager cm;

    [HideInInspector]
    public int playerMaxHealth, playerCurHealth;

    private void Awake()
    {
        gm = FindObjectOfType<Gamemode>();
        cm = FindObjectOfType<CombatManager>();
    }

    public void Start()
    {
        SetHealthToMax();
    }

    void SetHealthToMax()
    {
        playerCurHealth = playerMaxHealth;
    }

    public void RecieveDamage(int damage)
    {
        if (playerCurHealth > 0)
        {
            playerCurHealth -= damage;

            cm.Invoke("BeginEnemyTurn", gm.postHitTime);
        }

        // Check to see if the player's health equals or is less then 0 health
        if (playerCurHealth <= 0)
        {
            KillPlayer();
        }

    }

    void KillPlayer()
    {
        Debug.Log("Player Died");
    }
}
