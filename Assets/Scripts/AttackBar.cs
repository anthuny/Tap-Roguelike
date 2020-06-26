using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBar : MonoBehaviour
{
    Gamemode gm;

    public GameObject hitMarker;

    [Header("Statistics")]
    public float speed; 

    [Header("Declerations")]
    public List<Transform> checkPoints = new List<Transform>();
    public List<Transform> spawnPoints = new List<Transform>();

    private Transform nearestCheckPoint;
    private Transform oldNearestCheckPoint;
    private float nearestCheckPointDist;

    private Vector2 initialPos;
    private Vector2 nextPos;

    private bool incCheckPoints;

    // Start is called before the first frame update
    void Awake()
    {
        InitialLaunch();
    }

    void InitialLaunch()
    {
        gm = FindObjectOfType<Gamemode>();
    }

    public IEnumerator BeginAttackBarPattern()
    {
        PickSpawnPoint();

        yield return new WaitForSeconds(.2f);

        FindNearestCheckPoint();

        yield return new WaitForSeconds(.2f);

        SetMoveTowardValues();

        yield return new WaitForSeconds(.2f);

        ChooseNextCheckPoint();
    }

    void PickSpawnPoint()
    {
        int rand = Random.Range(0, spawnPoints.Count);

        hitMarker.transform.position = spawnPoints[rand].transform.position;

        Debug.Log("rand = " + rand);
        Debug.Log("Spawnpoints count = " + spawnPoints.Count);
        Debug.Log("Chosen spawnpoint name + " + spawnPoints[rand].name);
    }

    private void Update()
    {
        if (nearestCheckPoint)
        {
            ChooseNextCheckPoint();
        }
    }
    public void FindNearestCheckPoint()
    {
        Transform nearestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = hitMarker.transform.position;
        foreach (Transform potentialTarget in checkPoints)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                nearestTarget = potentialTarget;
            }
        }

        if (nearestCheckPoint)
        {
            oldNearestCheckPoint = nearestCheckPoint;
        }

        nearestCheckPoint = nearestTarget;
        nearestCheckPointDist = closestDistanceSqr;

        if (nearestCheckPoint.transform.position.x > hitMarker.transform.position.x)
        {
            incCheckPoints = true;
        }
        else
        {
            incCheckPoints = false;
        }

    }

    void SetMoveTowardValues()
    {
        // At the start of this function, set the values for starting and ending positions for the hit marker's path
        if (oldNearestCheckPoint != nearestCheckPoint)
        {
            oldNearestCheckPoint = nearestCheckPoint;
            initialPos = hitMarker.transform.position;
        }

        if (nextPos == null)
        {
            nextPos = nearestCheckPoint.transform.position;
        }
    }

    void ChooseNextCheckPoint()
    {
        if (checkPoints.Count <= 1)
        {
            Debug.LogWarning("ERROR - There needs to be more then one checkpoint in this attack bar");
            return;
        }

        // ChooseNextCheckPoint
        if (nearestCheckPointDist <= gm.minDistCheckPoint)
        {
            SetMoveTowardValues();

            int index = checkPoints.IndexOf(nearestCheckPoint);

            // If there is another checkpoint in the list, increasing the index of the list, 
            // then set the next checkpoint to the nextPos for the hit marker

            switch(incCheckPoints)
            {
                case true:

                    // set the nextPos to the next checkpoint in the list, incrementally
                    if (checkPoints[index + 1])
                    {
                        nextPos = checkPoints[index + 1].transform.position;
                        return;
                    }

                    // If the end of the list is reached, set the nextPos to the checkpoint previous to the current one
                    else if (index == checkPoints.Count - 1)
                    {
                        incCheckPoints = false;

                        nextPos = checkPoints[index - 1].transform.position;
                    }

                    break;

                case false:

                    // set the nextPos to the next checkpoint in the list, decrementally
                    if (checkPoints[index - 1])
                    {
                        nextPos = checkPoints[index - 1].transform.position;
                        return;
                    }

                    // If the start of the list is reached, set the nextPos to the checkpoint next to the current one
                    else if (index == 0)
                    {
                        incCheckPoints = true;

                        nextPos = checkPoints[index + 1].transform.position;
                    }
                    break;
            }

        }

        float step = speed * Time.deltaTime;
        hitMarker.transform.position = Vector2.MoveTowards(initialPos, nextPos, step);
    }
}
