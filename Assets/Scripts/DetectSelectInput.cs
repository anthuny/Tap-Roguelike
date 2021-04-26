using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetectSelectInput : MonoBehaviour
{
    private CombatManager _combatManager;
    private Image _selectionImage;
    private bool _selectionImageEnabled;
    [SerializeField] private Button _selectButton;
    private bool _selectEnabled;

    private void Awake()
    {
        _combatManager = FindObjectOfType<CombatManager>();

        _selectionImage = GetComponent<Image>();
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
        if (!_selectEnabled)
        {
            _selectEnabled = true;
            _selectionImage.enabled = true;
        }
        else
        {
            _selectEnabled = false;
            _selectionImage.enabled = false;
        }

    }
}
