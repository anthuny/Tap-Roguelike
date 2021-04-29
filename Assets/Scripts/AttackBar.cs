using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttackBar : MonoBehaviour 
{
    // Enums
    [HideInInspector]
    public enum AttackBarState {MOVING, IDLE};
    [HideInInspector]
    public AttackBarState curHitMarkerState;
    private enum HitMarkerVisible { VISIBLE, HIDDEN };
    private HitMarkerVisible curHitMarkerVisibility;

    // Inspector variables
    [Header("Main")]
    [SerializeField] private Gamemode _gamemode; 
    [SerializeField] private CombatManager _combatManager;
    [SerializeField] private GameObject hitMarker;
    [SerializeField] private Button _attackButton;
    [SerializeField] private int _hitMarkerStopMouseCode;

    [Header("Statistics")]
    [SerializeField] private float _speed;
    [Tooltip("Time after player hits, before enemy turn begins")]
    [SerializeField] public float timeTillBarResumes;
    [SerializeField] public float timeTillBarTurnsInvis;
    [SerializeField] private float timeTillHitMarkerRespawn;

    [Header("Declerations")]
    [SerializeField] private List<Transform> _checkPoints = new List<Transform>();
    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();
    [SerializeField] private List<Transform> _hitAreas = new List<Transform>();

    // Public
    [HideInInspector]
    public Collider2D hitMarkerCollider;
    [HideInInspector]
    public HitBar curCollidingHitArea;
    public bool canHit;

    // Private
    private RectTransform _hitMarkerRT;
    private GameObject _hitMarkerVisual;
    private float _step;
    private Vector3 _initialPos;
    private Vector3 _nextPos;

    void Awake()
    {
        InitialLaunch();
    }

    // starting relic turn, pressing fight counts as a relic click fix that
    public void LandHitMarker()
    {      
        // Check if the user performed the land hit marker input
        if (canHit && _combatManager.CheckRelicUIHiderStatus())
        {
            StartCoroutine("BeginHitMarkerStoppingSequence"); // Stop the hit marker
            UpdateIfRelicCanAttack(false);
            _combatManager.activeRelic.DetermineUnitMoveChoice(_combatManager.activeRelic, _combatManager.relicActiveSkill);
        }
    }

    public void UpdateIfRelicCanAttack(bool cond)
    {
        canHit = cond;
    }

    void InitialLaunch()
    {
        _combatManager = FindObjectOfType<CombatManager>();        
        hitMarkerCollider = hitMarker.GetComponent<BoxCollider2D>();
        _hitMarkerVisual = hitMarker.transform.GetChild(0).gameObject;
        _hitMarkerVisual.SetActive(false);
        _hitMarkerRT = hitMarker.GetComponent<RectTransform>();
    }

    public void DisableBarVisuals()
    {
        for (int i = 0; i < _checkPoints.Count; i++)
        {
            _checkPoints[i].GetChild(0).gameObject.SetActive(false);
        }

        for (int x = 0; x < _spawnPoints.Count; x++)
        {
            _spawnPoints[x].GetChild(0).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Find the nearest object from the hit marker 
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public Transform FindNearestObjectFromHitMarker(List<Transform> list)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPosition = _hitMarkerRT.localPosition;

        foreach (RectTransform rt in list)
        {
            float dist = Vector2.Distance(rt.localPosition, currentPosition);

            if (dist < minDist)
            {
                tMin = rt;
                minDist = dist;
            }
        }

        return tMin;
    }

    /// <summary>
    /// Set the HitMarkers Starting And Ending position
    /// </summary>
    /// <returns></returns>
    Vector3 SetHitMarkerStartingAndEndingPos(Transform checkPoint = null)
    {
        // Get a random index from the spawnpoints
        int rand = Random.Range(0, _checkPoints.Count);

        Transform transform;

        // If the destination position for the hit marker is being set, go into this if statement
        if (checkPoint)
        {
            for (int i = 0; i < _checkPoints.Count; i++)
            {
                // Set the starting spawn point to not equal the passed checkpoint
                if (i != _checkPoints.IndexOf(checkPoint))
                {
                    transform = _checkPoints[i];
                    return transform.GetComponent<RectTransform>().localPosition;
                }
            }
        }
        // If the hit marker is looking for it's starting position, for the first time in the game, set it to a random one
        else
        {
            transform = _checkPoints[rand];
            return transform.GetComponent<RectTransform>().localPosition;
        }

        return Vector3.zero;
    }

    private void Update()
    {
        if (FindNearestObjectFromHitMarker(_checkPoints))
        {
            MoveHitMarker();
        }
    }

    /// <summary>
    /// Move's Hit Marker on update
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

            // If the hit marker hasn't reached it's destination, move towards it
            if (_step != 1)
            {
                // Move hit marker position
                _step += _speed * Time.deltaTime;
                _step = Mathf.Clamp(_step, 0, 1);
                Vector3 pos = Vector2.Lerp(_initialPos, _nextPos, _step);
                _hitMarkerRT.localPosition = pos;
            }  
            else if (_step == 1)
            {
                _initialPos = _nextPos;
                _nextPos = SetHitMarkerStartingAndEndingPos(FindNearestObjectFromHitMarker(_checkPoints));
                _step = 0;
            }
        }
    }

    /// <summary>
    /// Stops Hit Marker. Called when the hit marker lands on any hit bar (miss, good, great, perfect)
    /// </summary>
    private void StopHitMarker()
    {
        curHitMarkerState = AttackBar.AttackBarState.IDLE;
    }

    private void ResetAttackBarToDefault()
    {
        curHitMarkerState = AttackBar.AttackBarState.IDLE;
        _initialPos = SetHitMarkerStartingAndEndingPos();
        _hitMarkerRT.localPosition = _initialPos;
        curHitMarkerVisibility = HitMarkerVisible.HIDDEN;
        _nextPos = SetHitMarkerStartingAndEndingPos(FindNearestObjectFromHitMarker(_checkPoints));
        _step = 0;
    }

    /// <summary>
    /// Stop hit marker, 
    /// check to see which hit bar the hit marker landed on, 
    /// turn hit marker invisible,
    /// Make hit marker start again
    /// </summary>
    public IEnumerator BeginHitMarkerStoppingSequence()
    {
        // Stop hit marker
        StopHitMarker();

        // Check to see which hit bar the hit marker hit
        curCollidingHitArea.CheckIfMarkerHit();

        // Make hit marker invisible
        StartCoroutine(ToggleHitMarkerVisibility(false, timeTillBarTurnsInvis));

        yield return new WaitForSeconds(timeTillBarResumes);
    }

    /// <summary>
    /// Reset hit marker to default,
    /// Make hit marker start again
    /// </summary>
    public void BeginHitMarkerStartingSequence()
    {
        // Reset values
        ResetAttackBarToDefault();

        // Resume or start the hit marker's attack bar movement
        curHitMarkerState = AttackBar.AttackBarState.MOVING;
    }

    public IEnumerator ToggleHitMarkerVisibility(bool toggle, float time = 0)
    {
        yield return new WaitForSeconds(time);

        _hitMarkerVisual.SetActive(toggle);

        if (toggle)
        {
            curHitMarkerVisibility = HitMarkerVisible.VISIBLE;
        }
        else
        {
            curHitMarkerVisibility = HitMarkerVisible.HIDDEN;
        }
    }
}
