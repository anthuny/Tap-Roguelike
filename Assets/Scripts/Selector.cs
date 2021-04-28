using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{
    private CombatManager _combatManager;
    private SkillUIManager _skillUIManager;

    [SerializeField] private Button _selectButton;
    [HideInInspector]
    public bool selectEnabled;
    public Image selectionImage;
    public RectTransform iconBorderRT;

    [Space(1)]
    [Header("Skill UI Initialization")]
    [SerializeField] private Image skillIcon;
    [SerializeField] private Image skillBorderIcon;
    [SerializeField] private Image skillSelectionIcon;

    [Header("Main")]
    [HideInInspector]
    public SkillData skillData;

    [Space(1)]
    [Header("Skill UI Settings")]
    public Color skillIconColour;
    public Color skillBorderColour;
    public Color skillSelectionColour;
  

    [Tooltip("Units added to skill's icon size height dimension. (A value of 1 is the lowest whilst still displaying the border")]
    [SerializeField] private int skillBorderHeight;
    [Tooltip("Units added to skill's icon size width dimension. (A value of 1 is the lowest whilst still displaying the border")]
    [SerializeField] private int skillBorderWidth;
    [SerializeField] private bool isSkill;

    private void Awake()
    {
        _combatManager = FindObjectOfType<CombatManager>();
        _skillUIManager = FindObjectOfType<SkillUIManager>();

        selectionImage = GetComponent<Image>();
    }

    private void Start()
    {
        _selectButton.onClick.AddListener(ToggleSelectionImage);        // Add a listener to the button on a unit    
    }

    /// <summary>
    /// Toggle the selection image
    /// </summary>
    private void ToggleSelectionImage()
    {
        if (isSkill)
        {
            // If selection is not displayed, display it
            if (!selectEnabled)
            {
                _combatManager.maxTargetSelections = _combatManager.relicActiveSkill.maxTargetCount;
                _combatManager.maxSkillSelections = _combatManager.relicActiveSkill.maxSkillCount;
                _combatManager.oldCurTargetSelections = _combatManager.curTargetSelections;

                _combatManager.ManageSelectionCount(true, true, this, selectionImage, iconBorderRT, new Vector2(skillBorderHeight, skillBorderWidth));
            }
        }
        else
        {
            // If selection is not displayed, display it 
            if (!selectEnabled)
                _combatManager.ManageSelectionCount(true, false, this, selectionImage, iconBorderRT, new Vector2(skillBorderHeight, skillBorderWidth));
            
            // If selection is displayed, hide it
            else
                _combatManager.ManageSelectionCount(false, false, this, selectionImage, iconBorderRT, new Vector2(skillBorderHeight, skillBorderWidth));
        }
    }

    public void AssignSkillUIAesthetics()
    {
        _skillUIManager.AssignSkillAesthetics(skillIcon, skillSelectionIcon, skillBorderIcon, 
            skillIconColour, skillSelectionColour, skillBorderColour);

        skillSelectionIcon.enabled = false;
    }
}
