using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    private CombatManager _combatManager;

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

    [HideInInspector]
    public bool targetable;

    private void Awake()
    {
        _combatManager = FindObjectOfType<CombatManager>();
        selectionImage = GetComponent<Image>();
    }

    private void Start()
    {
        //_selectButton.onClick.AddListener(ToggleSelectionImage); // Add a listener to the button on a unit    
    }

    public void ToggleSelectionImage(bool ifTargetIsMultiple)
    {
        // If unit is able to be targetable and a skill is active
        if (targetable && _combatManager.activeSkill)
        {
            // Clear unit select images
            _combatManager.ClearUnitSelectImages();

            // Add target to the selected targets
            _combatManager.AddTarget(unit);

            // If active skill target type is multiple
            if (ifTargetIsMultiple)
            {
                // Add all other enemies to the selected targets
                if (_combatManager.activeSkill.targetType == "Multiple")
                    if (_combatManager.activeUnit.unitType == Unit.UnitType.ALLY)
                        for (int i = 0; i < _combatManager._enemies.Count; i++)
                            _combatManager._enemies[i].target.ToggleSelectionImage(false);
            }
            // Update Unit's mana for skill cost
            StartCoroutine(_combatManager.activeUnit.UpdateCurMana(_combatManager.activeSkill.manaRequired, false));

            // Toggle off selected skill image
            _combatManager._unitHudInfo.ToggleSkillSelectionImage(_combatManager.activeSkill, false);

            // Prepare attack bar opening sequence
            StartCoroutine(_combatManager.activeAttackBar.PrepareAttackBarOpen());
        }
    }
}
