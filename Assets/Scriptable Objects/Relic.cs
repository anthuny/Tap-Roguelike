using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Relic", menuName = "Relic")]
public class Relic : ScriptableObject
{
    [Header("Main")]
    public new string name;
    public int power;
    public int maxHealth;
    [HideInInspector]
    public int curHealth;
    public int speed;
    public int turnSpeed;
    public int durability;
    [Header("Aesthetics")]
    public Color color;
    [Space(1)]
    [Header("Skills")]
    public Skill passiveSkill;
    public Skill basicSkill;
    public Skill primarySkill;
    public Skill secondarySkill;
    public Skill alternateSkill;
    public Skill ultimateSkill;
}
