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
        if (collision.CompareTag("HitMarker"))
        {
            _attackBar.curCollidingHitArea = this;
        }
    }

    /// <summary>
    /// Check if a hit bar was hit by the hit marker. And determine the rank of damage dealt
    /// </summary>
    public void CheckIfMarkerHit()
    {
        // Check if the hit marker collider is touching any hit bar collider
        if (_attackBar.activeHitMarkerCollider.IsTouching(hitAreaCollider))
        {
            // Determine which type of hit bar this is
            switch (curBarType)
            {
                case BarType.PERFECT:

                    //StartCoroutine(_devManager.FlashText("Relic perfect hit")); // Debug
                    Debug.Log("Perfect Hit");

                    _combatManager.relicActiveSkillValueModifier = _combatManager.relicActiveSkill.perfectValueMultiplier;
                    _combatManager.relicActiveSkillProcModifier = _combatManager.relicActiveSkill.perfectProcMultiplier;

                    _combatManager.activeRelic.DetermineUnitMoveChoice(_combatManager.activeRelic, _combatManager.relicActiveSkill);
                    break;

                case BarType.GREAT:

                    //StartCoroutine(_devManager.FlashText("Relic great hit")); // Debug
                    Debug.Log("Great Hit");

                    _combatManager.relicActiveSkillValueModifier = _combatManager.relicActiveSkill.greatValueMultiplier;
                    _combatManager.relicActiveSkillProcModifier = _combatManager.relicActiveSkill.greatProcMultiplier;

                    _combatManager.activeRelic.DetermineUnitMoveChoice(_combatManager.activeRelic, _combatManager.relicActiveSkill);
                    break;

                case BarType.GOOD:

                    //StartCoroutine(_devManager.FlashText("Relic good hit")); // Debug
                    Debug.Log("Good Hit");

                    _combatManager.relicActiveSkillValueModifier = _combatManager.relicActiveSkill.goodValueMultiplier;
                    _combatManager.relicActiveSkillProcModifier = _combatManager.relicActiveSkill.goodProcMultiplier;

                    _combatManager.activeRelic.DetermineUnitMoveChoice(_combatManager.activeRelic, _combatManager.relicActiveSkill);
                    break;

                case BarType.MISS:

                    //StartCoroutine(_devManager.FlashText("Relic missed")); // Debug
                    Debug.Log("Miss Hit");

                    _combatManager.relicActiveSkillValueModifier = _combatManager.relicActiveSkill.missValueMultiplier;
                    _combatManager.relicActiveSkillProcModifier = _combatManager.relicActiveSkill.missProcMultiplier;

                    _combatManager.activeRelic.DetermineUnitMoveChoice(_combatManager.activeRelic, _combatManager.relicActiveSkill);
                    break;
            }
        }
        else
        {
            Debug.LogWarning("This hit bar isn't being collided with");
        }
    }


}
