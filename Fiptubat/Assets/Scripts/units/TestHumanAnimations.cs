using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A test class for debugging animations
/// </summary>
public class TestHumanAnimations : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        AttackCycle();
        Invoke("Crouch", 1f);
        Invoke("AttackCycle", 11f);
        Invoke("StandUp", 15f);
    }

    void PerformMoveCycle() {
        animator.SetFloat("InputMagnitude", 1f);
    }

    void AttackCycle() {
        Invoke("StartAiming", 1f);
        Invoke("FullAim", 3f);
        Invoke("Fire", 5f);
        Invoke("Reload", 7f);
        Invoke("StopAiming", 9f);
    }

    void StartAiming() {
        animator.SetFloat("Aim", 0.5f);
    }

    void FullAim() {
        animator.SetFloat("Aim", 1f);
    }

    void StopAiming() {
        animator.SetFloat("Aim", 0f);
    }

    void Fire() {
        animator.SetTrigger("Fire");
        animator.ResetTrigger("Reload");
    }

    void Reload() {
        animator.SetTrigger("Reload");
        animator.ResetTrigger("Fire");
    }

    void Crouch() {
        animator.SetTrigger("Crouch");
        animator.ResetTrigger("Stand");
    }

    void StandUp() {
        animator.SetTrigger("Stand");
        animator.ResetTrigger("Crouch");
    }

    
}
