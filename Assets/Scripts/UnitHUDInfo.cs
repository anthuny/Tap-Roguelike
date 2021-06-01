using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHUDInfo : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private Text unitNameText;
    [SerializeField] private Image unitPortrait;
    [SerializeField] private Text unitLevelText;
    [SerializeField] private Text unitHealthText;
    [SerializeField] private Text unitManaText;
    [SerializeField] private Text unitEnergyText;

    [Space(3)]

    [Header("Skills")]
    [SerializeField] private Image passiveSkillImage;
    [SerializeField] private Text passiveSkillManaCost;
    [SerializeField] private Image passiveCDImage;
    [SerializeField] private Text passiveCDText;

    [Space(1)]
    [SerializeField] private Image basicSkillImage;
    [SerializeField] private Text basicSkillManaCost;
    [SerializeField] private Image basicCDImage;
    [SerializeField] private Text basicCDText;

    [Space(1)]
    [SerializeField] private Image primarySkillImage;
    [SerializeField] private Text primarySkillManaCost;
    [SerializeField] private Image primaryCDImage;
    [SerializeField] private Text primaryCDText;

    [Space(1)]
    [SerializeField] private Image secondarySkillImage;
    [SerializeField] private Text secondarySkillManaCost;
    [SerializeField] private Image secondaryCDImage;
    [SerializeField] private Text secondaryCDText;

    [Space(1)]
    [SerializeField] private Image alternateSkillImage;
    [SerializeField] private Text alternateSkillManaCost;
    [SerializeField] private Image alternateCDImage;
    [SerializeField] private Text alternateCDText;

    [Space(1)]
    [SerializeField] private Image ultimateSkillImage;
    [SerializeField] private Text ultimateSkillManaCost;
    [SerializeField] private Image ultimateCDImage;
    [SerializeField] private Text ultimateCDText;

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
    public void SetValues(Unit unit)
    {
        this.unit = unit;

        SetUnit();
        SetSkills();
        SetActiveSkill();
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
        //SetSkillImage(unit.passiveSkill.)
        //SetSkillImage(unit.passiveSkill.)
        //SetSkillImage(unit.passiveSkill.)
        //SetSkillImage(unit.passiveSkill.)
        //SetSkillImage(unit.passiveSkill.)
        //SetSkillImage(unit.passiveSkill.)

        SetSkillManaCost(passiveSkillManaCost, unit.passiveSkill.manaRequired);
        SetSkillManaCost(basicSkillManaCost, unit.basicSkill.manaRequired);
        SetSkillManaCost(primarySkillManaCost, unit.primarySkill.manaRequired);
        SetSkillManaCost(secondarySkillManaCost, unit.secondarySkill.manaRequired);
        SetSkillManaCost(alternateSkillManaCost, unit.alternateSkill.manaRequired);
        SetSkillManaCost(ultimateSkillManaCost, unit.ultimateSkill.manaRequired);

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

    void SetActiveSkill()
    {
        if (unit.activeSkill != null)
        {
            //SetActiveSkillImage();
            SetActiveSkillNameText(unit.activeSkill.name);
            SetActiveSkillDescText(unit.activeSkill.description);
            SetActiveSkillManaCostText(unit.activeSkill.manaRequired);
            SetActiveSkilCDText(unit.activeSkill.turnCooldown);
            SetActiveSkillValue(unit.activeSkill.goodValueMultiplier);
            SetActiveSkillRemainingCDText(unit.activeSkill.curCooldown);
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

    void SetSkillManaCost(Text text, int manaCost)
    {
        text.text = manaCost.ToString();
    }

    void SetCDImage(Image image, float curCD, float maxCD)
    {
        image.fillAmount = curCD / maxCD;
    }

    void SetCDText(Text text, int cooldown)
    {
        if (cooldown == 0)
            passiveCDText.text = "";
        else
            passiveCDText.text = cooldown.ToString();
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
