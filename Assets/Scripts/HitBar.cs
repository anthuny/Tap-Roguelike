using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBar : MonoBehaviour
{
    Gamemode gm;

    public enum BarType { MID, HIGH };

    public BarType currentBarType;

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

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gm.attackDetected)
        {
            switch (currentBarType)
            {
                case BarType.MID:
                    Debug.Log("mid");

                    gm.attackBarScript.StopHitMarker();

                    break;

                case BarType.HIGH:
                    Debug.Log("high");

                    gm.attackBarScript.StopHitMarker();

                    break;
            }
        }
    }
}
