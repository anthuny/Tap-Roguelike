using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject endTurnGO;

    public  void ToggleButton(GameObject go, bool enabled)
    {
        go.SetActive(enabled);
    }
}
