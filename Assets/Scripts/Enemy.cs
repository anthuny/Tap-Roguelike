using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class Enemy : ScriptableObject
{
    public string enemyName;
    public int enemyLevel = 1;

    [Header("Statistics")]
    [Tooltip("The default maximum health the enemy spawns with")]
    public float enemyMaxHealth;
    [HideInInspector]
    public float enemyCurHealth;

    [Tooltip("The default amount of hit damage the enemy currently has")]
    public int enemyDamage;

    [Tooltip("The percentage rate of whether the enemy will attack before the player")]
    public float enemySpeed;

    [Tooltip("Tooltip WIP")]
    public float enemyDefence;

    public GameObject enemyVisual;

    [Header("The liklihood of what the enemy will do on their turn")]
    public int attackChance;
    public int skillChance;
}
