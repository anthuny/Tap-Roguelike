﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackBar : MonoBehaviour 
{
    // Inspector variables
    [Header("Main")]
    [SerializeField] private Gamemode _gamemode; 
    [SerializeField] private CombatManager _combatManager;
    [SerializeField] private GameObject hitMarker;
    [SerializeField] private Button _attackButton;
    [SerializeField] private int _hitMarkerStopMouseCode;
    public int skillUIFontSize;
    public int skillUIMissFontSize;
    public int skillUIPerfectFontSize;
    public Color perfectSkillUIColour;
    public Color greatSkillUIColour;
    public Color goodSkillUIColour;
    public Color missSkillUIColour;

    [Header("Statistics")]
    [SerializeField] private float _speed;
    [SerializeField] public float timeTillBarTurnsInvis;

    [Header("Declerations")]
    public List<Transform> _checkPoints = new List<Transform>();
    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();
    [SerializeField] private List<Transform> _hitAreas = new List<Transform>();

    [Header("Relic UI")]
    public GraphicRaycaster relicUIGR;
    private CanvasGroup relicUICG;
    public CanvasGroup relicUIHider;
    public float _relicUIHiderOffVal;
    public float _relicUIHiderSelectVal;
    public float _relicUIHiderOnVal;

    [Header("Hit Markers")]
    public float hitMarkerAlpha1;
    public float hitMarkerAlpha2;
    public float hitMarkerAlpha3Plus;
    [SerializeField] private float timeTillHitMarkerDestroys;

    [Space(3)]
    [SerializeField] GameObject backGO;
    [Space(3)]
    [Header("Attack Bar Skill Movement")]
    public Transform skillActiveTrans;
    public Transform defaultTrans;
    [HideInInspector]
    public bool skillActive;
    private bool activatedBackButton;

    private HitBar _hitBar;
    // Public
    [HideInInspector]
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

    [Header("Variable Names")]
    public string activeHitMarkerTag;
    public string regHitMarkerTag;


    [HideInInspector]
    public Collider2D activeHitMarkerColl;

    void Awake()
    {
        InitialLaunch();
    }

    public void SetActiveHitMarkerCollider(Collider2D coll)
    {
        activeHitMarkerColl = coll;
    }

    public void LandHitMarker()
    {      
        // Check if the user performed the land hit marker input
        if (CheckRelicUIHiderStatus() && skillActive)
        {
            //Debug.Log("Triggered");
            BeginHitMarkerStoppingSequence(); // Stop the hit marker
        }
    }

    void InitialLaunch()
    {
        _combatManager = FindObjectOfType<CombatManager>();
        relicUICG = relicUIGR.gameObject.GetComponent<CanvasGroup>();

        ToggleBackButton(false);
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

    public void MoveAttackBar(bool activating)
    {
        ToggleBackButton(activating);

        if (activating)
        {
            transform.position = skillActiveTrans.position;
            ToggleRelicSkillUIInput(false);
            UpdateUIAlpha(relicUICG, 0);
        }

        else
        {
            transform.position = defaultTrans.position;
            ToggleRelicSkillUIInput(true);
            UpdateUIAlpha(relicUICG, 1);

            _combatManager.relicActiveSkill = null;     // When exiting a skill, remove it from being the relic active skill
        }
    }

    public void BackButtonFunctionality()
    {
        if (!activatedBackButton)
            activatedBackButton = true;
        else
            activatedBackButton = false;

        if (activatedBackButton)
            MoveAttackBar(true);
        else
            MoveAttackBar(false);
    }

    public void ToggleBackButton(bool cond, bool iso = false)
    {
        backGO.SetActive(cond);

        if (!iso)
        {
            skillActive = cond;

            if (!cond)
            {
                DestroyAllHitMarkers();
                _combatManager.activeAttackData.Clear();
                _combatManager.ClearTargetSelections();
                _combatManager.ClearSkillSelections();
            }
        }
    }

    public void DestroyAllHitMarkers()
    {
        if (hitMarkers.Count == 0)
            return;

        for (int i = 0; i < hitMarkers.Count; i++)
            hitMarkers[i].DestroyHitMarkerInstant();

        hitMarkers.Clear();
    }

    public IEnumerator DestroyAllHitMarkersCo()
    {
        yield return new WaitForSeconds(0);
        Invoke("DestroyAllHitMarkers", timeTillHitMarkerDestroys);
    }
    public IEnumerator SpawnHitMarker(SkillData skillData)
    {
        DestroyAllHitMarkers();

        for (int i = 0; i < skillData.hitsRequired; i++)
        {
            GameObject go = Instantiate(hitMarkerGO, _initialPos, Quaternion.identity);
            _hitBar = go.GetComponent<HitBar>();

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
                activeHitMarker.gameObject.tag = activeHitMarkerTag;
            }
            else if (i == 1)
                hitMarkerScript.UpdateAlpha(hitMarkerAlpha2);
            else if (i >= 2)
                hitMarkerScript.UpdateAlpha(hitMarkerAlpha3Plus);

            yield return new WaitForSeconds(skillData.timeForNextHitMarker);
        }
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
                    StartCoroutine(activeHitMarker.DestroyHitMarker(timeTillBarTurnsInvis));

                    if (hitMarkers.Count == 0)
                    {
                        return;
                    }
                    activeHitMarkerCollider = hitMarkers[i].collider;
                    activeHitMarker = hitMarkers[i];
                    activeHitMarker.SetAsActiveHitMarker();
                    activeHitMarker.UpdateAlpha(hitMarkerAlpha1);
                    activeHitMarker.gameObject.tag = activeHitMarkerTag;

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
        // Check to see which hit bar the hit marker hit
        curCollidingHitArea.CheckIfMarkerHit();

        UpdateActiveHitMarker();
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

    /// <summary>
    /// Toggles the attack bar hider's display
    /// </summary>
    /// <param name="cond"></param>
    public void UpdateUIAlpha(CanvasGroup canvasGroup, float alpha)
    {
        canvasGroup.alpha = alpha;
    }

    void ToggleRelicSkillUIInput(bool cond)
    {
        relicUIGR.enabled = cond;
    }

    public bool CheckRelicUIHiderStatus()
    {
        // If the relic UI hider is off
        if (relicUIHider.alpha == _relicUIHiderOffVal)
            return true;

        if (relicUIHider.alpha == _relicUIHiderOnVal)
            return false;

        if (relicUIHider.alpha == _relicUIHiderSelectVal)
            return false;
        else
            return false;
    }
}
