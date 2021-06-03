using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHUDInfo : MonoBehaviour
{
    private CombatManager _combatManager;
    private UIManager _uiManager;

    [Header("Main")]
    [SerializeField] private Text unitNameText;
    [SerializeField] private Image unitPortrait;
    [SerializeField] private Text unitLevelText;
    [SerializeField] private Text unitHealthText;
    [SerializeField] private Text unitManaText;
    [SerializeField] private Text unitEnergyText;

    [Space(1)]

    [Header("Panels")]
    [SerializeField] private Image allSkillPanel;
    [SerializeField] private Image activeSkillPanel;

    [Space(3)]

    [Header("Skills")]
    [SerializeField] private Image passiveSkillImage;
    [SerializeField] private Image passiveCDImage;
    [SerializeField] private Image passiveDisabledImage;
    [SerializeField] private Text passiveCDText;
    [SerializeField] private Button passiveButton;

    [Space(1)]
    [SerializeField] private Image basicSkillImage;
    [SerializeField] private Image basicCDImage;
    [SerializeField] private Image basicDisabledImage;
    [SerializeField] private Text basicCDText;
    [SerializeField] private Button basicButton;

    [Space(1)]
    [SerializeField] private Image primarySkillImage;
    [SerializeField] private Image primaryCDImage;
    [SerializeField] private Image primaryDisabledImage;
    [SerializeField] private Text primaryCDText;
    [SerializeField] private Button primaryButton;

    [Space(1)]
    [SerializeField] private Image secondarySkillImage;
    [SerializeField] private Image secondaryCDImage;
    [SerializeField] private Image secondaryDisabledImage;
    [SerializeField] private Text secondaryCDText;
    [SerializeField] private Button secondaryButton;

    [Space(1)]
    [SerializeField] private Image alternateSkillImage;
    [SerializeField] private Image alternateCDImage;
    [SerializeField] private Image alternateDisabledImage;
    [SerializeField] private Text alternateCDText;
    [SerializeField] private Button alternateButton;

    [Space(1)]
    [SerializeField] private Image ultimateSkillImage;
    [SerializeField] private Image ultimateCDImage;
    [SerializeField] private Image ultimateDisabledImage;
    [SerializeField] private Text ultimateCDText;
    [SerializeField] private Button ultimateButton;

    [Space(3)]

    [Header("Actve Skill")]
    [SerializeField] private Image activeSkillImage;
    [SerializeField] private Text activeSkillNameText;
    [SerializeField] private Text activeSkillDescText;
    [SerializeField] private Text activeSkillManaCost;
    [SerializeField] private Text activeSkillCD;
    [SerializeField] private Text activeSkillValue;
    [SerializeField] private Image activeSkillRemainingCDImage;
    [SerializeField] private Text activeSkillRemaingingCDText;

    [Space(1)]

    private Unit unit;

    private void Awake()
    {
        _uiManager = FindObjectOfType<UIManager>();
        _combatManager = FindObjectOfType<CombatManager>();
    }

    private void Start()
    {
        passiveButton.onClick.AddListener(SetActiveSkill); // Add skill button listener
        basicButton.onClick.AddListener(SetActiveSkill); // Add skill button listener
        primaryButton.onClick.AddListener(SetActiveSkill); // Add skill button listener
        secondaryButton.onClick.AddListener(SetActiveSkill); // Add skill button listener
        alternateButton.onClick.AddListener(SetActiveSkill); // Add skill button listener
        ultimateButton.onClick.AddListener(SetActiveSkill); // Add skill button listener
    }
    public void SetValues(Unit unit)
    {
        this.unit = unit;

        SetUnit();
        SetSkills();
        ToggleUnitHUDUI(true, _combatManager.swapTeamUnitMovespeed);
    }

    public IEnumerator ToggleUnitHUDUI(bool enable, float time)
    {
        yield return new WaitForSeconds(time);
        StartCoroutine(_uiManager.ToggleImage(_uiManager.enemySkillUIGO, enable));
    }
    void SetUnit()
    {
        SetUnitName(unit.name);
        //SetUnitPortrait();
        SetUnitLevel(unit.level);
        SetUnitHealth(unit.curHealth);
        SetUnitMana(unit.curMana);
        SetUnitEnergy(unit.turnEnergy);
    }

    void SetSkills()
    {
        ToggleSkillInvalidImage(passiveDisabledImage, unit.passiveSkill.manaRequired);
        ToggleSkillInvalidImage(basicDisabledImage, unit.basicSkill.manaRequired);
        ToggleSkillInvalidImage(primaryDisabledImage, unit.primarySkill.manaRequired);
        ToggleSkillInvalidImage(secondaryDisabledImage, unit.secondarySkill.manaRequired);
        ToggleSkillInvalidImage(alternateDisabledImage, unit.alternateSkill.manaRequired);
        ToggleSkillInvalidImage(ultimateDisabledImage, unit.ultimateSkill.manaRequired);

        SetCDImage(passiveCDImage, unit.passiveSkill.curCooldown, unit.passiveSkill.turnCooldown);
        SetCDImage(basicCDImage, unit.basicSkill.curCooldown, unit.basicSkill.turnCooldown);
        SetCDImage(primaryCDImage, unit.primarySkill.curCooldown, unit.primarySkill.turnCooldown);
        SetCDImage(secondaryCDImage, unit.secondarySkill.curCooldown, unit.secondarySkill.turnCooldown);
        SetCDImage(alternateCDImage, unit.alternateSkill.curCooldown, unit.alternateSkill.turnCooldown);
        SetCDImage(ultimateCDImage, unit.ultimateSkill.curCooldown, unit.ultimateSkill.turnCooldown);

        SetCDText(passiveCDText, unit.passiveSkill.curCooldown);
        SetCDText(basicCDText, unit.basicSkill.curCooldown);
        SetCDText(primaryCDText, unit.primarySkill.curCooldown);
        SetCDText(secondaryCDText, unit.secondarySkill.curCooldown);
        SetCDText(alternateCDText, unit.alternateSkill.curCooldown);
        SetCDText(ultimateCDText, unit.ultimateSkill.curCooldown);
    }

    public void SetActiveSkill()
    {
        if (unit.activeSkill != null)
        {
            SetActiveSkillNameText(unit.activeSkill.name);
            SetActiveSkillDescText(unit.activeSkill.description);
            SetActiveSkillManaCostText(unit.activeSkill.manaRequired);
            SetActiveSkilCDText(unit.activeSkill.turnCooldown);
            SetActiveSkillValue(unit.activeSkill.goodValueMultiplier);
            SetActiveSkillRemainingCDText(unit.activeSkill.curCooldown);

            TogglePanel(activeSkillPanel, true);
            TogglePanel(allSkillPanel, false);
            //SetActiveSkillImage();
        }
    }

    #region Main Properties
    void SetUnitName(string name)
    {
        unitNameText.text = name;
    }

    void SetUnitPortrait(Image image)
    {
        unitPortrait = image;
    }

    void SetUnitLevel(int level)
    {
        unitLevelText.text = level.ToString();
    }

    void SetUnitHealth(float health)
    {
        int val = Mathf.RoundToInt(health);
        unitHealthText.text = val.ToString();
    }

    void SetUnitMana(int mana)
    {
        unitManaText.text = mana.ToString();
    }

    void SetUnitEnergy(int energy)
    {
        unitEnergyText.text = energy.ToString();
    }
    #endregion

    #region Skill Properties
    void SetSkillImage(Image image)
    {
        passiveSkillImage.sprite = image.sprite;
    }

    void TogglePanel(Image image, bool enable)
    {
        image.enabled = enable;
    }

    void ToggleSkillInvalidImage(Image image, float requiredMana)
    {
        if (unit.curMana <= requiredMana)
            image.enabled = false;
        else
            image.enabled = true;
    }

    public void ToggleSkillSelection(Image image, bool enable)
    {
        image.enabled = enable;
    }

    void SetSkillManaCost(Text text, int manaCost)
    {
        if (manaCost != 0)
            text.text = manaCost.ToString();
        else
            text.text = "";
    }

    void SetCDImage(Image image, float curCD, float maxCD)
    {
        image.fillAmount = curCD / maxCD;
    }

    void SetCDText(Text text, int cooldown)
    {
        if (cooldown == 0)
            text.text = "";
        else
            text.text = cooldown.ToString();
    }
    #endregion

    #region Active Skill    
    void SetActiveSkillImage(Image image)
    {
        activeSkillImage.sprite = image.sprite;
    }

    void SetActiveSkillNameText(string name)
    {
        activeSkillNameText.text = name;
    }

    void SetActiveSkillDescText(string desc)
    {
        activeSkillDescText.text = desc;
    }

    void SetActiveSkillManaCostText(int manaCost)
    {
        activeSkillManaCost.text = manaCost.ToString();
    }

    void SetActiveSkilCDText(int cooldown)
    {
        activeSkillCD.text = cooldown.ToString();
    }

    void SetActiveSkillValue(float value)
    {
        int val = Mathf.RoundToInt(value);
        activeSkillValue.text = val.ToString() + " %";
    }
    void SetActiveSkillRemainingCDImage(Image image, float curCD, float maxCD)
    {
        image.fillAmount = curCD / maxCD;
    }
    void SetActiveSkillRemainingCDText(int cooldown)
    {
        activeSkillRemaingingCDText.text = cooldown.ToString();

        SetActiveSkillRemainingCDImage(activeSkillRemainingCDImage, unit.activeSkill.curCooldown, unit.activeSkill.turnCooldown);
    }
    #endregion
}
