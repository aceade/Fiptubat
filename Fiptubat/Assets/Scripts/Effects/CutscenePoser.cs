using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutscenePoser : MonoBehaviour
{
    private UnitAnimator animator;

    public bool crouched;

    public bool aiming;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<UnitAnimator>();
        Invoke("Setup", 0.1f);
    }

    private void Setup() {
        if (crouched) {
            animator.Crouch(true);
        }

        if (aiming) {
            animator.SetAim(2f);
        } else {
            animator.SetAim(0f);
        }
    }

}
