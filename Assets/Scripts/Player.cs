using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector]
    public Gamemode gm;
    [HideInInspector]
    public EnemyManager em;

    [Tooltip("The maximum health the player can have")]
    public int playerMaxHealth;
    [Tooltip("The current health the player current has")]
    public int playerCurHealth;

    private void Awake()
    {
        gm = FindObjectOfType<Gamemode>();
        em = FindObjectOfType<EnemyManager>();
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

            em.Invoke("BeginEnemyTurn", gm.postHitTime);
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
