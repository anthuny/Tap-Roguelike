using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitsRemainingText : MonoBehaviour
{
    [SerializeField] private Text _hitsRemainingText;

    /// <summary>a
    /// Update Remaining Hit Text
    /// </summary>
    /// <param name="val"></param>
    public void UpdateRemainingHitText(bool cond, int val = 0)
    {
        int finalVal = Mathf.Abs(val);  // Ensure hit text is a positive value
        _hitsRemainingText.text = finalVal.ToString();  // Set text
    }
}
