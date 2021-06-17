using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillIconUI : MonoBehaviour
{
    public enum SkillType { BASIC, PRIMARY, SECONDARY, ALTERNATE}
    public SkillType skillType;
    [SerializeField] private Image selectionImage;
    public Image skillImage;
    public Image disabledImage;
    public Text manaCostText;
    public Button button;
    private UnitHUDInfo _unitHudInfo;
    public Unit unit;
    public SkillData skillData;

    private CombatManager _combatManager;

    private void Awake()
    {
        _unitHudInfo = FindObjectOfType<UnitHUDInfo>();
        _combatManager = FindObjectOfType<CombatManager>();
    }

    public void ReferenceUnit()
    {
        _combatManager = FindObjectOfType<CombatManager>();
        switch (skillType)
        {
            case SkillType.BASIC:
                skillData = _combatManager.activeUnit.basicSkill;
                break;
            case SkillType.PRIMARY:
                skillData = _combatManager.activeUnit.primarySkill;
                break;
            case SkillType.SECONDARY:
                skillData = _combatManager.activeUnit.secondarySkill;
                break;
            case SkillType.ALTERNATE:
                skillData = _combatManager.activeUnit.alternateSkill;
                break;
        }

        unit = _combatManager.activeUnit;
    }
    public void ActiveSkillToggleUI()
    {
        // Sets skill active as true
        _combatManager.activeAttackBar.UpdateActiveSkill(true);

        // Set active skill
        _unitHudInfo = FindObjectOfType<UnitHUDInfo>();
        _unitHudInfo.SetActiveSkill(unit, skillData);
        _combatManager.SetActiveSkill(skillData);

        // toggle correct unit select images based onskill
        _combatManager.ToggleUnitSelectImages(_combatManager.activeSkill);

        _unitHudInfo.DisableAllSkillSelectionImages();  // Disable all skill selection images

        ToggleSelectionImage(true);  // Toggle selection image of selected skill icon
    }

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
    }

    #region Skill Properties

    public void ToggleSelectionImage(bool enable)
    {
        selectionImage.enabled = enable;
    }

    public void SetSkillImage(Image imageRef, Image image)
    {
        imageRef.sprite = image.sprite;
    }

    public void ToggleSkillInvalidImage(Image image, Text text, int requiredMana, int curMana)
    {
        if (curMana >= requiredMana)
        {
            image.enabled = false;
            SetSkillManaCost(false, text, requiredMana, curMana);
        }
        else
        {
            image.enabled = true;
            SetSkillManaCost(true, text, requiredMana, curMana);
        }
    }

    public void ToggleSkillSelection(Image image, bool enable)
    {
        image.enabled = enable;
    }

    public void SetSkillManaCost(bool disableImageEnabled, Text text, int manaCost = 0, int curMana = 0)
    {
        if (!disableImageEnabled)
            text.text = "";
        else
            text.text = curMana.ToString() + "/" + manaCost.ToString();
    }

    public void SetCDImageValue(Image image, float curCD, float maxCD)
    {
        if (curCD != 0)
            image.fillAmount = curCD / maxCD;
        else
            image.fillAmount = 0;
    }

    public void SetCDText(Text text, int cooldown)
    {
        if (cooldown == 0)
            text.text = "";
        else
            text.text = cooldown.ToString();
    }
    #endregion
}


