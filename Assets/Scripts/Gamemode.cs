using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamemode : MonoBehaviour
{
    [HideInInspector]
    public AttackBar ab;
    [HideInInspector]
    public CombatManager cm;
    [HideInInspector]
    public Player p;
    [HideInInspector]
    public DevManager dm;

    [Tooltip("The minimum distance the hit marker must be to the nearest checkpoint before the next checkpoint is made the next target")]
    public float minDistCheckPoint;

    [Header("Attack Bar Settings")]
    public float missDmgMult;
    public float goodDmgMult;
    public float greatDmgMult;
    public float perfectDmgMult;
    public float timeTillBarResumes;
    public float timeTillBarTurnsInvis;

    [Header("Player Settings")]
    public float breatheTime = 2;

    [Header("Developer Mode")]
    public GameObject devTextParent;
    public bool devVisualsOn;
    public bool devTextOn;

    [HideInInspector]
    public bool attackDetected, hitMarkerStarted;

    [Tooltip("The amount of time in seconds that must elapse after the enemy spawns before the next stage can begin")]
    public float EnemyTimeWaitSpawn;

    [Tooltip("The amount of time in seconds that must elapse after the player is hit, for the next stage can begin")]
    public float postHitTime;

    // Start is called before the first frame update
    void Start()
    {
        InitialLaunch();
        FindCheckPointPositions();
    }

    void FindCheckPointPositions()
    {
        /*
        if (devMode)
        {
            for (int i = 0; i < ab.checkPoints.Count; i++)
            {
                Debug.Log("Checkpoint " + (i+1) + "'s position = " + ab.checkPoints[i].transform.position);
            }
        }
        */
    }

    public void InitialLaunch()
    {
        cm = FindObjectOfType<CombatManager>();
        p = FindObjectOfType<Player>();
        dm = FindObjectOfType<DevManager>();

        Screen.fullScreen = true;

        Screen.orientation = ScreenOrientation.Portrait;

        FindActiveAttackBar();

        if (!devVisualsOn)
        {
            ab.DisableBarVisuals();
        }
    }

    public void FindActiveAttackBar()
    {
        ab = FindObjectOfType<AttackBar>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            // Begin attack bar pattern
            // Determine whether enemy or player attacks first
            StartCoroutine(cm.EnemySpawn());
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
