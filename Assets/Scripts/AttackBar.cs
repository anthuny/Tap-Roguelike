using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackBar : MonoBehaviour 
{
    // Enums
    [HideInInspector]
    public enum AttackBarState {MOVING, IDLE};
    [HideInInspector]
    public AttackBarState curHitMarkerState;

    // Inspector variables
    [Header("Main")]
    [SerializeField] private Gamemode _gamemode; 
    [SerializeField] private CombatManager _combatManager;
    [SerializeField] private GameObject hitMarker;
    [SerializeField] private Button _attackButton;
    [SerializeField] private int _hitMarkerStopMouseCode;
    public float hitMarkerInvisTime;

    [Header("Statistics")]
    [SerializeField] private float _speed;
    [SerializeField] public float timeTillBarTurnsInvis;


    [Header("Declerations")]
    public List<Transform> _checkPoints = new List<Transform>();
    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();
    [SerializeField] private List<Transform> _hitAreas = new List<Transform>();

    [Header("Hit Markers")]
    public float hitMarkerAlpha1;
    public float hitMarkerAlpha2;
    public float hitMarkerAlpha3Plus;
    [SerializeField] private float timeTillHitMarkerDestroys;
    // Public
    //[HideInInspector]
    public Collider2D activeHitMarkerCollider;
        //[HideInInspector]
    public List<HitMarker> hitMarkers = new List<HitMarker>();
    [HideInInspector]
    public HitMarker activeHitMarker;
    [HideInInspector]
    public HitBar curCollidingHitArea;
    [HideInInspector]
    public bool canHit;
    [HideInInspector]
    public bool startingMoving;
    [HideInInspector]
    public GameObject hitMarkerGO;
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

    public void LandHitMarker()
    {      
        // Check if the user performed the land hit marker input
        if (_combatManager.CheckRelicUIHiderStatus())
            BeginHitMarkerStoppingSequence(); // Stop the hit marker
    }

    void InitialLaunch()
    {
        _combatManager = FindObjectOfType<CombatManager>();               
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
    /// Set the HitMarkers Starting And Ending position
    /// </summary>
    /// <returns></returns>
    public Vector3 SetHitMarkerStartingAndEndingPos(Transform checkPoint = null)
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

    void DestroyAllHitMarkers()
    {
        for (int i = 0; i < hitMarkers.Count; i++)
        {
            StartCoroutine(hitMarkers[i].DestroyHitMarker());
            hitMarkers.RemoveAt(i);

            if (i == hitMarkers.Count - 1)
            {
                for (int x = 0; x < hitMarkers.Count; x++)
                {
                    StartCoroutine(hitMarkers[i].DestroyHitMarker());
                }

                hitMarkers.Clear();
            }
        }
    }

    public IEnumerator SpawnHitMarker(SkillData skillData)
    {
        DestroyAllHitMarkers();

        for (int i = 0; i < skillData.hitsRequired; i++)
        {
            GameObject go = Instantiate(hitMarkerGO, _initialPos, Quaternion.identity);
            int val = i + 1;
            go.name = "Hit Marker " + val;
            HitMarker hitMarkerScript = go.GetComponent<HitMarker>();
            hitMarkers.Add(hitMarkerScript);

            go.transform.SetParent(hitMarker.transform);
            hitMarkerScript.initialPos = _checkPoints[0].GetComponent<RectTransform>().localPosition;
            hitMarkerScript.nextPos = _checkPoints[1].GetComponent<RectTransform>().localPosition;
            hitMarkerScript.speed = _speed;
            hitMarkerScript.attackBar = this;

            if (i == 0)
            {
                activeHitMarkerCollider = go.GetComponent<BoxCollider2D>();
                activeHitMarker = hitMarkerScript;
                activeHitMarker.SetAsActiveHitMarker();
                _hitMarkerVisual = go.transform.GetChild(0).gameObject;
                hitMarkerScript.UpdateAlpha(hitMarkerAlpha1);
            }
            else if (i == 1)
                hitMarkerScript.UpdateAlpha(hitMarkerAlpha2);
            else if (i >= 2)
                hitMarkerScript.UpdateAlpha(hitMarkerAlpha3Plus);

            yield return new WaitForSeconds(skillData.timeForNextHitMarker);
        }
    }

    /// <summary>
    /// Stops Hit Marker. Called when the hit marker lands on any hit bar (miss, good, great, perfect)
    /// </summary>
    private void StopHitMarker()
    {
        curHitMarkerState = AttackBar.AttackBarState.IDLE;

        activeHitMarker.stopped = true;
    }

    private void ResetAttackBarToDefault()
    {
        curHitMarkerState = AttackBar.AttackBarState.IDLE;
        _initialPos = SetHitMarkerStartingAndEndingPos();

        _step = 0;
    }

    void UpdateActiveHitMarker()
    {
        if (hitMarkers.Count >= 1)
        {
            for (int i = 0; i < hitMarkers.Count; i++)
            {
                if (i == 0)
                {
                    hitMarkers.RemoveAt(i);
                    StartCoroutine(activeHitMarker.DestroyHitMarker(timeTillHitMarkerDestroys));

                    if (hitMarkers.Count == 0)
                    {
                        return;
                    }
                    activeHitMarkerCollider = hitMarkers[i].collider;
                    activeHitMarker = hitMarkers[i];
                    activeHitMarker.SetAsActiveHitMarker();
                    activeHitMarker.UpdateAlpha(hitMarkerAlpha1);
                    _hitMarkerVisual = activeHitMarker._hitMarkerImageGO;
                }
                else if (i == 1)
                {
                    hitMarkers[i].UpdateAlpha(hitMarkerAlpha2);
                    _hitMarkerVisual = hitMarkers[i]._hitMarkerImageGO;
                }
                else if (i >= 2)
                {
                    hitMarkers[i].UpdateAlpha(hitMarkerAlpha3Plus);
                    _hitMarkerVisual = hitMarkers[i]._hitMarkerImageGO;
                } 
            }
        }
    }
    /// <summary>
    /// Stop hit marker, 
    /// check to see which hit bar the hit marker landed on, 
    /// turn hit marker invisible,
    /// Make hit marker start again
    /// </summary>
    public void BeginHitMarkerStoppingSequence()
    {
        // Stop hit marker
        StopHitMarker();

        // Check to see which hit bar the hit marker hit
        curCollidingHitArea.CheckIfMarkerHit();

        UpdateActiveHitMarker();
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

        int rand = Random.Range(0, 1);
        _step = rand;
    }

    public IEnumerator ToggleHitMarkerVisibility(HitMarker hitMarker, GameObject visual, bool toggle, float time = 0)
    {
        yield return new WaitForSeconds(time);

        visual.SetActive(toggle);

        if (toggle)
            hitMarker.curHitMarkerVisibility = HitMarker.HitMarkerVisible.VISIBLE;
        else
            hitMarker.curHitMarkerVisibility = HitMarker.HitMarkerVisible.HIDDEN;
    }
}
