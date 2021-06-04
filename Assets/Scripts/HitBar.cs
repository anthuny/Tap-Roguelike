using System.Collections;
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
                    _combatManager.activeSkillValueModifier = _combatManager.relicActiveSkill.perfectValueMultiplier;
                    _combatManager.relicActiveSkillProcModifier = _combatManager.relicActiveSkill.perfectProcMultiplier;
                    break;

                case BarType.GREAT:
                    Debug.Log("Great Hit");
                    _combatManager.activeSkillValueModifier = _combatManager.relicActiveSkill.greatValueMultiplier;
                    _combatManager.relicActiveSkillProcModifier = _combatManager.relicActiveSkill.greatProcMultiplier;
                    break;

                case BarType.GOOD:
                    Debug.Log("Good Hit");
                    _combatManager.activeSkillValueModifier = _combatManager.relicActiveSkill.goodValueMultiplier;
                    _combatManager.relicActiveSkillProcModifier = _combatManager.relicActiveSkill.goodProcMultiplier;
                    break;

                case BarType.MISS:
                    Debug.Log("Miss Hit");
                    _combatManager.activeSkillValueModifier = _combatManager.relicActiveSkill.missValueMultiplier;
                    _combatManager.relicActiveSkillProcModifier = _combatManager.relicActiveSkill.missProcMultiplier;
                    break;
            }

            StartCoroutine(_combatManager.activeRelic.UnitSkillFunctionality(true, _combatManager.relicActiveSkill));

            _attackBar.UpdateRemainingHitsText(true, -(_combatManager.relicActiveSkill.hitsRequired - _attackBar.hitCount));

            _attackBar.DestroyActiveHitMarker(_combatManager.activeAttackBar.timeTillBarTurnsInvis);

            if (_attackBar.hitCount != _combatManager.relicActiveSkill.hitsRequired)
            {
                _attackBar.SpawnHitMarker(_combatManager.relicActiveSkill);
            }
            else if (_attackBar.hitCount == _combatManager.relicActiveSkill.hitsRequired)
                _attackBar.ResetHitCount();
        }
        else
            Debug.LogWarning("This hit bar isn't being collided with");
    }
}
