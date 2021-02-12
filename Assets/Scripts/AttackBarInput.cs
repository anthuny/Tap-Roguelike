using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBarInput : MonoBehaviour
{
    [SerializeField] private int hitMarkerStopMouseCode;

    private AttackBar _attackBar;

    // Start is called before the first frame update
    void Awake()
    {
        InitialLaunch();
    }

    void InitialLaunch()
    {
        _attackBar = GetComponent<AttackBar>();
    }

    // Update is called once per frame
    void Update()
    {
        LandHitMarker(hitMarkerStopMouseCode);
    }

    public void LandHitMarker(int val = 0)
    {
        // Null reference check
        if (_attackBar.nearestHitAreaCollider)
        {
            // Check if hit marker is touching the nearest hit area collider, and
            // Check if the user performed the land hit marker input
            if (_attackBar.hitMarkerCollider.IsTouching(_attackBar.nearestHitAreaCollider) &&
                Input.GetMouseButtonDown(hitMarkerStopMouseCode))
            {
                HitBar _hitBar = _attackBar.nearestHitAreaCollider.gameObject.GetComponent<HitBar>();
                _hitBar.CheckIfMarkerHit();
            }
        }
    }
}
