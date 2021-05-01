﻿using System.Collections;
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
  
    [SerializeField] private bool isSkill;
    public bool startingSkill;
    public bool startingSkillActivated;
    public int enemyCount;

    private void Awake()
    {
        _combatManager = FindObjectOfType<CombatManager>();
        _skillUIManager = FindObjectOfType<SkillUIManager>();

        selectionImage = GetComponent<Image>();
    }

    private void Start()
    {
        _selectButton.onClick.AddListener(ToggleSelectionImage); // Add a listener to the button on a unit    
    }

    /// <summary>
    /// Toggle the selection image
    /// </summary>
    public void ToggleSelectionImage()
    {
        if (isSkill)
        {
            // If selection is not displayed, display it
            if (!selectEnabled)
            {
                if (startingSkill && !startingSkillActivated)
                {
                    startingSkillActivated = true;

                    _combatManager.ManageSelectionCount(true, this,
                    _combatManager.curSkillSelections, skillData.maxSkillCount, _combatManager.curTargetSelections, skillData.maxTargetCount, skillData, true);
                }
                else
                {
                    _combatManager.ManageSelectionCount(true, this,
                    _combatManager.curSkillSelections, skillData.maxSkillCount, _combatManager.curTargetSelections, skillData.maxTargetCount, skillData, false);
                }
            }
        }
        else
        {
            // If selection is not displayed, display it 
            if (!selectEnabled)
                _combatManager.UpdateTargetSelection(this, enemyCount, true);

            // If selection is displayed, hide it
            else
                    _combatManager.UpdateTargetSelection(this, enemyCount, false);
        }
    }

    public void AssignSkillUIAesthetics()
    {
        _skillUIManager.AssignSkillAesthetics(skillIcon, skillSelectionIcon, skillBorderIcon, 
            skillIconColour, skillSelectionColour, skillBorderColour);

        skillSelectionIcon.enabled = false;
    }
}
