using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Relic", menuName = "Relic")]
public class Relic : ScriptableObject
{
    [Header("Main")]
    public new string name;
    public int level = 1;
    public Sprite portraitSprite;

    [Space(3)]

    [Header("Main Stats")]  
    public int power;
    public int maxHealth;
    [HideInInspector]
    public int curHealth;
    [HideInInspector]
    public int curMana;
    public int maxMana;
    public int manaGainTurn;
    public int energy;

    [Space(3)]
    
    [Header("Growth Stats")]
    public int powerGrowth;
    public int healthGrowth;
    public int energyGrowth;
    public int maxManaGrowth;

    [Space(3)]
    
    [Header("Aesthetics")]
    public Color color;

    [Space(3)]

    public GameObject weapon;

    [Header("Skills")]
    public Skill passiveSkill;
    public Skill basicSkill;
    public Skill primarySkill;
    public Skill secondarySkill;
    public Skill alternateSkill;
    public Skill ultimateSkill;
}
