using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject endTurnGO;
    public GameObject startFightGO;
    public GameObject cancelAttackGO;
    public GameObject skillUIGO;
    public GameObject hitsRemainingTextGO;
    public GameObject attackBarGO;
    public GameObject relicSkillGO;
    public GameObject targetedEnemyInfoGO;
    public GameObject enemySkillDetailsGO;
    public GameObject enemyAllSkillsGO;

    public void ToggleImage(GameObject imageGO, bool enabled)
    {
        imageGO.SetActive(enabled);
    }
}
