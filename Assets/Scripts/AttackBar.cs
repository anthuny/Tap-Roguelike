using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackBar : MonoBehaviour 
{
    private UIManager _UIManager;

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
    public float timeTillBarTurnsInvis;
    public float timeTillAttackBarReturns;

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
    [SerializeField] private HitsRemainingText _hitsRemainingText;

    [Space(3)]
    [SerializeField] GameObject backGO;
    [Space(3)]
    [Header("Attack Bar Skill Movement")]
    public Transform skillActiveTrans;
    public Transform defaultTrans;
    [HideInInspector]
    public bool skillActive;
    private bool activatedBackButton;
    [HideInInspector]
    public int hitCount;

    private HitBar _hitBar;
    // Public
    [HideInInspector]
    public Collider2D activeHitMarkerCollider;
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

    public string hitMarkerTag;

    void Awake()
    {
        InitialLaunch();

        _UIManager = FindObjectOfType<UIManager>();
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
        relicUICG = relicUIGR.gameObject.GetComponent<CanvasGroup>();
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
        _combatManager.ClearSkillTargets();  // Clear skill targets

        MoveAttackBar(true);
        _UIManager.ToggleImage(_UIManager.endTurnGO, true);    // Enabled end turn button

        // Give mana back to relic for cancelling the skill
        StartCoroutine(_combatManager.activeUnit.UpdateCurMana(_combatManager.activeSkill.manaRequired, true));

        //MoveAttackBar(false);
    }

    public void ToggleImage(GameObject imageGO, bool enabled)
    {
        _UIManager.ToggleImage(imageGO, enabled);
    }

    public void UpdateRemainingHitsText(bool cond, int val = 0)
    {
        _hitsRemainingText.UpdateRemainingHitText(cond, val);
    }

    public void ToggleBackButton(bool cond, bool iso = false)
    {
        _UIManager.ToggleImage(_UIManager.cancelAttackGO, cond);

        if (!iso)
        {
            skillActive = cond;

            if (!cond)
            {
                DestroyActiveHitMarker(0);
                _combatManager.activeAttackData.Clear();
                _combatManager.ClearUnitTargets();
                _combatManager.ClearSkillTargets();
                _UIManager.ToggleImage(_UIManager.hitsRemainingTextGO, cond);
            }
        }
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

    public void SpawnHitMarker(SkillData skillData)
    {
        GameObject go = Instantiate(hitMarkerGO, _initialPos, Quaternion.identity);
        _hitBar = go.GetComponent<HitBar>();

        go.name = "Hit Marker"; 
        HitMarker hitMarkerScript = go.GetComponent<HitMarker>();
        activeHitMarker = hitMarkerScript;
        go.transform.SetParent(hitMarker.transform);

        hitMarkerScript.initialPos = _checkPoints[0].GetComponent<RectTransform>().localPosition;
        hitMarkerScript.nextPos = _checkPoints[1].GetComponent<RectTransform>().localPosition;
        hitMarkerScript.speed = _speed;
        hitMarkerScript.attackBar = this;

        if (_combatManager.relicActiveSkill.curHitsCompleted == 0)
            UpdateRemainingHitsText(true, -(_combatManager.relicActiveSkill.hitsRequired - hitCount));

        activeHitMarkerCollider = go.GetComponent<BoxCollider2D>();
        activeHitMarker = hitMarkerScript;
        activeHitMarker.SetAsActiveHitMarker();
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

    void ToggleRelicSkillUIInput(bool cond)
    {
        relicUIGR.enabled = cond;
    }
}
