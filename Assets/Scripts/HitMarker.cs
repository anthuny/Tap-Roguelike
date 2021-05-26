using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitMarker : MonoBehaviour
{
    [HideInInspector]
    public enum HitMarkerVisible { HIDDEN, VISIBLE };
    [HideInInspector]
    public HitMarkerVisible curHitMarkerVisibility;

    [HideInInspector]
    public AttackBar attackBar;
    [HideInInspector]
    public float step = 1, speed;
    [HideInInspector]
    public Vector3 initialPos, oldInitialPos, nextPos;

    [HideInInspector]
    public bool stopped;

    public BoxCollider2D collider;
    public GameObject _hitMarkerImageGO;
    [SerializeField] private CanvasGroup _canvasgroup;
    [SerializeField] private RectTransform _hitMarkerRT;
    private BoxCollider2D hitMarkerCollider;

    private void Start()
    {
        _hitMarkerRT = gameObject.GetComponent<RectTransform>();
        _hitMarkerImageGO.SetActive(false);
        hitMarkerCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PingPong();
    }

    void ToggleDisplay()
    {
        StartCoroutine(attackBar.ToggleHitMarkerVisibility(this, _hitMarkerImageGO, true, 0));
    }

    void PingPong()
    {
        if (stopped)
            return;

        if (curHitMarkerVisibility == HitMarkerVisible.HIDDEN)
        {
            ToggleDisplay();
        }

        // If the hit marker hasn't reached it's destination, move towards it
        if (step != 1)
        {
            // Move hit marker position
            step += speed * Time.deltaTime;
            step = Mathf.Clamp(step, 0, 1);
            Vector3 pos = Vector2.Lerp(initialPos, nextPos, step);
            _hitMarkerRT.localPosition = pos;
        }
        else if (step == 1)
        {
            oldInitialPos = initialPos;
            initialPos = nextPos;
            nextPos = oldInitialPos;
            step = 0;
        }
    }

    public void UpdateAlpha(float alpha)
    {
        _canvasgroup.alpha = alpha;
    }

    public void DestroyHitMarkerInstant()
    {
        Destroy(this.gameObject);
    }

    public IEnumerator DestroyHitMarker(float time)
    {
        stopped = true;
        yield return new WaitForSeconds(time);

        Destroy(this.gameObject);
    }

    public void SetAsActiveHitMarker()
    {
        attackBar.activeHitMarker = this;
    }
}
