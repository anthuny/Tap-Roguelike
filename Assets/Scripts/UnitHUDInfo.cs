using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHUDInfo : MonoBehaviour
{
    private CombatManager _combatManager;
    private UIManager _uiManager;

    [Header("Target Unit Main")]
    public GameObject cancelAttackGO;
    public GameObject attackBarDarkenerGO;
    [SerializeField] private Text _tNameText;
    [SerializeField] private Image _tUnitPortrait;
    [SerializeField] private Text _tUnitLevelText;
    [SerializeField] private Text _tUnitHealthText;
    public Text tUnitManaText;
    [SerializeField] private Text _tUnitEnergyText;

    [Header("Enemy Unit Panels")]
    public GameObject eAllSkillPanel;

    [Space(3)]
    [Header("Enemy Unit Skills")]
    public SkillIconUI eBasicSkillIcon;
    public SkillIconUI ePrimarySkillIcon;
    public SkillIconUI eSecondarySkillIcon;
    public SkillIconUI eAlternateSkillIcon;

    [Header("Relic Panels")]
    public GameObject rAllSkillPanel;
    public GameObject rActiveSkillPanel;
    public GameObject rAttackBarPanel;
    public GameObject attackButton;

    [Space(3)]
    [Header("Relic Unit Skills")]
    [SerializeField] private SkillIconUI _rBasicSkillIcon;
    [SerializeField] private SkillIconUI _rPrimarySkillIcon;
    [SerializeField] private SkillIconUI _rSecondarySkillIcon;
    [SerializeField] private SkillIconUI _rAlternateSkillIcon;

    [Space(1)]
    /*
    [Header("Enemy Main")]
    [SerializeField] private Text _rNameText;
    [SerializeField] private Image _rUnitPortrait;
    [SerializeField] private Text _rUnitLevelText;
    [SerializeField] private Text _rUnitHealthText;
    [SerializeField] private Text _rUnitManaText;
    [SerializeField] private Text _rUnitEnergyText;
    */
    [Space(3)]
    
    [Header("Relic Active Skill")]
    [SerializeField] private Image _rActiveSkillImage;
    [SerializeField] private Text _rActiveSkillNameText;
    [SerializeField] private Text _rActiveSkillDescText;
    [SerializeField] private Text _rActiveSkillManaCost;
    [SerializeField] private Text _rActiveSkillCD;
    [SerializeField] private Text _rActiveSkillPerfectHitText;
    [SerializeField] private Text _rActiveSkillGreatHitText;
    [SerializeField] private Text _rActiveSkillGoodHitText;
    [SerializeField] private Text _rActiveSkillMissHitText;
    [SerializeField] private Image _rActiveSkillRemainingCDImage;
    [SerializeField] private Text _rActiveSkillRemaingingCDText;

    [Space(1)]

    [Header("Other")]
    [Tooltip("The prefab GameObject of Selected Unit Portrait")]
    [SerializeField] private GameObject _selectedUnitPortrait;
    [Tooltip("The Grid Layout Group component attached to selected unit portraits parent.")]
    [SerializeField] private List<SelectedUnitPortrait> _selectedUnitPortraits = new List<SelectedUnitPortrait>();
    [SerializeField] private Image _allyTeamPortraitImage;
    [SerializeField] private Image _enemyTeamPortraitImage;


    [HideInInspector]
    public Unit unit;

    private void Start()
    {
        _uiManager = FindObjectOfType<UIManager>();
        _combatManager = FindObjectOfType<CombatManager>();

        RemoveAllUI();
    }

    public void ToggleSkillSelectionImage(SkillData activeSkill, bool enable)
    {
        if (_combatManager.activeUnit.GetUnitType() == Unit.UnitType.ALLY)
        {
            switch (activeSkill.skillType)
            {
                case "Basic":
                    _rBasicSkillIcon.ToggleSelectionImage(enable);
                    break;
                case "Primary":
                    _rPrimarySkillIcon.ToggleSelectionImage(enable);
                    break;
                case "Secondary":
                    _rSecondarySkillIcon.ToggleSelectionImage(enable);
                    break;
                case "Alternate":
                    _rAlternateSkillIcon.ToggleSelectionImage(enable);
                    break;
            }
        }
        else
        {
            switch (activeSkill.skillType)
            {
                case "Basic":
                    eBasicSkillIcon.ToggleSelectionImage(enable);
                    break;
                case "Primary":
                    ePrimarySkillIcon.ToggleSelectionImage(enable);
                    break;
                case "Secondary":
                    eSecondarySkillIcon.ToggleSelectionImage(enable);
                    break;
                case "Alternate":
                    eAlternateSkillIcon.ToggleSelectionImage(enable);
                    break;
            }
        }
    }

    public void DisableAllSkillSelectionImages()
    {
        if (_combatManager.activeUnit.GetUnitType() == Unit.UnitType.ALLY)
        {
            _rBasicSkillIcon.ToggleSelectionImage(false);
            _rPrimarySkillIcon.ToggleSelectionImage(false);
            _rSecondarySkillIcon.ToggleSelectionImage(false);
            _rAlternateSkillIcon.ToggleSelectionImage(false);
        }
        else
        {
            eBasicSkillIcon.ToggleSelectionImage(false);
            ePrimarySkillIcon.ToggleSelectionImage(false);
            eSecondarySkillIcon.ToggleSelectionImage(false);
            eAlternateSkillIcon.ToggleSelectionImage(false);
        }
    }
    public void ExitSkillDetailsButton()
    {
        // Remove skill detail panel, display all skill panel
        ExitSkillDetailPanel();

        // Remove all unit select images
        _combatManager.ClearUnitSelectImages();
    }

    public void StartAttack()
    {
        // Toggle UI to display attack bar
        ToggleToAttackBar();

        // Spawn Hit marker for skill
        _combatManager.activeAttackBar.SpawnHitMarker(_combatManager.activeSkill);
    }

    void ExitSkillDetailPanel()
    {
        TogglePanel(rAllSkillPanel, true);
        TogglePanel(rActiveSkillPanel, false);
    }
    public void ToggleToAttackBar()
    {
        TogglePanel(rAttackBarPanel, true);
        TogglePanel(rActiveSkillPanel, false);
        TogglePanel(cancelAttackGO, true);
        TogglePanel(attackBarDarkenerGO, true);
    }

    public void ToggleToAllSkillsPanel()
    {
        TogglePanel(rAttackBarPanel, false);
        TogglePanel(rActiveSkillPanel, false);
        TogglePanel(cancelAttackGO, false);
        TogglePanel(rAllSkillPanel, true);
        TogglePanel(attackBarDarkenerGO, false);
    }
    void RemoveAllUI()
    {
        TogglePanel(eAllSkillPanel, false);
        TogglePanel(rAllSkillPanel, false);
        TogglePanel(rActiveSkillPanel, false);
        TogglePanel(attackBarDarkenerGO, false);
    }

    public void DeselectAllSelections()
    {
        eBasicSkillIcon.ToggleSelectionImage(false);
        ePrimarySkillIcon.ToggleSelectionImage(false);
        eSecondarySkillIcon.ToggleSelectionImage(false);
        eAlternateSkillIcon.ToggleSelectionImage(false);
    }

    public void AssignUnitSkillsToSkillIcon(Unit unit)
    {
        if (unit.unitType == Unit.UnitType.ALLY)
        {
            _rBasicSkillIcon.ReferenceUnit();
            _rPrimarySkillIcon.ReferenceUnit();
            _rSecondarySkillIcon.ReferenceUnit();
            _rAlternateSkillIcon.ReferenceUnit();
        }
        else
        {
            eBasicSkillIcon.ReferenceUnit();
            ePrimarySkillIcon.ReferenceUnit();
            eSecondarySkillIcon.ReferenceUnit();
            eAlternateSkillIcon.ReferenceUnit();
        }

        UpdateSkillInvalidImages(unit);
    }
    public void TogglePanels(Unit unit)
    {
        this.unit = unit;
        // If enemy, update enemy icon unit reference
        if (unit.unitType == Unit.UnitType.ENEMY)
        {
            eBasicSkillIcon.SetUnit(unit);
            ePrimarySkillIcon.SetUnit(unit);
            eSecondarySkillIcon.SetUnit(unit);
            eAlternateSkillIcon.SetUnit(unit);

            TogglePanel(eAllSkillPanel, true);
        }
        // If enemy, update relic icon unit reference
        else
        {
            _rBasicSkillIcon.SetUnit(unit);
            _rPrimarySkillIcon.SetUnit(unit);
            _rSecondarySkillIcon.SetUnit(unit);
            _rAlternateSkillIcon.SetUnit(unit);

            TogglePanel(rAllSkillPanel, true);
            TogglePanel(rActiveSkillPanel, false);
        }

        //SetUnitInfoUI(unit);
        SetUnitAllSkills(unit);
    }

    void SetUnitAllSkills(Unit unit)
    {
        // If Enemy turn
        if (!_combatManager.relicTurn)
        {
            //_ePassiveSkillIcon.SetSkillImage(_ePassiveSkillIcon.skillImage, )

            /*
            // Set Enemy all skill CD Image Value
            tPassiveSkillIcon.SetCDImageValue(tPassiveSkillIcon.cdImage, unit.passiveSkill.curCooldown, unit.passiveSkill.turnCooldown);
            tBasicSkillIcon.SetCDImageValue(tBasicSkillIcon.cdImage, unit.basicSkill.curCooldown, unit.basicSkill.turnCooldown);
            tPrimarySkillIcon.SetCDImageValue(tPrimarySkillIcon.cdImage, unit.primarySkill.curCooldown, unit.primarySkill.turnCooldown);
            tSecondarySkillIcon.SetCDImageValue(tSecondarySkillIcon.cdImage, unit.secondarySkill.curCooldown, unit.secondarySkill.turnCooldown);

            // Update enemy all skill cd image value
            tPassiveSkillIcon.SetCDImageValue(tPassiveSkillIcon.cdImage, unit.passiveSkill.curCooldown, unit.passiveSkill.turnCooldown);
            tBasicSkillIcon.SetCDImageValue(tBasicSkillIcon.cdImage, unit.basicSkill.curCooldown, unit.basicSkill.turnCooldown);
            tPrimarySkillIcon.SetCDImageValue(tPrimarySkillIcon.cdImage, unit.primarySkill.curCooldown, unit.primarySkill.turnCooldown);
            tSecondarySkillIcon.SetCDImageValue(tSecondarySkillIcon.cdImage, unit.secondarySkill.curCooldown, unit.secondarySkill.turnCooldown);

            // Update enemy all skill cd text value
            tPassiveSkillIcon.SetCDText(tPassiveSkillIcon.cdText, unit.passiveSkill.curCooldown);
            tBasicSkillIcon.SetCDText(tBasicSkillIcon.cdText, unit.basicSkill.curCooldown);
            tPrimarySkillIcon.SetCDText(tPrimarySkillIcon.cdText, unit.primarySkill.curCooldown);
            tSecondarySkillIcon.SetCDText(tSecondarySkillIcon.cdText, unit.secondarySkill.curCooldown);
            */
        }
        // If Relic turn
        else
        {
            //_rPassiveSkillIcon.SetSkillImage(_rPassiveSkillIcon.skillImage, )

            /*
            // Set ERelicnemy all skill CD Image Value
            _rPassiveSkillIcon.SetCDImageValue(_rPassiveSkillIcon.cdImage, unit.passiveSkill.curCooldown, unit.passiveSkill.turnCooldown);
            _rBasicSkillIcon.SetCDImageValue(_rBasicSkillIcon.cdImage, unit.basicSkill.curCooldown, unit.basicSkill.turnCooldown);
            _rPrimarySkillIcon.SetCDImageValue(_rPrimarySkillIcon.cdImage, unit.primarySkill.curCooldown, unit.primarySkill.turnCooldown);
            _rSecondarySkillIcon.SetCDImageValue(_rSecondarySkillIcon.cdImage, unit.secondarySkill.curCooldown, unit.secondarySkill.turnCooldown);

            */
        }

        //UpdateSkillInvalidImages(unit);
        //UpdateSkillManaRequired(unit);
    }

    public void UpdateSkillInvalidImages(Unit unit)
    {
        if (unit.GetUnitType() == Unit.UnitType.ALLY)
        {
            _rBasicSkillIcon.ToggleSkillInvalidImage(_rBasicSkillIcon.disabledImage, _rBasicSkillIcon.manaCostText, unit.basicSkill.manaRequired, unit.curMana);
            _rPrimarySkillIcon.ToggleSkillInvalidImage(_rPrimarySkillIcon.disabledImage, _rPrimarySkillIcon.manaCostText, unit.primarySkill.manaRequired, unit.curMana);
            _rSecondarySkillIcon.ToggleSkillInvalidImage(_rSecondarySkillIcon.disabledImage, _rSecondarySkillIcon.manaCostText, unit.secondarySkill.manaRequired, unit.curMana);
            _rAlternateSkillIcon.ToggleSkillInvalidImage(_rAlternateSkillIcon.disabledImage, _rAlternateSkillIcon.manaCostText, unit.alternateSkill.manaRequired, unit.curMana);
        }
        else
        {
            eBasicSkillIcon.ToggleSkillInvalidImage(eBasicSkillIcon.disabledImage, eBasicSkillIcon.manaCostText, unit.basicSkill.manaRequired, unit.curMana);
            ePrimarySkillIcon.ToggleSkillInvalidImage(ePrimarySkillIcon.disabledImage, ePrimarySkillIcon.manaCostText, unit.primarySkill.manaRequired, unit.curMana);
            eSecondarySkillIcon.ToggleSkillInvalidImage(eSecondarySkillIcon.disabledImage, eSecondarySkillIcon.manaCostText, unit.secondarySkill.manaRequired, unit.curMana);
            eAlternateSkillIcon.ToggleSkillInvalidImage(eAlternateSkillIcon.disabledImage, eAlternateSkillIcon.manaCostText, unit.alternateSkill.manaRequired, unit.curMana);
        }
    }

    public void SetActiveSkill(Unit unit, SkillData skillData)
    {
        if (unit.GetUnitType() == Unit.UnitType.ALLY)
        {
            // Set active skill icon
            _combatManager.activeAttackBar.SetActiveSkillImage(skillData.sprite);
            //SetActiveSkillImage();
            SetActiveSkillNameText(_rActiveSkillNameText, skillData.name);
            SetActiveSkillDescText(_rActiveSkillDescText, skillData.description);
            SetActiveSkillManaCostText(_rActiveSkillManaCost, skillData.manaRequired);
            SetActiveSkilCDText(_rActiveSkillCD, skillData.turnCooldown);
            SetActiveSkillRemainingCDText(_rActiveSkillRemaingingCDText, skillData.curCooldown);
            SetActiveSkillValue(_rActiveSkillMissHitText, skillData.missValueMultiplier);
            SetActiveSkillValue(_rActiveSkillGoodHitText, skillData.goodValueMultiplier);
            SetActiveSkillValue(_rActiveSkillGreatHitText, skillData.greatValueMultiplier);
            SetActiveSkillValue(_rActiveSkillPerfectHitText, skillData.perfectValueMultiplier);

            TogglePanel(rActiveSkillPanel, true);
        }
    }

    public void TogglePanel(GameObject go, bool enable)
    {
        go.SetActive(enable);
    }

    #region Active Skill    
    void SetActiveSkillImage(Image imageRef, Image image)
    {
        imageRef.sprite = image.sprite;
    }

    void SetActiveSkillNameText(Text text, string name)
    {
        text.text = name;
    }

    void SetActiveSkillDescText(Text text, string desc)
    {
        text.text = desc;
    }

    void SetActiveSkillManaCostText(Text text, int manaCost)
    {
        text.text = manaCost.ToString();
    }

    void SetActiveSkilCDText(Text text, int cooldown)
    {
        text.text = cooldown.ToString();
    }

    void SetActiveSkillValue(Text text, float value)
    {
        float f = value * 100;
        int val = Mathf.RoundToInt(f);
        text.text = val.ToString() + "%";
    }
    void SetActiveSkillRemainingCDImage(Image image, float curCD, float maxCD)
    {
        image.fillAmount = curCD / maxCD;
    }
    void SetActiveSkillRemainingCDText(Text text, int cooldown)
    {
        if (cooldown == 0)
            text.text = "";
        else
            text.text = cooldown.ToString();
    }
    #endregion

    public void ToggleTargetedUnitsPortrait(bool enable)
    {
        // Continue if active unit is an ally unit
        if (_combatManager.activeUnit.unitType == Unit.UnitType.ENEMY)
            return;

        if (enable)
        {
            #region Initialization
            bool allyTeam;
            if (_combatManager.activeUnit.unitType == Unit.UnitType.ALLY)
                allyTeam = true;
            else
                allyTeam = false;
            Image teamImage = allyTeam ? _allyTeamPortraitImage : _enemyTeamPortraitImage;
            #endregion

            for (int i = 0; i < _combatManager.CalculateTargetAmount(); i++)
            {
                SelectedUnitPortrait selectedUnitPortrait = _selectedUnitPortraits[i];

                if (_combatManager.activeSkill.targetsAllowed == "Enemies")
                {
                    selectedUnitPortrait.SetTargetUnitPortrait(_combatManager._enemies[i].portraitSprite, teamImage,
                    _combatManager._enemies[i].curHealth, _combatManager._enemies[i].maxHealth, _combatManager._enemies[i].curMana,
                    _combatManager._enemies[i].maxMana);
                }
                else if (_combatManager.activeSkill.targetsAllowed == "Allys")
                {
                    selectedUnitPortrait.SetTargetUnitPortrait(_combatManager.activeRelic.portraitSprite, teamImage,
                    _combatManager.activeRelic.curHealth, _combatManager.activeRelic.maxHealth, _combatManager.activeRelic.curMana,
                    _combatManager.activeRelic.maxMana);
                }
            }
        }
        else
        {
            for (int i = 0; i < _selectedUnitPortraits.Count; i++)
            {
                _selectedUnitPortraits[i].ToggleUnitPortrait(false);
            }
        }
    }
}
