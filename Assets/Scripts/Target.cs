using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    private CombatManager _combatManager;

    [SerializeField] private Unit unit;
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

    public void ToggleSelectionImage()
    {
        // If unit is able to be targetable a skill is active
        if (targetable && _combatManager.activeSkill)
        {
            // Clear unit select images
            _combatManager.ClearUnitSelectImages();

            // toggle this unit's select image on
            unit.ToggleSelectImage(true);

            // Declare the target in combat
            _combatManager.DeclareTarget(unit);

            // Start attack bar sequence if active unit is an ally
            if (_combatManager.activeUnit.unitType == Unit.UnitType.ALLY)
                _combatManager._unitHudInfo.StartAttack();
        }

        /*
        // If selection is not displayed, display it 
        if (!selectEnabled)
            _combatManager.AddUnitTarget(this);
        // If selection is displayed, hide it
        else
        {
            _combatManager.RemoveUnitTarget(this);
        }
        */
    }
}
