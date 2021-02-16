using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicManager : MonoBehaviour
{
    [SerializeField] private Gamemode _gamemode;
    [SerializeField] private CombatManager _combatManager;

    public int maxHealth, curHealth;

    public void Start()
    {
        SetHealthToMax();
    }

    void SetHealthToMax()
    {
        curHealth = maxHealth;
    }

    public void RecieveDamage(int damage)
    {
        if (curHealth > 0)
        {
            curHealth -= damage;
        }

        // Check to see if the player's health equals or is less then 0 health
        if (curHealth <= 0)
        {
            KillPlayer();
        }

    }

    void KillPlayer()
    {
        Debug.Log("Player Died");
    }
}
