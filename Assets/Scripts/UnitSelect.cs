using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelect : MonoBehaviour
{
    [SerializeField] private Image _selectImage;
    [SerializeField] private Animator _animator;

    public void ToggleTurnImage(bool toggle)
    {
        _selectImage.enabled = toggle;  // Toggle select image
        _animator.SetBool("move", toggle);  // Toggle select image animation
    }
}
