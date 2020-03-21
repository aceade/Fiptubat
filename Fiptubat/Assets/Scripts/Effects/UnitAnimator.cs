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
        animator = GetComponent<Animator>();
        myCollider = GetComponent<Collider>();
    }

    /// <summary>
    /// Used by humanoid units to crouch. Currently based on changing collider heights
    /// </summary>
    /// <param name="crouched"></param>
    public void Crouch(bool crouched) {
        // TODO: replace with crouch animation
        if (crouched) {
			GetComponent<CapsuleCollider>().height = 1f;
		} else {
			GetComponent<CapsuleCollider>().height = 2f;
		}
    }

    public void Vault() {
        GetComponent<Rigidbody>().AddForce(Vector3.up * 2f, ForceMode.Impulse);
    }

    public void Climb() {
        // TBD
    }

    /// <summary>
    /// Only used by turrets.
    /// </summary>
    /// <param name="status"></param>
    public void SetScanStatus(bool status) {
        animator.SetBool("Scanning", status);
    }
}
