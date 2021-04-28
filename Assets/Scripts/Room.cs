using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Room", menuName = "Room")]
public class Room : ScriptableObject
{
    [Header("Main")]
    public string roomName;
    public List<Enemy> roomEnemies = new List<Enemy>();
    public List<GameObject> roomEnemyGO = new List<GameObject>();
    public int roomMaxEnemies;

}
