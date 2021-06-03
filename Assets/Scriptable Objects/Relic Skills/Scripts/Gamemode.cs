using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamemode : MonoBehaviour
{
    [Tooltip("The minimum distance the hit marker must be to the nearest checkpoint before the next checkpoint is made the next target")]
    public float minDistCheckPoint;

    [Header("Attack Bar Settings")]
    public float missDmgMult;
    public float goodDmgMult;
    public float greatDmgMult;
    public float perfectDmgMult;

    // Start is called before the first frame update
    void Awake()
    {
        InitialLaunch();
    }

    public void InitialLaunch()
    {
        Screen.fullScreen = true;

        Screen.orientation = ScreenOrientation.Portrait;
    }
}
