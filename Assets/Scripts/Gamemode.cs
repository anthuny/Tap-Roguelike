using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamemode : MonoBehaviour
{
    [HideInInspector]
    public AttackBar attackBarScript;

    [Tooltip("The minimum distance the hit marker must be to the nearest checkpoint before the next checkpoint is made the next target")]
    public float minDistCheckPoint;

    public float timeTillBarResumes;
    public float timeTillBarTurnsInvis;

    [Header("Developer Mode")]
    public bool devMode;

    [HideInInspector]
    public bool attackDetected;

    // Start is called before the first frame update
    void Start()
    {
        InitialLaunch();
        FindCheckPointPositions();

        attackBarScript.DisableBarVisuals();
    }

    void FindCheckPointPositions()
    {
        if (devMode)
        {
            for (int i = 0; i < attackBarScript.checkPoints.Count; i++)
            {
                Debug.Log("Checkpoint " + (i+1) + "'s position = " + attackBarScript.checkPoints[i].transform.position);
            }
        }

    }
    public void InitialLaunch()
    {
        Screen.fullScreen = true;

        Screen.orientation = ScreenOrientation.Portrait;

        FindActiveAttackBar();
    }

    public void FindActiveAttackBar()
    {
        attackBarScript = FindObjectOfType<AttackBar>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            // Begin attack bar pattern
            attackBarScript.BeginAttackBarPattern();
        }

        DetectInput();
    }

    void DetectInput()
    {
        if (Input.GetMouseButton(0))
        {
            attackDetected = true;
        }
        else
        {
            attackDetected = false;
        }
    }
}
