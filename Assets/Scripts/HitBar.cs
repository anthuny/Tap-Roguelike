using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBar : MonoBehaviour
{
    public enum BarType { MISS, GOOD, GREAT, PERFECT };

    public BarType curBarType;

    [SerializeField] private Gamemode _gamemode;
    [SerializeField] private AttackBar _attackBar;
    [SerializeField] private RelicManager _relicManager;

    public DevManager _devManager;

    private Collider2D hitAreaCollider;

    private void Awake()
    {
        hitAreaCollider = GetComponent<Collider2D>();

        _devManager = FindObjectOfType<DevManager>();
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
        if (_attackBar.hitMarkerCollider.IsTouching(hitAreaCollider))
        {
            // Determine which type of hit bar this is
            switch (curBarType)
            {
                case BarType.PERFECT:
                    // Debug
                    StartCoroutine(_devManager.FlashText("Relic perfect hit"));
                    // Cause Damage to relic
                    _relicManager.RecieveDamage(3);
                    break;

                case BarType.GREAT:
                    // Debug
                    StartCoroutine(_devManager.FlashText("Relic great hit"));
                    // Cause Damage to relic
                    _relicManager.RecieveDamage(2);
                    break;

                case BarType.GOOD:
                    // Debug
                    StartCoroutine(_devManager.FlashText("Relic good hit"));
                    // Cause Damage to relic
                    _relicManager.RecieveDamage(1);
                    break;

                case BarType.MISS:
                    // Debug
                    StartCoroutine(_devManager.FlashText("Relic missed"));
                    // Cause Damage to relic
                    _relicManager.RecieveDamage(0);
                    break;
            }
        }
        else
        {
            Debug.LogWarning("This hit bar isn't being collided with");
        }
    }


}
