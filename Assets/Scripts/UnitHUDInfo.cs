using UnityEngine;
using UnityEngine.UI;

public class UnitHUDInfo : MonoBehaviour
{
    private CombatManager _combatManager;
    private UIManager _uiManager;

    [Header("Target Unit Main")]
    public GameObject cancelAttackGO;
    [SerializeField] private GameObject attackBarDarkenerGO;
    [SerializeField] private Text _tNameText;
    [SerializeField] private Image _tUnitPortrait;
    [SerializeField] private Text _tUnitLevelText;
    [SerializeField] private Text _tUnitHealthText;
    public Text tUnitManaText;
    [SerializeField] private Text _tUnitEnergyText;

    [Header("Target Unit Panels")]
    public GameObject tTargetUnitInfoPanel;
    public GameObject tAllSkillPanel;
    public GameObject tActiveSkillPanel;

    [Space(3)]
    [Header("Target Unit Skills")]
    public SkillIconUI tPassiveSkillIcon;
    public SkillIconUI tBasicSkillIcon;
    public SkillIconUI tPrimarySkillIcon;
    public SkillIconUI tSecondarySkillIcon;

    [Space(3)]
    [Header("Target Unit Active Skill")]
    [SerializeField] private Image _tActiveSkillImage;
    [SerializeField] private Text _tActiveSkillNameText;
    [SerializeField] private Text _tActiveSkillDescText;
    [SerializeField] private Text _tActiveSkillManaCost;
    [SerializeField] private Text _tActiveSkillCD;
    [SerializeField] private Text _tActiveSkillPerfectHitText;
    [SerializeField] private Text _tActiveSkillGreatHitText;
    [SerializeField] private Text _tActiveSkillGoodHitText;
    [SerializeField] private Text _tActiveSkillMissHitText;
    [SerializeField] private Image _tActiveSkillRemainingCDImage;
    [SerializeField] private Text _tActiveSkillRemainingCDText;

    [Header("Relic Panels")]
    public GameObject rAllSkillPanel;
    public GameObject rActiveSkillPanel;
    public GameObject rAttackBarPanel;
    public GameObject attackButton;

    [Space(3)]
    [Header("Relic Skills")]
    [SerializeField] private SkillIconUI _rPassiveSkillIcon;
    [SerializeField] private SkillIconUI _rBasicSkillIcon;
    [SerializeField] private SkillIconUI _rPrimarySkillIcon;
    [SerializeField] private SkillIconUI _rSecondarySkillIcon;

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

    [HideInInspector]
    public Unit unit;

    private void Start()
    {
        _uiManager = FindObjectOfType<UIManager>();
        _combatManager = FindObjectOfType<CombatManager>();

        RemoveAllUI();
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
        TogglePanel(tTargetUnitInfoPanel, false);
        TogglePanel(tAllSkillPanel, false);
        TogglePanel(tActiveSkillPanel, false);
        TogglePanel(rAllSkillPanel, false);
        TogglePanel(rActiveSkillPanel, false);
        TogglePanel(attackBarDarkenerGO, false);
    }
    public void ToggleSkillIconSelection(SkillIconUI skillIcon, bool enable)
    {
        skillIcon.ToggleSelectionImage(enable);
    }

    public void DeselectAllSelections()
    {
        tPassiveSkillIcon.ToggleSelectionImage(false);
        tBasicSkillIcon.ToggleSelectionImage(false);
        tPrimarySkillIcon.ToggleSelectionImage(false);
        tSecondarySkillIcon.ToggleSelectionImage(false);
    }

    public void AssignUnitSkillsToSkillIcon(Unit unit)
    {
        if (unit.unitType == Unit.UnitType.ALLY)
        {
            _rPassiveSkillIcon.ReferenceUnit();
            _rBasicSkillIcon.ReferenceUnit();
            _rPrimarySkillIcon.ReferenceUnit();
            _rSecondarySkillIcon.ReferenceUnit();
        }
        else
        {
            tPassiveSkillIcon.ReferenceUnit();
            tBasicSkillIcon.ReferenceUnit();
            tPrimarySkillIcon.ReferenceUnit();
            tSecondarySkillIcon.ReferenceUnit();
        }
    }
    public void TogglePanels(Unit unit)
    {
        this.unit = unit;
        // If enemy, update enemy icon unit reference
        if (unit.unitType == Unit.UnitType.ENEMY)
        {
            tPassiveSkillIcon.SetUnit(unit);
            tBasicSkillIcon.SetUnit(unit);
            tPrimarySkillIcon.SetUnit(unit);
            tSecondarySkillIcon.SetUnit(unit);

            TogglePanel(tAllSkillPanel, true);
            TogglePanel(tActiveSkillPanel, false);
        }
        // If enemy, update relic icon unit reference
        else
        {
            _rPassiveSkillIcon.SetUnit(unit);
            _rBasicSkillIcon.SetUnit(unit);
            _rPrimarySkillIcon.SetUnit(unit);
            _rSecondarySkillIcon.SetUnit(unit);

            TogglePanel(rAllSkillPanel, true);
            TogglePanel(rActiveSkillPanel, false);
        }

        //SetUnitInfoUI(unit);
        SetUnitAllSkills(unit);
    }

