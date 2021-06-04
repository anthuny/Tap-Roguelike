using UnityEngine;
using UnityEngine.UI;

public class UnitHUDInfo : MonoBehaviour
{
    private CombatManager _combatManager;
    private UIManager _uiManager;

    [Header("Target Unit Main")]
    public Unit unit;
    [SerializeField] private Text _eNameText;
    [SerializeField] private Image _eUnitPortrait;
    [SerializeField] private Text _eUnitLevelText;
    [SerializeField] private Text _eUnitHealthText;
    public Text eUnitManaText;
    [SerializeField] private Text _eUnitEnergyText;

    [Header("Target Unit Panels")]
    public GameObject eTargetUnitInfoPanel;
    public GameObject eAllSkillPanel;
    public GameObject eActiveSkillPanel;

    [Space(3)]
    [Header("Target Unit Skills")]
    public SkillIconUI tPassiveSkillIcon;
    public SkillIconUI tBasicSkillIcon;
    public SkillIconUI tPrimarySkillIcon;
    public SkillIconUI tSecondarySkillIcon;

    [Space(3)]
    [Header("Target Unit Active Skill")]
    [SerializeField] private Image _eActiveSkillImage;
    [SerializeField] private Text _eActiveSkillNameText;
    [SerializeField] private Text _eActiveSkillDescText;
    [SerializeField] private Text _eActiveSkillManaCost;
    [SerializeField] private Text _eActiveSkillCD;
    [SerializeField] private Text _eActiveSkillPerfectHitText;
    [SerializeField] private Text _eActiveSkillGreatHitText;
    [SerializeField] private Text _eActiveSkillGoodHitText;
    [SerializeField] private Text _eActiveSkillMissHitText;
    [SerializeField] private Image _eActiveSkillRemainingCDImage;
    [SerializeField] private Text _eActiveSkillRemaingingCDText;

    [Header("Relic Panels")]
    public GameObject rAllSkillPanel;
    public GameObject rActiveSkillPanel;
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

    private void Awake()
    {
        _uiManager = FindObjectOfType<UIManager>();
        _combatManager = FindObjectOfType<CombatManager>();
    }

    private void Start()
    {
        RemoveAllUI();
    }

    public void ExitSkillDetailPanel()
    {
        TogglePanel(rAllSkillPanel, true);
        TogglePanel(rActiveSkillPanel, false);
    }
    void RemoveAllUI()
    {
        TogglePanel(eTargetUnitInfoPanel, false);
        TogglePanel(eAllSkillPanel, false);
        TogglePanel(eActiveSkillPanel, false);
        TogglePanel(rAllSkillPanel, false);
        TogglePanel(rActiveSkillPanel, false);
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

    public void AssignUnitSkillsToSkillIcon()
    {
        _rPassiveSkillIcon.ReferenceUnit();
        _rBasicSkillIcon.ReferenceUnit();
        _rPrimarySkillIcon.ReferenceUnit();
        _rSecondarySkillIcon.ReferenceUnit();
    }
    public void SetValues(Unit unit)
    {
        this.unit = unit;
        // If enemy, update enemy icon unit reference
        if (unit.unitType == Unit.UnitType.ENEMY)
        {
            tPassiveSkillIcon.SetUnit(unit);
            tBasicSkillIcon.SetUnit(unit);
            tPrimarySkillIcon.SetUnit(unit);
            tSecondarySkillIcon.SetUnit(unit);

            TogglePanel(eAllSkillPanel, true);
            TogglePanel(eActiveSkillPanel, false);
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

        SetUnit(unit);
        SetSkills(unit);
    }

    public void SetUnit(Unit unit)
    {
        TogglePanel(eTargetUnitInfoPanel, true);
        SetUnitName(_eNameText, unit.name);
        SetUnitLevel(_eUnitLevelText, unit.level);
        SetUnitHealth(_eUnitHealthText, unit.curHealth);
        SetUnitMana(eUnitManaText, unit.curMana);
        SetUnitEnergy(_eUnitEnergyText, unit.turnEnergy);
    }

    void SetSkills(Unit unit)
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
            SetActiveSkillNameText(_eActiveSkillNameText, skillData.name);
            SetActiveSkillDescText(_eActiveSkillDescText, skillData.description);
            SetActiveSkillManaCostText(_eActiveSkillManaCost, skillData.manaRequired);
            SetActiveSkilCDText(_eActiveSkillCD, skillData.turnCooldown);
            SetActiveSkillRemainingCDText(_eActiveSkillRemaingingCDText, skillData.curCooldown);
            SetActiveSkillValue(_eActiveSkillMissHitText, skillData.missValueMultiplier);
            SetActiveSkillValue(_eActiveSkillGoodHitText, skillData.goodValueMultiplier);
            SetActiveSkillValue(_eActiveSkillGreatHitText, skillData.greatValueMultiplier);
            SetActiveSkillValue(_eActiveSkillPerfectHitText, skillData.perfectValueMultiplier);

            TogglePanel(eActiveSkillPanel, true);
            TogglePanel(eAllSkillPanel, false);
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
            TogglePanel(attackButton, skillData.activatable);   // Display attack button if skill is activatable
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
