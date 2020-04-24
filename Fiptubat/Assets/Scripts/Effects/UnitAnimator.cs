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
    protected Animator animator;

    protected void Start() {
        animator = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// Used by humanoid units to crouch.
    /// Requires bones to have colliders.
    /// </summary>
    /// <param name="crouched"></param>
    public void Crouch(bool crouched) {
        if (crouched) {
            animator.SetTrigger("Crouch");
            animator.ResetTrigger("Stand");
		} else {
            animator.SetTrigger("Stand");
            animator.ResetTrigger("Crouch");
		}
    }

    /// <summary>
    /// Start moving. For humans, point the gun at the ground while doing so.
    /// </summary>
    public virtual void StartMoving() {
        SetAim(0f);
    }

    public virtual void StopMoving() {
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

    protected void ResetStrafe() {
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

    public void Damage(Vector3 attackDirection) {
        float forDot = Mathf.RoundToInt(Vector3.Dot(attackDirection, transform.forward));
        float sideDot = Mathf.RoundToInt(Vector3.Dot(attackDirection, transform.right));
        animator.SetFloat("Flinch_Forward", forDot);
        animator.SetFloat("Flinch_Sideways", sideDot);
        animator.SetTrigger("Hit");
    }

    public void Die() {
        animator.SetTrigger("Dying");
    }

    /// <summary>
    /// Only used by turrets.
    /// </summary>
    /// <param name="status"></param>
    public void SetScanStatus(bool status) {
        animator.SetBool("Scanning", status);
    }
}
