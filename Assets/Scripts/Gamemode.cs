using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamemode : MonoBehaviour
{
    private AttackBar attackBarScript;

    [Tooltip("The minimum distance the hit marker must be to the nearest checkpoint before the next checkpoint is made the next target")]
    public float minDistCheckPoint;

    // Start is called before the first frame update
    void Start()
    {
        InitialLaunch();
    }

    public void InitialLaunch()
    {
        Screen.fullScreen = true;

        Screen.orientation = ScreenOrientation.Portrait;

        FindActiveAttackBar();
    }

    public void FindActiveAttackBar()
    {
        attackBarScript = FindObjectOfType<AttackBar>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("a");

            // Begin attack bar pattern
            StartCoroutine(attackBarScript.BeginAttackBarPattern());
        }
    }
}
