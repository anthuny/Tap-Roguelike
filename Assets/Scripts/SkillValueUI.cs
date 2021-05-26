using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillValueUI : MonoBehaviour
{
    [HideInInspector]
    public SkillUIManager skillUIManager;
    [HideInInspector]
    public Canvas canvas;
    [SerializeField] private CanvasGroup _canvasGroup;
    // Update is called once per frame

    private float alpha = 1;
    private bool destroy;
    [HideInInspector]
    public bool stopMoving = false;

    private Coroutine coroutine;
    private bool stopping;

    [HideInInspector]
    public int index;

    [SerializeField] private Outline outline;
    private float originalPosY;
    private float stoppedPosY;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }
    private void Start()
    {
        originalPosY = transform.localPosition.y;
        EnableMoving();
    }

    void UpdateStoppedPos()
    {
        if (!destroy)
            stoppedPosY = transform.localPosition.y;
    }

    public void BeginRemoving()
    {
        destroy = true;
    }
    
    public float CalculateDistanceTravelled()
    {
        return stoppedPosY - originalPosY;
    }

    IEnumerator StopMoving()
    {
        stopping = true;
        yield return new WaitForSeconds(skillUIManager.textPanLength);
        stopMoving = true;

        stoppedPosY = transform.position.y;
        stopping = false;
    }

    public void EnableMoving()
    {
        if (stopping)
            StopCoroutine(coroutine);

        stopMoving = false;
        coroutine = StartCoroutine(StopMoving());
    }

    public void HideText()
    {
        destroy = true;
    }

    void Update()
    {
        UpdateStoppedPos();
        StartCoroutine(HideTextFunctionality());
        Move();
    }

    void Move()
    {
        if (stopMoving)
            return;

        float y = skillUIManager.panSpeedUI * Time.deltaTime;
        transform.position += new Vector3(0, y, 0);
    }

    IEnumerator HideTextFunctionality()
    {
        if (!destroy)
            yield break;

        stopMoving = false;

        yield return new WaitForSeconds(skillUIManager.textBonusLength + (index * skillUIManager.elapsedTimeDestroyMultiplier));

        outline.enabled = false;

        alpha -= skillUIManager.SkillUIFadeOutSpeed * Time.deltaTime;
        _canvasGroup.alpha = alpha;
        
        if (alpha <= 0)
            DestroySkillUI();
    }

    void DestroySkillUI()
    {
        skillUIManager.RemoveText(this);
        Destroy(this.gameObject);
    }
}
