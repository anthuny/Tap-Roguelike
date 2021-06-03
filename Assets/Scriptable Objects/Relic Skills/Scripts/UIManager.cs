using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject endTurnGO;
    public GameObject startFightGO;
    public GameObject cancelAttackGO;
    public GameObject skillsUIGO;
    public GameObject hitsRemainingTextGO;
    public GameObject attackBarGO;
    public GameObject relicSkillGO;
    public GameObject targetedEnemyInfoGO;
    public GameObject enemySkillUIGO;
    public GameObject enemySkillDetailsGO;
    public GameObject enemyAllSkillsGO;
    public GameObject relicActiveSkillDetailsGO;

    public IEnumerator ToggleImage(GameObject imageGO, bool enabled, float time = 0)
    {
        yield return new WaitForSeconds(time);

        imageGO.SetActive(enabled);
    }
}