    public void SetUnitInfoUI(Unit unit)
    {
        TogglePanel(tTargetUnitInfoPanel, true);
        SetUnitName(_tNameText, unit.name);
        SetUnitLevel(_tUnitLevelText, unit.level);
        SetUnitHealth(_tUnitHealthText, unit.curHealth);
        SetUnitMana(tUnitManaText, unit.curMana);
        SetUnitEnergy(_tUnitEnergyText, unit.turnEnergy);
    }

    void SetUnitAllSkills(Unit unit)
    {
        // If Enemy turn
        if (!_combatManager.relicTurn)
        {
            //_ePassiveSkillIcon.SetSkillImage(_ePassiveSkillIcon.skillImage, )

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
        }
        // If Relic turn
        else
        {
            //_rPassiveSkillIcon.SetSkillImage(_rPassiveSkillIcon.skillImage, )

            // Set ERelicnemy all skill CD Image Value
            _rPassiveSkillIcon.SetCDImageValue(_rPassiveSkillIcon.cdImage, unit.passiveSkill.curCooldown, unit.passiveSkill.turnCooldown);
            _rBasicSkillIcon.SetCDImageValue(_rBasicSkillIcon.cdImage, unit.basicSkill.curCooldown, unit.basicSkill.turnCooldown);
            _rPrimarySkillIcon.SetCDImageValue(_rPrimarySkillIcon.cdImage, unit.primarySkill.curCooldown, unit.primarySkill.turnCooldown);
            _rSecondarySkillIcon.SetCDImageValue(_rSecondarySkillIcon.cdImage, unit.secondarySkill.curCooldown, unit.secondarySkill.turnCooldown);

            // Update Relic all skill cd image value
            _rPassiveSkillIcon.SetCDImageValue(_rPassiveSkillIcon.cdImage, unit.passiveSkill.curCooldown, unit.passiveSkill.turnCooldown);
            _rBasicSkillIcon.SetCDImageValue(_rBasicSkillIcon.cdImage, unit.basicSkill.curCooldown, unit.basicSkill.turnCooldown);
            _rPrimarySkillIcon.SetCDImageValue(_rPrimarySkillIcon.cdImage, unit.primarySkill.curCooldown, unit.primarySkill.turnCooldown);
            _rSecondarySkillIcon.SetCDImageValue(_rSecondarySkillIcon.cdImage, unit.secondarySkill.curCooldown, unit.secondarySkill.turnCooldown);

            // Update Relic all skill cd text value
            _rPassiveSkillIcon.SetCDText(_rPassiveSkillIcon.cdText, unit.passiveSkill.curCooldown);
            _rBasicSkillIcon.SetCDText(_rBasicSkillIcon.cdText, unit.basicSkill.curCooldown);
            _rPrimarySkillIcon.SetCDText(_rPrimarySkillIcon.cdText, unit.primarySkill.curCooldown);
            _rSecondarySkillIcon.SetCDText(_rSecondarySkillIcon.cdText, unit.secondarySkill.curCooldown);
        }

        UpdateSkillInvalidImages(unit);
        //UpdateSkillManaRequired(unit);
    }

    public void UpdateSkillInvalidImages(Unit unit)
    {
        tPassiveSkillIcon.ToggleSkillInvalidImage(tPassiveSkillIcon.disabledImage, tPassiveSkillIcon.manaCostText, unit.passiveSkill.manaRequired, unit.curMana);
        tBasicSkillIcon.ToggleSkillInvalidImage(tBasicSkillIcon.disabledImage, tBasicSkillIcon.manaCostText, unit.basicSkill.manaRequired, unit.curMana);
        tPrimarySkillIcon.ToggleSkillInvalidImage(tPrimarySkillIcon.disabledImage, tPrimarySkillIcon.manaCostText, unit.primarySkill.manaRequired, unit.curMana);
        tSecondarySkillIcon.ToggleSkillInvalidImage(tSecondarySkillIcon.disabledImage, tSecondarySkillIcon.manaCostText, unit.secondarySkill.manaRequired, unit.curMana);
    }

