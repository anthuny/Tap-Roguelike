using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamemode : MonoBehaviour
{
    [HideInInspector]
    public AttackBar ab;
    [HideInInspector]
    public EnemyManager em;
    [HideInInspector]
    public Player p;

    [Tooltip("The minimum distance the hit marker must be to the nearest checkpoint before the next checkpoint is made the next target")]
    public float minDistCheckPoint;

    public float timeTillBarResumes;
    public float timeTillBarTurnsInvis;

    [Header("Developer Mode")]
    public bool devMode;

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
        if (devMode)
        {
            for (int i = 0; i < ab.checkPoints.Count; i++)
            {
                Debug.Log("Checkpoint " + (i+1) + "'s position = " + ab.checkPoints[i].transform.position);
            }
        }
    }

    public void InitialLaunch()
    {
        em = FindObjectOfType<EnemyManager>();
        p = FindObjectOfType<Player>();

        Screen.fullScreen = true;

        Screen.orientation = ScreenOrientation.Portrait;

        FindActiveAttackBar();

        if (!devMode)
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

            StartCoroutine(em.SpawnEnemy());
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
