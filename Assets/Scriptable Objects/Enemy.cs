using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class Enemy : ScriptableObject
{
    public new string name;
    public int level = 1;

    [Header("Aesthetics")]
    public Color color;

    [Header("Statistics")]
    [Tooltip("The default maximum health the enemy spawns with")]
    public float maxHealth;
    [HideInInspector]
    public float curHealth;

    [Tooltip("The default power the unit has")]
    public int power;

    [Tooltip("The percentage rate of whether the enemy will attack before the player")]
    public int speed;

    [Header("The liklihood of what the enemy will do on their turn")]
    public int attackChance;
    public int skillChance;

    [Header("Skills")]
    public Skill passiveSkill;
    public Skill basicSkill;
    public Skill primarySkill;
    public Skill secondarySkill;
    public Skill alternateSkill;
    public Skill ultimateSkill;

    [Header("UI")]
    public Image healthImage;
}
