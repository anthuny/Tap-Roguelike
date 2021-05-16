using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBarInput : MonoBehaviour
{
    [SerializeField] private AttackBar _attackBar;

    public void StopAttackBarHitMarker()
    {
        
        _attackBar.LandHitMarker();
    }   
}
