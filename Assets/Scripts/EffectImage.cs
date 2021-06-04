using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectImage : MonoBehaviour
{
    public Image effectImage;
    public int effectPower;
    public int effectDuration;
    [SerializeField] private Text effectPowerText;
    [HideInInspector]
    public Unit unit;
    [HideInInspector]
    public Effect effect;

    public void UpdateEffectPower(int effectPower)
    {
        this.effectPower += effectPower;
        effectPowerText.text = this.effectPower.ToString();
    }

    public void UpdateEffectDuration(int effectDuration)
    {
        this.effectDuration = effectDuration;
    }

    public void ToggleEffectImage(bool cond)
    {
        effectImage.enabled = cond;
    }

    public void ToggleEffectPowerText(bool cond)
    {
        effectPowerText.enabled = cond;
    }

    public void SetEffectImage(Sprite sprite)
    {
        effectImage.sprite = sprite;
    }

    public void UpdateEffectImageColour()
    {
        effectImage.color = effect.effectColor;
    }

    // Do the functionality of an effect
    public void Functionality(SkillData skillData)
    {
        //  Determine the type of effect
        switch (skillData.effect.effectType)
        {
            case "RecievedDamageAmp":
                unit.recievedDamageAmp = effectPower * skillData.effect.stackValue;
                break;

            case "Curse":
                break;
        }
    }
}
