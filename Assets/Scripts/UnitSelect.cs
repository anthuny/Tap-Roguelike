using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelect : MonoBehaviour
{
    [SerializeField] private Image _selectImage;
    [SerializeField] private Animator _animator;

    private CanvasGroup _canvasGroup;
    private CombatManager _combatManager;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _combatManager = FindObjectOfType<CombatManager>();
    }
    public void ToggleSelectImage(bool enable)
    {
        _selectImage.enabled = enable;  // Toggle select image
        _animator.SetBool("move", enable);  // Toggle select image animation
    }

    public void UpdateSelectImageAlpha(bool enable)
    {
        // Set alpha to low if active skill is unable to be casted, otherwise default alpha if it can be casted
        _canvasGroup.alpha = enable ? _combatManager.unitSelectImageActiveAlpha : _combatManager.unitSelectImageInactiveAlpha;
    }
}
