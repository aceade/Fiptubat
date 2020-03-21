using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class UnitAnimator : MonoBehaviour
{
    private Animator animator;

    void Start() {
        animator = GetComponent<Animator>();
    }

    public void Crouch() {
        // TBD
    }

    public void Vault() {
        // TBD
    }

    public void Climb() {
        // TBD
    }

    public void SetScanStatus(bool status) {
        // turrets only
        animator.SetBool("Scanning", status);
    }
}
