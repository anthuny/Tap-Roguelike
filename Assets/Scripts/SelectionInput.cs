using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionInput : MonoBehaviour
{
    [SerializeField] private Button _selectButton;
    private CombatManager _combatManager;
    [HideInInspector]
    public bool selectEnabled;
    [HideInInspector]
    public Image selectionImage;

    private void Awake()
    {
        _combatManager = FindObjectOfType<CombatManager>();
        selectionImage = GetComponent<Image>();
    }

    private void Start()
    {
        // Add a listener to the button on a unit
        _selectButton.onClick.AddListener(ToggleSelectionImage);
    }

    /// <summary>
    /// Toggle the selection image
    /// </summary>
    private void ToggleSelectionImage()
    {
        if (!selectEnabled)
        {
            _combatManager.ManageSelectionCount(true, this);
        }
        else
        {
            _combatManager.ManageSelectionCount(false, this);
        }

    }
}
