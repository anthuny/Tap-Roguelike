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

    public Transform nearestCheckPoint;
    private Transform oldNearestCheckPoint;

    private float nearestCheckPointDist;

    private int index;

    public Vector2 initialPos;
    public Vector2 nextPos;

    public bool incCheckPoints;



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

        hitMarker.transform.localPosition = spawnPoints[rand].transform.localPosition;

        Debug.Log("rand = " + rand);
        Debug.Log("Spawnpoints count = " + spawnPoints.Count);
        Debug.Log("Chosen spawnpoint name + " + spawnPoints[rand].name);
    }

    private void Update()
    {
        ChooseNextCheckPoint();

        if (nearestCheckPoint)
        {
            FindDistanceToNearestCheckPoint();
        }
    }

    public void FindNearestCheckPoint()
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPosition = hitMarker.transform.localPosition;
        foreach (Transform t in checkPoints)
        {
            float dist = Vector2.Distance(t.position, currentPosition);

            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }

            if (nearestCheckPoint)
            {
                oldNearestCheckPoint = nearestCheckPoint;
            }
        }

            nearestCheckPoint = tMin;

        if (nearestCheckPoint.transform.localPosition.x > hitMarker.transform.localPosition.x)
        {
            incCheckPoints = true;
        }
        else
        {
            incCheckPoints = false;
        }

        Debug.Log("incCheckPoints " + incCheckPoints);
        Debug.Log("nearestTarget " + nearestCheckPoint);
        Debug.Log("nearestTargetDist " + nearestCheckPointDist);
    }

    float FindDistanceToNearestCheckPoint()
    {
        return Vector2.Distance(nearestCheckPoint.transform.localPosition, hitMarker.transform.localPosition);
    }

    void SetMoveTowardValues()
    {
        // At the start of this function, set the values for starting and ending positions for the hit marker's path

        initialPos = hitMarker.transform.localPosition;

        Debug.Log("5");

        if (nextPos == new Vector2(0,0))
        {
            Debug.Log("next pos = " + nextPos);
            nextPos = nearestCheckPoint.transform.localPosition;
        }
    }

    void ChooseNextCheckPoint()
    {
        if (checkPoints.Count <= 1)
        {
            Debug.LogWarning("ERROR - There needs to be more then one checkpoint in this attack bar");
            return;
        }

        if (nearestCheckPoint)
        {
            nearestCheckPointDist = FindDistanceToNearestCheckPoint();
            Debug.Log(FindDistanceToNearestCheckPoint() + " " + gm.minDistCheckPoint);
        }

        // ChooseNextCheckPoint
        if (nearestCheckPointDist <= gm.minDistCheckPoint)
        {
            if (!nearestCheckPoint)
            {
                return;
            }

            else
            {
                index = checkPoints.IndexOf(nearestCheckPoint);

                Debug.Log(index);
            }

            SetMoveTowardValues();
            // If there is another checkpoint in the list, increasing the index of the list, 
            // then set the next checkpoint to the nextPos for the hit marker

            switch (incCheckPoints)
            {
                case true:

                    // set the nextPos to the next checkpoint in the list, incrementally
                    if (checkPoints[index + 1])
                    {
                        nextPos = checkPoints[index + 1].transform.localPosition;
                        Debug.Log("1");
                        return;
                    }

                    // If the end of the list is reached, set the nextPos to the checkpoint previous to the current one
                    else if (index == checkPoints.Count - 1)
                    {
                        incCheckPoints = false;
                        Debug.Log("2");
                        nextPos = checkPoints[index - 1].transform.localPosition;
                    }

                    break;

                case false:

                    // set the nextPos to the next checkpoint in the list, decrementally
                    if (checkPoints[index - 1])
                    {
                        nextPos = checkPoints[index - 1].transform.localPosition;
                        Debug.Log("3");
                        return;
                    }

                    // If the start of the list is reached, set the nextPos to the checkpoint next to the current one
                    else if (index == 0)
                    {
                        Debug.Log("4");
                        incCheckPoints = true;

                        nextPos = checkPoints[index + 1].transform.localPosition;
                    }
                    break;
            }

        }

        float step = speed * Time.deltaTime;
        hitMarker.transform.localPosition = Vector2.MoveTowards(initialPos, nextPos, step);
    }
}
