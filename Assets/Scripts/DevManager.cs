using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevManager : MonoBehaviour
{
    private List<GameObject> devTexts = new List<GameObject>();

    [SerializeField] private Gamemode _gamemode;
    [SerializeField] private AttackBar _attackBar;
    [SerializeField] private CombatManager _combatManager;

    [Header("Developer Mode")]
    [SerializeField] private GameObject _devTextParent;
    [SerializeField] private bool _devVisualsOn;
    [SerializeField] private bool _devTextOn;
    [SerializeField] private float _distAfterFirstText;
    [SerializeField] private float _distAfterSecondText;
    [Tooltip("Dev Text prefab")]
    [SerializeField] private GameObject _devText;
    [SerializeField] private Font _font;
    [SerializeField] private int _maxDevTextCount;
    [SerializeField] private int fontSize;

    private GameObject activeGO;
    private Text activeText;


    private void Awake()
    {
        InitialLaunch();
    }

    public void InitialLaunch()
    {
        if (!_devVisualsOn)
        {
            _attackBar.DisableBarVisuals();
        }
    }

    public void FlashText(string castorName, string targetName, string skillName, float skillVal, Unit target, int inflictUpTime = 0, string inflictName = "Nothing")
    {
        // If dev mode is not on, do not continue
        if (!_devTextOn)
            return;          

        PositionTexts(castorName, targetName, skillName, skillVal, inflictUpTime, target, inflictName);
    }

    /// <summary>
    /// Apply New Combat Text and position all texts
    /// </summary>
    void PositionTexts(string castorName, string targetName, string skillName, float skillVal, int inflictUpTime, Unit target, string inflictName = "Nothing")
    {
        GameObject go = Instantiate(_devText, _devTextParent.transform.position, Quaternion.identity);
        go.transform.SetParent(_devTextParent.transform);
        go.transform.position = _devTextParent.transform.position;
        go.name = "DevText";
        go.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 150);


        // Insert the new line of text at the start of the list
        devTexts.Insert(0, go);

        int skillDamage = RoundFloatToInt(skillVal * _combatManager.relicActiveSkillValueModifier);
        int recievedDamageAmp = RoundFloatToInt((target.recievedDamageAmp * (skillVal * _combatManager.relicActiveSkillValueModifier)));

        Text text = go.GetComponent<Text>();
        text.text = castorName + " used " + skillName + " at " + targetName + " for ( " +
            skillDamage + " + " + recievedDamageAmp + " ) applying " + inflictName + 
            " ( " + inflictUpTime + " )";
        text.font = _font;
        text.fontSize = fontSize;

        // Move past lines of text down for each new line
        for (int i = 0; i < devTexts.Count; i++)
        {
            // Skip the first element in list. It is always the newest line of text
            if (i != 0)
            {
                if (i == 1)
                    devTexts[i].transform.position += new Vector3(0, -_distAfterSecondText, 0);
                else
                    devTexts[i].transform.position += new Vector3(0, -_distAfterFirstText, 0);
            }

            // Cap the amount of lines by removing the oldest lines of text
            if (i >= _maxDevTextCount)
            {
                Destroy(devTexts[devTexts.Count - 1]);
                devTexts.RemoveAt(devTexts.Count - 1);
            }
        }
    }

    private int RoundFloatToInt(float f)
    {
        return Mathf.RoundToInt(f);
    }
}
