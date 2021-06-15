using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedUnitPortrait : MonoBehaviour
{
    [SerializeField] private Image _portraitImage;
    [SerializeField] private Image _teamImage;

    [Space(1)]

    [SerializeField] private GameObject parent;
    [SerializeField] private Image _healthImage;
    [SerializeField] private Image _manaImage;
    [SerializeField] private GameObject _healthImagePar;
    [SerializeField] private GameObject _manaImagePar;

    private void Awake()
    {
        ToggleUnitPortrait(false);
    }
    public void SetTargetUnitPortrait(Sprite portraitSprite, Image teamImage, float curHealth, float maxHealth, float curMana, float maxMana)
    {
        SetPortraitImage(portraitSprite);
        SetTeamImage(teamImage);
        SetHealthImage(curHealth, maxHealth);
        SetManaHealth(curMana, maxMana);

        ToggleUnitPortrait(true);
    }

    void SetPortraitImage(Sprite portraitSprite)
    {
        _portraitImage.sprite = portraitSprite;
    }

    void SetTeamImage(Image teamImage)
    {
        _teamImage.sprite = teamImage.sprite;
    }

    void SetHealthImage(float curHealth, float maxHealth)
    {
        _healthImage.fillAmount = curHealth / maxHealth;
    }

    void SetManaHealth(float curMana, float maxMana)
    {
        _manaImage.fillAmount = curMana / maxMana;
    }

    public void ToggleUnitPortrait(bool enable)
    {
        _portraitImage.enabled = enable;
        _teamImage.enabled = enable;
        _healthImagePar.SetActive(enable);
        _manaImagePar.SetActive(enable);

        parent.SetActive(enable);
    }
}
