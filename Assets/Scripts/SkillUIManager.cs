using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUIManager : MonoBehaviour
{
    private CombatManager _combatManager;

    [SerializeField] private SkillData relicPassiveSkill;
    [SerializeField] private SkillData relicBasicSkill;
    [SerializeField] private SkillData relicPrimarySkill;
    [SerializeField] private SkillData relicSecondarySkill;
    [SerializeField] private SkillData relicAlternateSkill;
    [SerializeField] private SkillData relicUltimateSkill;


    private void Awake()
    {
        _combatManager = FindObjectOfType<CombatManager>();
    }

    public void AssignRelicSkills()
    {
        relicPassiveSkill.InitializeSkill(
            _combatManager.startingRelic.passiveSkill.skillType, 
            _combatManager.startingRelic.passiveSkill.skillMode,
            _combatManager.startingRelic.passiveSkill.targetType,
            _combatManager.startingRelic.passiveSkill.attackSequence, 
            _combatManager.startingRelic.passiveSkill.targetsAllowed,
            _combatManager.startingRelic.passiveSkill.inflictType, 
            _combatManager.startingRelic.passiveSkill.targetCount,
            _combatManager.startingRelic.passiveSkill.name, 
            _combatManager.startingRelic.passiveSkill.description,
            _combatManager.startingRelic.passiveSkill.turnCooldown, 
            _combatManager.startingRelic.passiveSkill.damage,
            _combatManager.startingRelic.passiveSkill.missDamageMultiplier, 
            _combatManager.startingRelic.passiveSkill.goodDamageMultiplier,
            _combatManager.startingRelic.passiveSkill.greatDamageMultiplier, 
            _combatManager.startingRelic.passiveSkill.perfectDamageMultiplier,
            _combatManager.startingRelic.passiveSkill.procChance, 
            _combatManager.startingRelic.passiveSkill.missProcMultiplier,
            _combatManager.startingRelic.passiveSkill.goodProcMultiplier, 
            _combatManager.startingRelic.passiveSkill.greatProcMultiplier,
            _combatManager.startingRelic.passiveSkill.perfectProcMultiplier);

        relicBasicSkill.InitializeSkill(
            _combatManager.startingRelic.basicSkill.skillType,
            _combatManager.startingRelic.basicSkill.skillMode,
            _combatManager.startingRelic.basicSkill.targetType,
            _combatManager.startingRelic.basicSkill.attackSequence,
            _combatManager.startingRelic.basicSkill.targetsAllowed,
            _combatManager.startingRelic.basicSkill.inflictType,
            _combatManager.startingRelic.basicSkill.targetCount,
            _combatManager.startingRelic.basicSkill.name,
            _combatManager.startingRelic.basicSkill.description,
            _combatManager.startingRelic.basicSkill.turnCooldown,
            _combatManager.startingRelic.basicSkill.damage,
            _combatManager.startingRelic.basicSkill.missDamageMultiplier,
            _combatManager.startingRelic.basicSkill.goodDamageMultiplier,
            _combatManager.startingRelic.basicSkill.greatDamageMultiplier,
            _combatManager.startingRelic.basicSkill.perfectDamageMultiplier,
            _combatManager.startingRelic.basicSkill.procChance,
            _combatManager.startingRelic.basicSkill.missProcMultiplier,
            _combatManager.startingRelic.basicSkill.goodProcMultiplier,
            _combatManager.startingRelic.basicSkill.greatProcMultiplier,
            _combatManager.startingRelic.basicSkill.perfectProcMultiplier);

        relicPrimarySkill.InitializeSkill(
            _combatManager.startingRelic.primarySkill.skillType,
            _combatManager.startingRelic.primarySkill.skillMode,
            _combatManager.startingRelic.primarySkill.targetType,
            _combatManager.startingRelic.primarySkill.attackSequence,
            _combatManager.startingRelic.primarySkill.targetsAllowed,
            _combatManager.startingRelic.primarySkill.inflictType,
            _combatManager.startingRelic.primarySkill.targetCount,
            _combatManager.startingRelic.primarySkill.name,
            _combatManager.startingRelic.primarySkill.description,
            _combatManager.startingRelic.primarySkill.turnCooldown,
            _combatManager.startingRelic.primarySkill.damage,
            _combatManager.startingRelic.primarySkill.missDamageMultiplier,
            _combatManager.startingRelic.primarySkill.goodDamageMultiplier,
            _combatManager.startingRelic.primarySkill.greatDamageMultiplier,
            _combatManager.startingRelic.primarySkill.perfectDamageMultiplier,
            _combatManager.startingRelic.primarySkill.procChance,
            _combatManager.startingRelic.primarySkill.missProcMultiplier,
            _combatManager.startingRelic.primarySkill.goodProcMultiplier,
            _combatManager.startingRelic.primarySkill.greatProcMultiplier,
            _combatManager.startingRelic.primarySkill.perfectProcMultiplier);

        relicSecondarySkill.InitializeSkill(
            _combatManager.startingRelic.secondarySkill.skillType,
            _combatManager.startingRelic.secondarySkill.skillMode,
            _combatManager.startingRelic.secondarySkill.targetType,
            _combatManager.startingRelic.secondarySkill.attackSequence,
            _combatManager.startingRelic.secondarySkill.targetsAllowed,
            _combatManager.startingRelic.secondarySkill.inflictType,
            _combatManager.startingRelic.secondarySkill.targetCount,
            _combatManager.startingRelic.secondarySkill.name,
            _combatManager.startingRelic.secondarySkill.description,
            _combatManager.startingRelic.secondarySkill.turnCooldown,
            _combatManager.startingRelic.secondarySkill.damage,
            _combatManager.startingRelic.secondarySkill.missDamageMultiplier,
            _combatManager.startingRelic.secondarySkill.goodDamageMultiplier,
            _combatManager.startingRelic.secondarySkill.greatDamageMultiplier,
            _combatManager.startingRelic.secondarySkill.perfectDamageMultiplier,
            _combatManager.startingRelic.secondarySkill.procChance,
            _combatManager.startingRelic.secondarySkill.missProcMultiplier,
            _combatManager.startingRelic.secondarySkill.goodProcMultiplier,
            _combatManager.startingRelic.secondarySkill.greatProcMultiplier,
            _combatManager.startingRelic.secondarySkill.perfectProcMultiplier);

        relicAlternateSkill.InitializeSkill(
            _combatManager.startingRelic.alternateSkill.skillType,
            _combatManager.startingRelic.alternateSkill.skillMode,
            _combatManager.startingRelic.alternateSkill.targetType,
            _combatManager.startingRelic.alternateSkill.attackSequence,
            _combatManager.startingRelic.alternateSkill.targetsAllowed,
            _combatManager.startingRelic.alternateSkill.inflictType,
            _combatManager.startingRelic.alternateSkill.targetCount,
            _combatManager.startingRelic.alternateSkill.name,
            _combatManager.startingRelic.alternateSkill.description,
            _combatManager.startingRelic.alternateSkill.turnCooldown,
            _combatManager.startingRelic.alternateSkill.damage,
            _combatManager.startingRelic.alternateSkill.missDamageMultiplier,
            _combatManager.startingRelic.alternateSkill.goodDamageMultiplier,
            _combatManager.startingRelic.alternateSkill.greatDamageMultiplier,
            _combatManager.startingRelic.alternateSkill.perfectDamageMultiplier,
            _combatManager.startingRelic.alternateSkill.procChance,
            _combatManager.startingRelic.alternateSkill.missProcMultiplier,
            _combatManager.startingRelic.alternateSkill.goodProcMultiplier,
            _combatManager.startingRelic.alternateSkill.greatProcMultiplier,
            _combatManager.startingRelic.alternateSkill.perfectProcMultiplier);

        relicUltimateSkill.InitializeSkill(
            _combatManager.startingRelic.ultimateSkill.skillType,
            _combatManager.startingRelic.ultimateSkill.skillMode,
            _combatManager.startingRelic.ultimateSkill.targetType,
            _combatManager.startingRelic.ultimateSkill.attackSequence,
            _combatManager.startingRelic.ultimateSkill.targetsAllowed,
            _combatManager.startingRelic.ultimateSkill.inflictType,
            _combatManager.startingRelic.ultimateSkill.targetCount,
            _combatManager.startingRelic.ultimateSkill.name,
            _combatManager.startingRelic.ultimateSkill.description,
            _combatManager.startingRelic.ultimateSkill.turnCooldown,
            _combatManager.startingRelic.ultimateSkill.damage,
            _combatManager.startingRelic.ultimateSkill.missDamageMultiplier,
            _combatManager.startingRelic.ultimateSkill.goodDamageMultiplier,
            _combatManager.startingRelic.ultimateSkill.greatDamageMultiplier,
            _combatManager.startingRelic.ultimateSkill.perfectDamageMultiplier,
            _combatManager.startingRelic.ultimateSkill.procChance,
            _combatManager.startingRelic.ultimateSkill.missProcMultiplier,
            _combatManager.startingRelic.ultimateSkill.goodProcMultiplier,
            _combatManager.startingRelic.ultimateSkill.greatProcMultiplier,
            _combatManager.startingRelic.ultimateSkill.perfectProcMultiplier);
    }

    public void AssignActiveSkill(SkillData activeSkill)
    {
        _combatManager.relicActiveSkill = activeSkill;
    }




}

