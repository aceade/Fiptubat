using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Extends UnitAnimator so that the logs aren't spammed with warnings 
/// about certain human-specific parameters not being set.
/// </summary>
public class HumanAnimator : UnitAnimator
{
    public override void StartMoving(){
        base.StartMoving();
        animator.SetFloat("InputMagnitude", 1f);
        animator.SetFloat("Vertical", 2f);
    }

    public override void StopMoving() {
        base.StopMoving();
        animator.SetFloat("InputMagnitude", 0f);
        animator.SetFloat("Vertical", 0f);
        animator.SetFloat("Horizontal", 0f);
    }
}
