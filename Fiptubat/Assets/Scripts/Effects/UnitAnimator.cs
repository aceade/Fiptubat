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
    /// Used by humanoid units to crouch. Currently based on changing collider heights
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

    public void StartMoving() {
        animator.SetFloat("InputMagnitude", 1f);
        animator.SetFloat("Vertical", 1f);
    }

    public void StopMoving() {
        animator.SetFloat("InputMagnitude", 0f);
    }

    public void Strafe(Vector3 direction) {
        float frontDot = Vector3.Dot(transform.forward, direction);
        float sideDot = Vector3.Dot(transform.right, direction);
        if (frontDot != 0) {
            animator.SetFloat("Vertical", 1f);
        } else {
            animator.SetFloat("Horizontal", 1f);
        }
        Invoke("ResetStrafe", 1f);
    }

    private void ResetStrafe() {
        animator.SetFloat("Vertical", 0f);
        animator.SetFloat("Horizontal", 0f);
    }

    public void Attack() {
        animator.SetTrigger("Fire");
    }

    public void Reload() {
        animator.SetTrigger("Reload");
    }

    public void Vault() {
        GetComponent<Rigidbody>().AddForce(Vector3.up * 2f, ForceMode.Impulse);
        animator.SetTrigger("Vault");
    }

    public void Climb() {
        animator.SetTrigger("Climb");
    }

    /// <summary>
    /// Only used by turrets.
    /// </summary>
    /// <param name="status"></param>
    public void SetScanStatus(bool status) {
        animator.SetBool("Scanning", status);
    }
}
