using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBar : MonoBehaviour
{
    // Enums

    [HideInInspector]
    public enum AttackBarState {MOVING, IDLE};
    [HideInInspector]
    public AttackBarState currentHitMarkerState;
 
    Gamemode gm;


    // Public

    [Header("Statistics")]
    public float speed; 

    [Header("Declerations")]
    public List<Transform> checkPoints = new List<Transform>();
    public List<Transform> spawnPoints = new List<Transform>();
    public List<Transform> accuracyBars = new List<Transform>();

    public GameObject hitMarker;

    public Transform nextCheckPoint;

    public float nearestCheckPointDist;

    public int index;

    public Vector2 initialPos;
    public Vector2 nextPos;

    public bool incCheckPoints;
    public bool reachedTarget;


    // Private

    private bool movedToFirstCheckPoint;

    private GameObject hitMarkerVisual;

    [HideInInspector]
    public float step;
    [HideInInspector]
    public bool hitMarkerStopped;

    // Start is called before the first frame update
    void Awake()
    {
        InitialLaunch();
    }

    void InitialLaunch()
    {
        gm = FindObjectOfType<Gamemode>();
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

    public void BeginAttackBarPattern()
    {
        // Allow each accuracy bar to be able to be hit again
        hitMarkerStopped = false;

        gm.hitMarkerStarted = true;

        // Resume or start the hit marker's attack bar movement
        currentHitMarkerState = AttackBar.AttackBarState.MOVING;

        PickSpawnPoint();   

        FindNearestCheckPoint();

        SetMoveTowardValues();

        ChooseNextCheckPoint();
    }

    void PickSpawnPoint()
    {
        int rand = Random.Range(0, spawnPoints.Count);

        hitMarker.transform.position = spawnPoints[rand].transform.position;
    }

    private void Update()
    {
        if (nextCheckPoint)
        {
            FindDistanceToNearestCheckPoint();
            ChooseNextCheckPoint();
        }
    }

    public void FindNearestCheckPoint()
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPosition = hitMarker.transform.position;

        foreach (Transform t in checkPoints)
        {
            float dist = Vector2.Distance(t.position, currentPosition);

            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }

        nextCheckPoint = tMin;

        index = checkPoints.IndexOf(nextCheckPoint);

        if (nextCheckPoint.transform.position.x > hitMarker.transform.position.x)
        {
            incCheckPoints = true;
        }
        else
        {
            incCheckPoints = false;
        }

        //Debug.Log("incCheckPoints " + incCheckPoints);
        //Debug.Log("nearestTarget " + nearestCheckPoint);
        //Debug.Log("nearestTargetDist " + nearestCheckPointDist);
    }

    Transform FindNextCheckPoint()
    {
        return checkPoints[index];
    }

    float FindDistanceToNextCheckPoint()
    {
        return Vector2.Distance(checkPoints[index].transform.position, hitMarker.transform.position);
    }

    float FindDistanceToNearestCheckPoint()
    {
        return Vector2.Distance(nextCheckPoint.transform.position, hitMarker.transform.position);
    }

    void ChooseNextCheckPoint()
    {
        if (checkPoints.Count <= 1)
        {
            Debug.LogWarning("ERROR - There needs to be more then one checkpoint in this attack bar");
            return;
        }

        if (nextCheckPoint && !movedToFirstCheckPoint)
        {
            nearestCheckPointDist = FindDistanceToNearestCheckPoint();
        }

        if (nextCheckPoint && movedToFirstCheckPoint)
        {
            nextCheckPoint = FindNextCheckPoint();
            nearestCheckPointDist = FindDistanceToNextCheckPoint();
        }

        // ChooseNextCheckPoint
        if (nearestCheckPointDist <= gm.minDistCheckPoint
            && !reachedTarget)
        {
            reachedTarget = true;

            if (!movedToFirstCheckPoint)
            {
                movedToFirstCheckPoint = true;
            }

            step = 0;

            if (!nextCheckPoint)
            {
                return;
            }

            SetMoveTowardValues();
            // If there is another checkpoint in the list, increasing the index of the list, 
            // then set the next checkpoint to the nextPos for the hit marker

            switch (incCheckPoints)
            {
                case true:

                    // If the end of the list is reached, set the nextPos to the checkpoint previous to the current one
                    if (index == checkPoints.Count - 1)
                    {
                        incCheckPoints = false;

                        index = checkPoints.IndexOf(nextCheckPoint) - 1;
                        nextPos = checkPoints[index].transform.position;
                    }
                    // set the nextPos to the next checkpoint in the list, incrementally
                    else if (checkPoints[index + 1])
                    {
                        index = checkPoints.IndexOf(nextCheckPoint) + 1;
                        nextPos = checkPoints[index].transform.position;
                        return;
                    }

                    break;

                case false:

                     // If the start of the list is reached, set the nextPos to the checkpoint next to the current one
                    if (index == 0)
                    {
                        incCheckPoints = true;

                        index = checkPoints.IndexOf(nextCheckPoint) + 1;

                        nextPos = checkPoints[index].transform.position;
                    }
                    // set the nextPos to the next checkpoint in the list, decrementally
                    else if (checkPoints[index - 1])
                    {
                        index = checkPoints.IndexOf(nextCheckPoint) - 1;
                        nextPos = checkPoints[index].transform.position;
                        return;
                    }

                    break;
            }

        }


        if (nearestCheckPointDist > gm.minDistCheckPoint)
        {
            reachedTarget = false;
        }

        if (initialPos != new Vector2(0,0) && currentHitMarkerState == AttackBarState.MOVING)
        {
            // If the hit marker is invisible, make it visible before it starts moving
            if (!hitMarker.transform.GetChild(0).gameObject.activeSelf)
            {
                hitMarker.transform.GetChild(0).gameObject.SetActive(true);
            }

            step += speed * Time.deltaTime;
            Vector3 pos = Vector2.MoveTowards(initialPos, nextPos, step);

            hitMarker.transform.position = pos;
            //Debug.Log("Step " + step);
            //Debug.Log("initialPos " + initialPos);
        }

    }

    void SetMoveTowardValues()
    {
        // At the start of this function, set the values for starting and ending positions for the hit marker's path
        initialPos = hitMarker.transform.position;

        if (nextPos == new Vector2(0, 0))
        {
            //Debug.Log("next pos = " + nextPos);
            nextPos = nextCheckPoint.transform.position;
        }
    }

    public void StopHitMarker()
    {
        gm.ab.currentHitMarkerState = AttackBar.AttackBarState.IDLE;

        nextCheckPoint = null;
        nearestCheckPointDist = 0;
        index = 0;
        initialPos = Vector2.zero;
        nextPos = Vector2.zero;
        incCheckPoints = false;
        reachedTarget = false;
        movedToFirstCheckPoint = false;
        step = 0;

        StartCoroutine(MakeHitMarkerInvisible(gm.timeTillBarTurnsInvis));
    }

    public IEnumerator MakeHitMarkerInvisible(float time = 0)
    {
        yield return new WaitForSeconds(time);

        hitMarkerVisual.SetActive(false);

        StartCoroutine(ResumeAttackBar(gm.timeTillBarResumes));
    }
    public IEnumerator ResumeAttackBar(float time = 0)
    {
        yield return new WaitForSeconds(time);

        StartCoroutine(gm.cm.DetermineEnemyMove(gm.breatheTime));
    }
}
