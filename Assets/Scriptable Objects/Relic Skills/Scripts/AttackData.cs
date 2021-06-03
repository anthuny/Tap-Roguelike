using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackData : MonoBehaviour
{
    public SkillData skillData;
    public Unit target;
    public float val;
    public float relicActiveSkillValueModifier;
    public float effectVal;
    public bool inCombat;
    public Effect effect;
    public string skillName;
    public string skillEffectName;
    public int effectPower;
    public int effectDuration;
    public int curHitsCompleted;
    public int hitsRequired;
    public float timeBetweenHitUI;
    public float timeTillEffectInflict;
    public Transform skillUIValueParent;
    public int curTargetCount;
    public int hitTarget;
}
