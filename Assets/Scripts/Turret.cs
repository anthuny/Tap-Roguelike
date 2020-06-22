using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Turret", menuName = "Turret")]
public class Turret : ScriptableObject
{
    public string turretName;
    public GameObject turretObj;
}
