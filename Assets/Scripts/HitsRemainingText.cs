using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitsRemainingText : MonoBehaviour
{
    [SerializeField] private Text _hitsRemainingText;
    private float val;

    /// <summary>a
    /// Update Remaining Hit Text
    /// </summary>
    /// <param name="val"></param>
    public void UpdateRemainingHitText(bool cond, int val = 0, int isoVal = 0)
    {
        this.val += val;
        if (cond)
            this.val = isoVal;

        if (isoVal == 0 && val == 0)
            _hitsRemainingText.text = "";
        else
            _hitsRemainingText.text = this.val.ToString();
    }
}
