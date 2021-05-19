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

    [SerializeField] private Outline outline;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }
    private void Start()
    {
        Invoke("RemoveText", skillUIManager.textLifeLength);
        StartCoroutine(BeginRemoving());
    }

    IEnumerator BeginRemoving()
    {
        yield return new WaitForSeconds(skillUIManager.textLifeLength);
        destroy = true;
    }

    void Update()
    {
        RemoveText();

        Move();
    }

    void Move()
    {
        float y = skillUIManager.panSpeedUI * Time.deltaTime;
        transform.position += new Vector3(0, y, 0);
    }

    void RemoveText()
    {
        if (!destroy)
            return;

        outline.enabled = false;

        alpha -= skillUIManager.SkillUIFadeOutSpeed * Time.deltaTime;
        _canvasGroup.alpha = alpha;
        
        if (alpha <= 0)
            DestroySkillUI();
    }

    void DestroySkillUI()
    {
        Destroy(this.gameObject);
    }
}
