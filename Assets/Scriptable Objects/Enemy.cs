using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class Enemy : ScriptableObject
{
    [Header("Main")]
    public new string name;
    public int level = 1;

    [Space(3)]

    [Header("Main Stats")]
    public int power;
    [Tooltip("The default maximum health the enemy spawns with")]
    public float maxHealth;
    [HideInInspector]
    public float curHealth;
    [HideInInspector]
    public int curMana;
    public int maxMana;
    public int manaGainTurn;
    [Tooltip("The percentage rate of whether the enemy will attack before the player")]
    public int energy;

    [Space(3)]

    [Header("Growth Stats")]
    public int powerGrowth;
    public int healthGrowth;
    public int manaGrowth;
    public int maxManaGrowth;

    [Space(3)]

    [Header("The liklihood of what the enemy will do on their turn")]
    public int attackChance;
    public int skillChance;

    [Space(3)]

    [Header("Aesthetics")]
    public Color color;

    [Space(3)]

    [Header("Skills")]
    public Skill passiveSkill;
    public Skill basicSkill;
    public Skill primarySkill;
    public Skill secondarySkill;
    public Skill alternateSkill;
    public Skill ultimateSkill;
}
