using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject endTurnGO;
    public GameObject startFightGO;
    public GameObject hitsRemainingTextGO;
    public GameObject attackBarGO;
    public GameObject targetedUnitInfoGO;

    public IEnumerator ToggleImage(GameObject imageGO, bool enabled, float time = 0)
    {
        yield return new WaitForSeconds(time);

        imageGO.SetActive(enabled);
    }
}
