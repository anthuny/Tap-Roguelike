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

    [Tooltip("The default amount of hit damage the enemy currently has")]
    public int damage;

    [Tooltip("The percentage rate of whether the enemy will attack before the player")]
    public int speed;

    [Header("The liklihood of what the enemy will do on their turn")]
    public int attackChance;
    public int skillChance;

    [Header("Skills")]
    public List<Skill> passiveSkills = new List<Skill>();
    public List<Skill> primarySkills = new List<Skill>();
    public List<Skill> secondarySkills = new List<Skill>();
    public List<Skill> alternateSkills = new List<Skill>();
    public List<Skill> ultimateSkills = new List<Skill>();
}
