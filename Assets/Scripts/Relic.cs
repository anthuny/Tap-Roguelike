using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Relic", menuName = "Relic")]
public class Relic : ScriptableObject
{
    [Header("Main")]
    public new string name;
    public int damage;
    public int maxHealth;
    [HideInInspector]
    public int curHealth;
    public int speed;
    public int turnSpeed;
    public int defence;
    public int durability;
    [Header("Aesthetics")]
    public Color color;
    [Space(1)]
    [Header("Skills")]
    public List<Skill> passiveSkills = new List<Skill>();
    public List<Skill> primarySkills = new List<Skill>();
    public List<Skill> secondarySkills = new List<Skill>();
    public List<Skill> alternateSkills = new List<Skill>();
    public List<Skill> ultimateSkills = new List<Skill>();
}
