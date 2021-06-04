using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    public void PlayAnimation(Animator animator, string var, bool enabled)
    {
        animator.SetBool(var, enabled);
    }
}
