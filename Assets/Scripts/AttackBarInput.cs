using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBarInput : MonoBehaviour
{
    [SerializeField] private int _hitMarkerStopMouseCode;

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
        LandHitMarker(_hitMarkerStopMouseCode);
    }

    public void LandHitMarker(int val = 0)
    {
        // Check if the user performed the land hit marker input
        if (Input.GetMouseButtonDown(_hitMarkerStopMouseCode) && !_attackBar.attackInputLocked)
        {
            _attackBar.attackInputLocked = true;

            // Stop the hit marker
            _attackBar.BeginHitMarkerStoppingSequence();        
        }
    }
}
