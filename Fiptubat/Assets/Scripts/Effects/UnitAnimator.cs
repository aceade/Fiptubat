using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

/// <summary>
/// General animator for units. Will <strong>NOT</strong> throw exceptions if the unit cannot obey -
/// it is up to the BaseUnit class to prevent this being called.
/// </summary>
public class UnitAnimator : MonoBehaviour
{
    private Animator animator;

    private Collider myCollider;

    void Start() {
        animator = GetComponentInChildren<Animator>();
        myCollider = GetComponent<Collider>();
    }

    /// <summary>
    /// Used by humanoid units to crouch. Currently based on changing collider heights.
    /// TODO: add colliders based on bones
    /// </summary>
    /// <param name="crouched"></param>
    public void Crouch(bool crouched) {
        if (crouched) {
            animator.SetTrigger("Crouch");
            animator.ResetTrigger("Stand");
			GetComponent<CapsuleCollider>().height = 1f;
		} else {
            animator.SetTrigger("Stand");
            animator.ResetTrigger("Crouch");
			GetComponent<CapsuleCollider>().height = 2f;
		}
    }

    /// <summary>
    /// Start moving. For humans, point the gun at the ground while doing so.
    /// </summary>
    public void StartMoving() {
        animator.SetFloat("InputMagnitude", 1f);
        animator.SetFloat("Vertical", 2f);
        SetAim(0f);
    }

    public void StopMoving() {
        animator.SetFloat("InputMagnitude", 0f);
        animator.SetFloat("Vertical", 0f);
        animator.SetFloat("Horizontal", 0f);
        SetAim(1f);
    }

    public void Strafe(Vector3 direction) {
        float frontDot = Vector3.Dot(transform.forward, direction);
        float sideDot = Vector3.Dot(transform.right, direction);
        animator.SetFloat("Vertical", frontDot);
        animator.SetFloat("Horizontal", sideDot);
        animator.SetFloat("InputMagnitude", 1f);
        Invoke("ResetStrafe", 0.3f);
    }

    private void ResetStrafe() {
        animator.SetFloat("Vertical", 0f);
        animator.SetFloat("Horizontal", 0f);
        animator.SetFloat("InputMagnitude", 0f);
    }

    public void Attack() {
        animator.SetTrigger("Fire");
    }

    public void Reload() {
        animator.SetTrigger("Reload");
    }

    public void SetVerticalAimAngle(float angle) {
        if (Mathf.Abs(angle) > 0.1f) {
            animator.SetFloat("VerAimAngle", angle);
        }
    }

    /// <summary>
    /// When player units are selecting their path, disable animations
    /// </summary>
    /// <param name="selecting"></param>
    public void SetPathStatus(bool selecting) {
        if (selecting) {
            SetAim(0f);
        } else {
            SetAim(1f);
        }
    }

    public void SetAim(float aim) {
        animator.SetFloat("Aim", aim);
    }

    public void Vault() {
        animator.SetTrigger("Vault");
    }

    public void Climb() {
        animator.SetTrigger("Climb");
    }

    public void StopClimbing() {
        animator.ResetTrigger("Climb");
    }

    /// <summary>
    /// Only used by turrets.
    /// </summary>
    /// <param name="status"></param>
    public void SetScanStatus(bool status) {
        animator.SetBool("Scanning", status);
    }
}