    public void UpdateSkillManaRequired(Unit unit)
    {
        // If the skill Icon's disabled image is enabled, display the mana UI
        if (tPassiveSkillIcon.disabledImage.enabled)
            tPassiveSkillIcon.SetSkillManaCost(true, tPassiveSkillIcon.manaCostText, unit.passiveSkill.manaRequired, unit.curMana);
        else   // If the skill Icon's disabled image is disabled, hide the mana UI
            tPassiveSkillIcon.SetSkillManaCost(false, tPassiveSkillIcon.manaCostText);

        // If the skill Icon's disabled image is enabled, display the mana UI
        if (tBasicSkillIcon.disabledImage.enabled)
            tBasicSkillIcon.SetSkillManaCost(true, tBasicSkillIcon.manaCostText, unit.basicSkill.manaRequired, unit.curMana);
        else   // If the skill Icon's disabled image is disabled, hide the mana UI
            tPassiveSkillIcon.SetSkillManaCost(false, tBasicSkillIcon.manaCostText);

        // If the skill Icon's disabled image is enabled, display the mana UI
        if (tPrimarySkillIcon.disabledImage.enabled)
            tPrimarySkillIcon.SetSkillManaCost(true, tPrimarySkillIcon.manaCostText, unit.primarySkill.manaRequired, unit.curMana);
        else   // If the skill Icon's disabled image is disabled, hide the mana UI
            tPassiveSkillIcon.SetSkillManaCost(false, tPrimarySkillIcon.manaCostText);

        // If the skill Icon's disabled image is enabled, display the mana UI
        if (tSecondarySkillIcon.disabledImage.enabled)
            tSecondarySkillIcon.SetSkillManaCost(true, tSecondarySkillIcon.manaCostText, unit.secondarySkill.manaRequired, unit.curMana);
        else   // If the skill Icon's disabled image is disabled, hide the mana UI
            tPassiveSkillIcon.SetSkillManaCost(false, tSecondarySkillIcon.manaCostText);
    }
    public void SetActiveSkill(Unit unit, SkillData skillData)
    {
        if (unit.unitType == Unit.UnitType.ENEMY)
        {
            //SetActiveSkillImage();
            SetActiveSkillNameText(_tActiveSkillNameText, skillData.name);
            SetActiveSkillDescText(_tActiveSkillDescText, skillData.description);
            SetActiveSkillManaCostText(_tActiveSkillManaCost, skillData.manaRequired);
            SetActiveSkilCDText(_tActiveSkillCD, skillData.turnCooldown);
            SetActiveSkillRemainingCDText(_tActiveSkillRemainingCDText, skillData.curCooldown);
            SetActiveSkillValue(_tActiveSkillMissHitText, skillData.missValueMultiplier);
            SetActiveSkillValue(_tActiveSkillGoodHitText, skillData.goodValueMultiplier);
            SetActiveSkillValue(_tActiveSkillGreatHitText, skillData.greatValueMultiplier);
            SetActiveSkillValue(_tActiveSkillPerfectHitText, skillData.perfectValueMultiplier);

            TogglePanel(tActiveSkillPanel, true);
            TogglePanel(tAllSkillPanel, false);
        }
        else
        {
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
            TogglePanel(rAllSkillPanel, false);
            //TogglePanel(attackButton, skillData.activatable);   // Display attack button if skill is activatable
        }
    }

    public void TogglePanel(GameObject go, bool enable)
    {
        go.SetActive(enable);
    }

    #region Main Properties
    void SetUnitName(Text text, string name)
    {
        text.text = name;
    }

    void SetUnitPortrait(Image imageRef, Image image)
    {
        imageRef.sprite = image.sprite;
    }

    void SetUnitLevel(Text text, int level)
    {
        text.text = level.ToString();
    }

    void SetUnitHealth(Text text, float health)
    {
        int val = Mathf.RoundToInt(health);
        text.text = val.ToString();
    }

    public void SetUnitMana(Text text, int mana)
    {
        text.text = mana.ToString();
    }

    void SetUnitEnergy(Text text, int energy)
    {
        text.text = energy.ToString();
    }
    #endregion

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
        text.text = val.ToString() + " %";
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
}
