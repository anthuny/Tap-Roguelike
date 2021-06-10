﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBar : MonoBehaviour
{
    public enum BarType { MISS, GOOD, GREAT, PERFECT };
    public BarType curBarType;

    private CombatManager _combatManager;

    [SerializeField] private Gamemode _gamemode;
    [SerializeField] private AttackBar _attackBar;
    [SerializeField] private RelicManager _relicManager;

    public DevManager _devManager;

    private Collider2D hitAreaCollider;

    private void Awake()
    {
        hitAreaCollider = GetComponent<Collider2D>();

        _devManager = FindObjectOfType<DevManager>();
        _combatManager = FindObjectOfType<CombatManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_attackBar.hitMarkerTag))
            _attackBar.curCollidingHitArea = this;
    }

    /// <summary>
    /// Check if a hit bar was hit by the hit marker. And determine the rank of damage dealt
    /// </summary>
    public void CheckIfMarkerHit()
    {
        _attackBar.hitCount++;

        Collider2D coll = _attackBar.activeHitMarkerCollider;

        // Check if the hit marker collider is touching any hit bar collider
        if (coll.IsTouching(hitAreaCollider))
        {
            // Determine which type of hit bar this is
            switch (curBarType)
            {
                case BarType.PERFECT:
                    Debug.Log("Perfect Hit");
                    _combatManager.activeSkillValueModifier = _combatManager.activeSkill.perfectValueMultiplier;
                    _combatManager.activeSkillProcModifier = _combatManager.activeSkill.perfectProcMultiplier;
                    break;

                case BarType.GREAT:
                    Debug.Log("Great Hit");
                    _combatManager.activeSkillValueModifier = _combatManager.activeSkill.greatValueMultiplier;
                    _combatManager.activeSkillProcModifier = _combatManager.activeSkill.greatProcMultiplier;
                    break;

                case BarType.GOOD:
                    Debug.Log("Good Hit");
                    _combatManager.activeSkillValueModifier = _combatManager.activeSkill.goodValueMultiplier;
                    _combatManager.activeSkillProcModifier = _combatManager.activeSkill.goodProcMultiplier;
                    break;

                case BarType.MISS:
                    Debug.Log("Miss Hit");
                    _combatManager.activeSkillValueModifier = _combatManager.activeSkill.missValueMultiplier;
                    _combatManager.activeSkillProcModifier = _combatManager.activeSkill.missProcMultiplier;
                    break;
            }

            StartCoroutine(_combatManager.activeRelic.UnitSkillFunctionality(_combatManager.activeSkill));

            _attackBar.UpdateRemainingHitsText(true, -(_combatManager.activeSkill.hitsRequired - _attackBar.hitCount));

            _attackBar.DestroyActiveHitMarker(_combatManager.activeAttackBar.timeTillBarTurnsInvis);

            if (_attackBar.hitCount != _combatManager.activeSkill.hitsRequired)
            {
                _attackBar.SpawnHitMarker(_combatManager.activeSkill);
            }
            else if (_attackBar.hitCount == _combatManager.activeSkill.hitsRequired)
                _attackBar.ResetHitCount();
        }
        else
            Debug.LogWarning("This hit bar isn't being collided with");
    }
}
