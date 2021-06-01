using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoBehaviour
{
    private CombatManager _combatManager;
    private UIManager _UIManager;

    public enum SkillStatus { DISABLED, ACTIVE}
    public SkillStatus skillStatus;

    [Header("Skill Initialization")]
    [SerializeField] private SkillData _passiveSkill;
    [SerializeField] private SkillData _basicSkill;
    [SerializeField] private SkillData _primarySkill;
    [SerializeField] private SkillData _secondarySkill;
    [SerializeField] private SkillData _alternateSkill;
    [SerializeField] private SkillData _ultimateSkill;
    [Space(5)]
    [Header("Skill UI")]
    [Space(2)]
    [Header("Initialization")]
    public Target relicSkillPassiveTarget;
    public Target relicSkillBasicTarget;
    public Target relicSkillPrimaryTarget;
    public Target relicSkillSecondaryTarget;
    public Target relicSkillAlternateTarget;
    public Target relicSkillUltimateTarget;
    [Space(2)]
    [Tooltip("Text GameObject to instantiate")]
    [SerializeField] private GameObject skillUIText;
    [Tooltip("The speed at which skill UI values travel upwards")]
    public float panSpeedUI;
    [Tooltip("How long the text remains on screen before destroying")]
    public float textPanLength;
    [Tooltip("How long the text remains on screen after all text is displayed")]
    public float textBonusLength;
    [Tooltip("The difference in Y value between each skill UI value has")]
    public float yDistBetweenUI;
    public float yDistBetweenEffectUI;
    public float xDisplacementStr;
    public float SkillUIFadeOutSpeed;
    [Tooltip("The higher the value, the longer each attack value displays for after the previous")]
    public float elapsedTimeDestroyMultiplier;

    //[HideInInspector]
    public List<SkillValueUI> storedSkillValueUI = new List<SkillValueUI>();

    [Header("Active Stored Attacks")]
    public AttackData activeAttackData;

    private float textOffsetDist;
    private float yVal;

    private Target target;

    private void Awake()
    {
        _combatManager = FindObjectOfType<CombatManager>();
        _UIManager = FindObjectOfType<UIManager>();
        _UIManager.ToggleImage(_UIManager.endTurnGO, false);
    }

    public void UpdateSkillStatus(SkillStatus skillStatus)
    {
        this.skillStatus = skillStatus;

        if (skillStatus == SkillStatus.ACTIVE)
            _UIManager.ToggleImage(_UIManager.endTurnGO, false);
        else
            _UIManager.ToggleImage(_UIManager.endTurnGO, true);
    }

    // When Relic selects a skill to attack with
    public void AssignActiveSkill(Button button)
    {
        // If it's not relic's turn, exit
        if (!_combatManager.relicTurn)
            return;

        SkillData skillData = button.transform.parent.parent.GetComponent<Target>().skillData;
        AssignFirstSkill(skillData);

        // If unit has enough for the active skill, cast it
        if (_combatManager.activeUnit.HasEnoughManaForSkill())
        {
            // If the skill is not activatable, don't continue
            if (!skillData.activatable || skillData.onCooldown)
                return;

            // Update mana on first hit for skill mana cost
            StartCoroutine(_combatManager.activeUnit.UpdateCurMana(_combatManager.activeSkill.manaRequired, false));
            _combatManager.activeAttackBar.MoveAttackBar(true);
        }
        // If the unit doesnt have enough mana for active skill, dont cast skill
        else
        {

        }     
    }

    public void AssignFirstSkill(SkillData skillData)
    {
        _combatManager.relicActiveSkill = skillData;
        _combatManager.activeSkill = skillData;
    }
    public void AssignSkillAesthetics(Image skillIcon, Image skillSelectionIcon, Image skillBorderIcon,
        Color skillIconColour, Color skillSelectionColour, Color skillBorderColour)
    {
        skillIcon.color = skillIconColour;
        skillSelectionIcon.color = skillSelectionColour;
        skillBorderIcon.color = skillBorderColour;
    }

    public void InitializeSkills(Unit unit)
    {
        AssignSkills(unit);
        AssignRelicSkillTargets();
        UpdateSkillAesthetics();
    }
    public void AssignSkills(Unit unit)
    {
        _passiveSkill = unit.passiveSkill;
        _basicSkill = unit.basicSkill;
        _primarySkill = unit.primarySkill;
        _secondarySkill = unit.secondarySkill;
        _alternateSkill = unit.alternateSkill;
        _ultimateSkill = unit.ultimateSkill;
    }
    void AssignRelicSkillTargets()
    {
        relicSkillPassiveTarget.skillData = _combatManager.activeRelic.passiveSkill;
        relicSkillBasicTarget.skillData = _combatManager.activeRelic.basicSkill;
        relicSkillPrimaryTarget.skillData = _combatManager.activeRelic.primarySkill;
        relicSkillSecondaryTarget.skillData = _combatManager.activeRelic.secondarySkill;
        relicSkillAlternateTarget.skillData = _combatManager.activeRelic.alternateSkill;
        relicSkillUltimateTarget.skillData = _combatManager.activeRelic.ultimateSkill;
    }

    void UpdateSkillAesthetics()
    {
        relicSkillPassiveTarget.AssignSkillUIAesthetics();
        relicSkillBasicTarget.AssignSkillUIAesthetics();
        relicSkillPrimaryTarget.AssignSkillUIAesthetics();
        relicSkillSecondaryTarget.AssignSkillUIAesthetics();
        relicSkillAlternateTarget.AssignSkillUIAesthetics();
        relicSkillUltimateTarget.AssignSkillUIAesthetics();
    }

    public void SetActiveAttackData(AttackData attackData)
    {
        activeAttackData = attackData;
    }

    public void RemoveText(SkillValueUI skillValueUI)
    {
        storedSkillValueUI.Remove(skillValueUI);
    }
    public void HideValueUI()
    {
        for (int i = 0; i < storedSkillValueUI.Count; i++)
        {
            storedSkillValueUI[i].HideText();
        }
    }

    public void ResetTextOffset()
    {
        textOffsetDist = 0;
    }

    public void DisplaySkillValue(Unit caster, Unit target, Transform skillUIValueParent, float val = 0, AttackData attackData = null)
    {
        GameObject go = Instantiate(skillUIText, skillUIValueParent.position, Quaternion.identity);    // Initialize
        SkillValueUI skillValueUI = go.GetComponent<SkillValueUI>();    // Initialize
        target.storedskillValueUI.Add(skillValueUI);    // add skill value ui to unit for on destroy
        go.GetComponent<SkillValueUI>().skillUIManager = this;    // Initialize
        Text text = go.GetComponent<Text>();    // Initalization
        go.transform.SetParent(skillUIValueParent);    // Set parent
        skillValueUI.index = target.hitRecievedCount;

        // Set text off set 
        if (skillUIValueParent.childCount >= 2)
            textOffsetDist = skillUIValueParent.GetChild(skillUIValueParent.childCount-2).GetComponent<SkillValueUI>().CalculateDistanceTravelled();

        // Reset all texts movement again
        storedSkillValueUI.Add(skillValueUI);
        for (int i = 0; i < storedSkillValueUI.Count; i++)
        {
            storedSkillValueUI[i].EnableMoving();
        }

        Vector3 pos = go.transform.localPosition;    // Initalization
        pos.x = Random.Range(-xDisplacementStr, xDisplacementStr);    // Randomize x position

        // If value is not 0
        if (val != 0)
        {
            text.text = Mathf.Abs(val).ToString();  // Display damage

            // Set font size
            if (attackData.relicActiveSkillValueModifier == attackData.skillData.perfectValueMultiplier)
                text.fontSize = _combatManager.activeAttackBar.skillUIPerfectFontSize;
            else
                text.fontSize = _combatManager.activeAttackBar.skillUIFontSize;

            // Set text UI Colour
            if (attackData.relicActiveSkillValueModifier == attackData.skillData.perfectValueMultiplier)
                text.color = _combatManager.activeAttackBar.perfectSkillUIColour;
            else if (attackData.relicActiveSkillValueModifier == attackData.skillData.greatValueMultiplier)
                text.color = _combatManager.activeAttackBar.greatSkillUIColour;
            else if (attackData.relicActiveSkillValueModifier == attackData.skillData.goodValueMultiplier)
                text.color = _combatManager.activeAttackBar.goodSkillUIColour;
            else if (attackData.relicActiveSkillValueModifier == attackData.skillData.missValueMultiplier)
                text.color = _combatManager.activeAttackBar.missSkillUIColour;
        }
        // If value is 0
        else
        {
            text.fontSize = _combatManager.activeAttackBar.skillUIMissFontSize;     // Set font size
            text.text = "Miss";   // Display miss text
            text.color = _combatManager.activeAttackBar.missSkillUIColour;
        }

        yVal = (yDistBetweenUI + textOffsetDist) * (target.hitRecievedCount-1);
        pos.y = go.transform.localPosition.y + yVal;

        go.transform.localPosition = pos;   // Update position

        // Set canvas sorting order to always appear infront of any other skill Text UI.
        skillValueUI.canvas.sortingOrder = target.hitRecievedCount;

        // If this is the last UI of the skill on for this attack
        // Reset hit recieved count for next skill UI
        if (_combatManager.activeSkill)
        {
            if (target.hitRecievedCount == _combatManager.activeSkill.hitsRequired)
                target.hitRecievedCount = 0;
        }
    }

    public void SetSkillMaxCooldown(SkillData skillData)
    {
        skillData.curCooldown = skillData.turnCooldown;
        skillData.onCooldown = true;
    }

    public void UpdateSkillCooldown(SkillData skillData)
    {
        if (skillData.curCooldown != 0)
            skillData.curCooldown--;

        if (skillData.curCooldown == 0)
            skillData.onCooldown = false;

        //UpdateSkillCooldownVisuals(skillData);
    }

    public void UpdateSkillCooldownVisuals(SkillData skillData, Unit.UnitType unitType)
    {
        if (unitType == Unit.UnitType.ENEMY)
        {

        }
        else if (unitType == Unit.UnitType.ALLY)
        {
            switch (skillData.skillType)
            {
                case "Passive":
                    target = relicSkillPassiveTarget;
                    break;
                case "Basic":
                    target = relicSkillBasicTarget;
                    break;
                case "Primary":
                    target = relicSkillPrimaryTarget;
                    break;
                case "Secondary":
                    target = relicSkillSecondaryTarget;
                    break;
                case "Alternate":
                    target = relicSkillAlternateTarget;
                    break;
                case "Ultimate":
                    target = relicSkillUltimateTarget;
                    break;
            }

            // Loop through each relic skill to see which target to use
            if (skillData.turnCooldown == 0)
            {
                target.cooldownImage.fillAmount = 0;
                target.cooldownText.text = "";
                return;
            }

            if (skillData.curCooldown != 0)
            {
                float val = (float)skillData.curCooldown / (float)skillData.turnCooldown;
                target.cooldownImage.fillAmount = Mathf.Round(val * 100) / 100f;
                target.cooldownText.text = skillData.curCooldown.ToString();
            }
            else
            {
                target.cooldownImage.fillAmount = 0;
                target.cooldownText.text = "";
            }
        }
 
    }

    public void DecrementSkillCooldown()
    {
        // Decrease all relic's cooldown
        UpdateSkillCooldown(_passiveSkill);
        UpdateSkillCooldown(_basicSkill);
        UpdateSkillCooldown(_primarySkill);
        UpdateSkillCooldown(_secondarySkill);
        UpdateSkillCooldown(_alternateSkill);
        UpdateSkillCooldown(_ultimateSkill);
    }

    private int RoundFloatToInt(float f)
    {
        return Mathf.RoundToInt(f);
    }
}

