using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackBar : MonoBehaviour 
{
    public UIManager _UIManager;

    // Inspector variables
    [Header("Main")]
    [SerializeField] private Gamemode _gamemode; 
    public CombatManager _combatManager;
    public GameObject hitMarkerPar;
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
    public float timeTillBarTurnsInvis;
    public float timeTillAttackBarReturns;

    [Header("Declerations")]
    public List<Transform> _checkPoints = new List<Transform>();
    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();
    [SerializeField] private List<Transform> _hitAreas = new List<Transform>();

    [Header("Relic UI")]
    public GameObject skillsUIGO;
    private CanvasGroup skillsUICGGO;
    public CanvasGroup relicUIHider;
    public float _relicUIHiderOffVal;
    public float _relicUIHiderSelectVal;
    public float _relicUIHiderOnVal;
    [SerializeField] private HitsRemainingText _hitsRemainingText;

    [Space(3)]
    [SerializeField] GameObject backGO;
    [Space(3)]
    [Header("Attack Bar Skill Movement")]
    public Transform skillActiveTrans;
    public Transform defaultTrans;
    //[HideInInspector]
    public bool skillActive;
    private bool activatedBackButton;
    [HideInInspector]
    public int hitCount;

    private HitBar _hitBar;
    // Public
    [HideInInspector]
    public Collider2D activeHitMarkerCollider;
    //[HideInInspector]
    public HitMarker activeHitMarker;
    [HideInInspector]
    public HitBar curCollidingHitArea;
    [HideInInspector]
    public bool canHit;
    [HideInInspector]
    public bool startingMoving;
    public GameObject hitMarkerGO;
    // Private
    private RectTransform _hitMarkerRT;
    private GameObject _hitMarkerVisual;
    private float _step;
    private Vector3 _initialPos;
    private Vector3 _nextPos;

    public string hitMarkerTag;

    void Awake()
    {
        InitialLaunch();

        _UIManager = FindObjectOfType<UIManager>();

        HideRelicAtackUI();
    }

    public void HideRelicAtackUI()
    {
        StartCoroutine(_UIManager.ToggleImage(_UIManager.cancelAttackGO, false));   // Disable cancel attack button
        StartCoroutine(_UIManager.ToggleImage(_UIManager.hitsRemainingTextGO, false));  // Hide hits remaining text
        //StartCoroutine(_UIManager.ToggleImage(_UIManager.skillsUIGO, false));   // Hide relic skills UI
        StartCoroutine(_UIManager.ToggleImage(_UIManager.endTurnGO, false));    // Enable end turn button
        StartCoroutine(_UIManager.ToggleImage(_UIManager.attackBarGO, false));    // Enable end turn button
    }

    public void Refresh()
    {
        HideRelicAtackUI();
    }
    public void LandHitMarker()
    {      
        // Check if the user performed the land hit marker input
        if (skillActive)
            BeginHitMarkerStoppingSequence(); // Stop the hit marker
    }

    public void UpdateActiveSkill(bool active)
    {
        skillActive = active;
    }

    void InitialLaunch()
    {
        _combatManager = FindObjectOfType<CombatManager>();
        skillsUICGGO = skillsUIGO.gameObject.GetComponent<CanvasGroup>();
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

    public void UpdateSkillActive(bool enable)
    {
        skillActive = enable;
    }

    public void ToggleRelicAttackUI(bool enable)
    {
        UpdateSkillActive(enable);

        _UIManager = FindObjectOfType<UIManager>();
        if (!enable)
        {
            StartCoroutine(_UIManager.ToggleImage(_UIManager.cancelAttackGO, false));   // Disable cancel attack button
            StartCoroutine(_UIManager.ToggleImage(_UIManager.hitsRemainingTextGO, false));  // Hide hits remaining text
            //StartCoroutine(_UIManager.ToggleImage(_UIManager.skillsUIGO, true));   // Show relic skills UI
            StartCoroutine(_UIManager.ToggleImage(_UIManager.endTurnGO, true));    // Show end turn button

            if (_combatManager)
                _combatManager.relicActiveSkill = null;     // When exiting a skill, remove it from being the relic active skill
        }
        else
        {
            StartCoroutine(_UIManager.ToggleImage(_UIManager.cancelAttackGO, true));   // Show cancel attack button
            StartCoroutine(_UIManager.ToggleImage(_UIManager.attackBarGO, true));  // Show attack bar.
            StartCoroutine(_UIManager.ToggleImage(_UIManager.hitsRemainingTextGO, true));  // Show hits remaining text
            //StartCoroutine(_UIManager.ToggleImage(_UIManager.skillsUIGO, false));   // Hide relic skills UI
            StartCoroutine(_UIManager.ToggleImage(_UIManager.endTurnGO, false));
        }
    }
    public void BackButtonFunctionality(bool wasButton)
    {
        _combatManager = FindObjectOfType<CombatManager>();
        _combatManager.ClearSkillTargets();  // Clear skill targets

        ToggleRelicAttackUI(false);

        //Destroy hit markers
        if (activeHitMarker)
            DestroyActiveHitMarker(0);

        if (wasButton)
            // Give mana back to relic for cancelling the skill
            StartCoroutine(_combatManager.activeUnit.UpdateCurMana(_combatManager.activeSkill.manaRequired, true));
    }

    public void UpdateRemainingHitsText(bool cond, int val = 0)
    {
        _hitsRemainingText.UpdateRemainingHitText(cond, val);
    }

    public void DestroyActiveHitMarker(float time)
    {
        if (activeHitMarker)
        {
            StartCoroutine(activeHitMarker.DestroyHitMarker(time));
            activeHitMarkerCollider = null;
            activeHitMarker = null;
        }
    }

    public void ResetHitCount()
    {
        if (hitCount != 0)
            hitCount = 0;
    }

    void SetActiveHitMarker(GameObject go)
    {
        activeHitMarker = go.GetComponent<HitMarker>();
    }
    public void SpawnHitMarker(SkillData skillData)
    {
        GameObject go = Instantiate(hitMarkerGO, _initialPos, Quaternion.identity);
        SetActiveHitMarker(go);
        activeHitMarker.SetPositions(_checkPoints[0].GetComponent<RectTransform>().localPosition, _checkPoints[1].GetComponent<RectTransform>().localPosition);
        activeHitMarker.SetSpeed(_speed);

        _hitBar = go.GetComponent<HitBar>();

        if (_combatManager.relicActiveSkill.curHitsCompleted == 0)
            UpdateRemainingHitsText(true, -(_combatManager.relicActiveSkill.hitsRequired - hitCount));

        activeHitMarkerCollider = go.GetComponent<BoxCollider2D>();
        _hitMarkerVisual = go.transform.GetChild(0).gameObject;
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
}
