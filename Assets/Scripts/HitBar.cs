using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBar : MonoBehaviour
{
    Gamemode gm;

    public enum BarType { MISS, MID, HIGH };

    public BarType currentBarType;

    public enum BarState { ALONE, HOVERED };

    public BarState currentBarState;

    Collider2D hitCollider;

    private void Awake()
    {
        gm = FindObjectOfType<Gamemode>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckIfMarkerHit(hitCollider);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        currentBarState = BarState.HOVERED;
        hitCollider = collision;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        currentBarState = BarState.ALONE;
        hitCollider = collision;
    }

    void CheckIfMarkerHit(Collider2D col)
    {
        if (hitCollider == col
            && currentBarState == BarState.HOVERED
            && gm.hitMarkerStarted)
        {
            if (gm.attackDetected && !gm.ab.hitMarkerStopped)
            {
                gm.ab.hitMarkerStopped = true;

                switch (currentBarType)
                {
                    case BarType.HIGH:
                        // Debug
                        StartCoroutine(gm.dm.FlashText("Player hit High"));

                        gm.ab.StopHitMarker();
                        break;

                    case BarType.MID:
                        // Debug
                        StartCoroutine(gm.dm.FlashText("Player hit Medium"));

                        gm.ab.StopHitMarker();
                        break;
                }

                if (currentBarType == BarType.MISS)
                {
                    // Debug
                    StartCoroutine(gm.dm.FlashText("Player missed"));
                    gm.ab.StopHitMarker();
                }
            }
        }
    }
}
