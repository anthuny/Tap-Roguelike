using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBar : MonoBehaviour
{
    // Enums
    [HideInInspector]
    public enum AttackBarState {MOVING, IDLE};
    //[HideInInspector]
    public AttackBarState curHitMarkerState;

    private enum HitMarkerVisible {VISIBLE, HIDDEN};
    private HitMarkerVisible curHitMarkerVisibility;

    // Public
    [Header("Statistics")]
    public float speed; 

    [Header("Declerations")]
    [SerializeField] private List<Transform> checkPoints = new List<Transform>();
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private List<Transform> hitAreas = new List<Transform>();
    public GameObject hitMarker;

    // Private
    private int selectedCheckpointSpawnIndex;


    public float step;

    private Gamemode gm;

    private GameObject hitMarkerVisual;

    private Transform landingCheckpoint;
    public Transform nearestHitArea;

    public Vector2 initialPos;
    public Vector2 nextPos;

    //[HideInInspector]
    public Collider2D hitMarkerCollider, nearestHitAreaCollider;
    private Collider2D prevNearestHitAreaCollider;

    // Start is called before the first frame update
    void Awake()
    {
        InitialLaunch();
    }

    void InitialLaunch()
    {
        gm = FindObjectOfType<Gamemode>();

        hitMarkerCollider = hitMarker.GetComponent<BoxCollider2D>();

        hitMarkerVisual = hitMarker.transform.GetChild(0).gameObject;
        hitMarkerVisual.SetActive(false);      
    }

    public void DisableBarVisuals()
    {
        for (int i = 0; i < checkPoints.Count; i++)
        {
            checkPoints[i].GetChild(0).gameObject.SetActive(false);
        }

        for (int x = 0; x < spawnPoints.Count; x++)
        {
            spawnPoints[x].GetChild(0).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Called from Combat Manager script
    /// Begins the Hit Bar movement
    /// </summary>
    public void BeginAttackBarPattern()
    {
        // Reset values

        Refresh();

        // Resume or start the hit marker's attack bar movement
        curHitMarkerState = AttackBar.AttackBarState.MOVING;

        // Set the spawn location for the hit marker
        hitMarker.transform.position = SetHitMarkerRandomSpawnLocation();

        nextPos = SetHitMarkerDestination();
    }

    /// <summary>
    /// Fidn the nearest object from the hit marker 
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public Transform FindNearestObjectFromHitMarker(List<Transform> list)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPosition = hitMarker.transform.position;

        foreach (Transform t in list)
        {
            float dist = Vector2.Distance(t.position, currentPosition);

            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }

        return tMin;
    }

    /// <summary>
    /// Set the Hit Marker's spawning location to a random checkpoint
    /// </summary>
    /// <returns></returns>
    Vector3 SetHitMarkerRandomSpawnLocation()
    {
        int rand = Random.Range(0, spawnPoints.Count);

        selectedCheckpointSpawnIndex = rand;

        //Debug.Log(rand + " " + spawnPoints[rand]);

        return spawnPoints[rand].transform.position;
    }

    private void Update()
    {
        if (FindNearestObjectFromHitMarker(checkPoints))
        {
            MoveHitMarker();

            #region Obtaining reference to each hit area's hitbox for hit marker
            nearestHitArea = FindNearestObjectFromHitMarker(hitAreas);
            FindNearestHitAreaCollider();
            #endregion
        }
    }

    /// <summary>
    /// Update what the nearest hit area collider is
    /// </summary>
    void FindNearestHitAreaCollider()
    {
        if (nearestHitArea)
        {
            nearestHitAreaCollider = nearestHitArea.GetComponent<BoxCollider2D>();

            // Only update the nearest hit area collider when there is a change in colliders
            if (prevNearestHitAreaCollider != nearestHitAreaCollider)
            {
                prevNearestHitAreaCollider = nearestHitAreaCollider;
                nearestHitAreaCollider = nearestHitArea.GetComponent<BoxCollider2D>();
            }
        }
    }

    /// <summary>
    /// Set Hit Marker positions before moving
    /// </summary>
    Vector2 SetHitMarkerDestination()
    {
        for (int i = 0; i < checkPoints.Count; i++)
        {
            if (i != selectedCheckpointSpawnIndex)
            {
                return checkPoints[i].transform.position;
            }
        }

        return Vector2.zero;
    }

    /// <summary>
    /// Move's Hit Marker
    /// </summary>
    void MoveHitMarker()
    {
        // Only continue if the hit marker is allowed to move
        if (curHitMarkerState == AttackBarState.MOVING)
        {
            // If hit marker is invisible, make it visible
            if (curHitMarkerVisibility == HitMarkerVisible.HIDDEN)
            {
                StartCoroutine(ToggleHitMarkerVisibility(true, 0f));
            }

            // Move hit marker position
            step += speed * Time.deltaTime;
            Vector3 pos = Vector2.MoveTowards(initialPos, nextPos, step);
            hitMarker.transform.position = pos;

            // If the hit marker reaches a checkpoint, set the next move position to the other checkpoint
            if (pos.x == nextPos.x)
            {
                // Assign the finishing checkpoint, so that the other checkpoint can be distinguished as 
                // the new target checkpoint
                landingCheckpoint = FindNearestObjectFromHitMarker(checkPoints);

                for (int i = 0; i < checkPoints.Count; i++)
                {
                    if (i != checkPoints.IndexOf(landingCheckpoint))
                    {
                        initialPos = nextPos;
                        nextPos = checkPoints[i].transform.position;
                        step = 0;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Stops Hit Marker. Called when the hit marker lands on any hit bar (miss, good, great, perfect)
    /// </summary>
    public void StopHitMarker()
    {
        gm.ab.curHitMarkerState = AttackBar.AttackBarState.IDLE;

        Refresh();

        StartCoroutine(ToggleHitMarkerVisibility(false, gm.timeTillBarTurnsInvis));
    }

    private void Refresh()
    {
        gm.ab.curHitMarkerState = AttackBar.AttackBarState.IDLE;
        initialPos = Vector2.zero;
        nextPos = Vector2.zero;
        curHitMarkerVisibility = HitMarkerVisible.HIDDEN;
        selectedCheckpointSpawnIndex = 3;
        initialPos = hitMarker.transform.position;
        nextPos = Vector2.zero;
        step = 0;
    }

    public IEnumerator ToggleHitMarkerVisibility(bool toggle, float time = 0)
    {
        yield return new WaitForSeconds(time);

        hitMarkerVisual.SetActive(toggle);

        if (toggle)
        {
            curHitMarkerVisibility = HitMarkerVisible.VISIBLE;
        }
        else
        {
            curHitMarkerVisibility = HitMarkerVisible.HIDDEN;
            StartCoroutine(ResumeAttackBar(gm.timeTillBarResumes));
        }
    }
    public IEnumerator ResumeAttackBar(float time = 0)
    {
        yield return new WaitForSeconds(time);
        
        StartCoroutine(gm.cm.DetermineEnemyMove(gm.breatheTime));
    }
}
