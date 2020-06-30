using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy
{
    public string name;

    [Header("Statistics")]

    [Header("Health")]
    [Tooltip("The default maximum health the enemy spawns with")]
    public float maxHealth;
    [Tooltip("The percentage range to alter the max health. (10% would result in 10% increase or decrease of the inputed 'maxHealth'")]
    public float maxHealthRange;
}
