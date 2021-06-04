using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    private CombatManager _combatManager;
    public SkillUIManager _skillUIManager;
    private UIManager _uIManager;
    public Unit unit;
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
    public SkillData skillData;
    public Image cooldownImage;
    public Text cooldownText;

    [Space(1)]
    [Header("Skill UI Settings")]
    public Color skillIconColour;
    public Color skillBorderColour;
    public Color skillSelectionColour;
  
    [SerializeField] private bool isSkill;
    public int enemyIndex;

    private bool tempBool;

    private void Awake()
    {
        _combatManager = FindObjectOfType<CombatManager>();
        selectionImage = GetComponent<Image>();
        _uIManager = FindObjectOfType<UIManager>();
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
        /*
        if (!tempBool)
        {
            tempBool = true;
            return;
        }
        */

        if (isSkill)
        {
            if (skillData.onCooldown || !skillData.activatable || !_combatManager.relicTurn)
                return;

            // Don't continue if unit doesnt have enough mana for skill
            if (!_combatManager.activeUnit.HasEnoughManaForSkill())
                return;

            _combatManager.activeAttackBar.UpdateActiveSkill(true);     // Sets skill active as true

            // Manage unit targets
            _combatManager.ManageTargets(true, false, this,
            _combatManager.curUnitTargets, _combatManager.activeSkill.maxTargetCount, skillData);

            _combatManager.activeAttackBar.UpdateSkillActive(true);
        }
        else
        {
            if (skillData)
            {
                // If ally, filter all but enemy target selections
                if (unit.unitType == Unit.UnitType.ALLY)
                    _combatManager.ManageTargets(false, false, this,
                    _combatManager.curUnitTargets, _combatManager.activeSkill.maxTargetCount, skillData);
                // If enemy, filter all but ally target selections
                else
                    _combatManager.ManageTargets(false, true, this,
                    _combatManager.curUnitTargets, _combatManager.activeSkill.maxTargetCount, skillData);

                // If selection is not displayed, display it 
                if (!selectEnabled)
                    _combatManager.UpdateUnitTargets(this, enemyIndex, true);

                // If selection is displayed, hide it
                else
                    _combatManager.UpdateUnitTargets(this, enemyIndex, false);
            }

            // If selecting a unit
            else
            {
                // If selection is not displayed, display it 
                if (!selectEnabled)
                    _combatManager.AddUnitTarget(this);
                // If selection is displayed, hide it
                else
                {
                    _combatManager.RemoveUnitTarget(this);
                }

            }
        }
    }

    public void AssignSkillUIAesthetics()
    {
        _skillUIManager.AssignSkillAesthetics(skillIcon, skillSelectionIcon, skillBorderIcon, 
            skillIconColour, skillSelectionColour, skillBorderColour);

        skillSelectionIcon.enabled = false;
    }
}
