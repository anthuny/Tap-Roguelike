using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBar : MonoBehaviour
{
    [SerializeField] private Gamemode _gamemode;
    [SerializeField] private DevManager _devManager;
    [SerializeReference] private AttackBar _attackBar;

    public enum BarType { MISS, GOOD, GREAT, PERFECT };

    public BarType curBarType;

    /// <summary>
    /// Check if a hit bar was hit by the hit marker. And determine the rank of damage dealt
    /// </summary>
    public void CheckIfMarkerHit()
    {
        // Determine which type of hit bar this is
        switch (curBarType)
        {
            case BarType.PERFECT:
                // Debug
                StartCoroutine(_devManager.FlashText("Player perfect hit"));
                // Stop hit marker
                _attackBar.StopHitMarker();
                break;

            case BarType.GREAT:
                // Debug
                StartCoroutine(_devManager.FlashText("Player great hit"));
                // Stop hit marker
                _attackBar.StopHitMarker();
                break;

            case BarType.GOOD:
                // Debug
                StartCoroutine(_devManager.FlashText("Player good hit"));
                // Stop hit marker
                _attackBar.StopHitMarker();
                break;

            case BarType.MISS:
                // Debug
                StartCoroutine(_devManager.FlashText("Player missed"));
                // Stop hit marker
                _attackBar.StopHitMarker();
                break;
        }
    }
}
