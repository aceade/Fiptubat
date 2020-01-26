using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A dummy unit that flashes. Used to test swapping sides and detection by flashing a siren light
/// </summary>
public class FlashingUnit : BaseUnit
{
    private Light flashLight;

    private WaitForSeconds flashlightRotationCycle = new WaitForSeconds(0.1f);

    private WaitForSeconds targetTrackerCycle = new WaitForSeconds(0.05f);

    public Color selectedColour, deselectedColour, seenEnemyColour, heardEnemyColour;
    
    private bool rotatingLight = false;
    private bool trackingTarget = false;

    protected override void Start() {
        flashLight = GetComponentInChildren<Light>();
        base.Start();
    }

    public override void SelectUnit(bool isMyTurn) {
        voiceSystem.Moving();
        if (flashLight == null) {
            flashLight = GetComponentInChildren<Light>();
        }
        flashLight.color = selectedColour;
        if (!rotatingLight) {
            StartCoroutine(rotateLight());
        }
        Invoke("FinishTurn", 2f);
        
    }

    private IEnumerator rotateLight() {
        rotatingLight = true;
        int rotationCount = 0;
        
        while (rotationCount < 3) {
            float angle = flashLight.transform.rotation.y;
            flashLight.transform.Rotate(Vector3.up * 100f * Time.deltaTime);
            if (Mathf.Approximately(angle, 0f)) {
                rotationCount ++;
            }
            yield return flashlightRotationCycle;
        }
        rotatingLight = false;
        
    }

    private IEnumerator trackTarget() {
        while (trackingTarget) {
            IDamage target = targetSelection.SelectTarget();
            Debug.LogFormat("Target for flashing light is {0}", target.GetTransform());
            rotateTowardsTarget(target.GetTransform());
            yield return flashlightRotationCycle;
        }
        
    }

    private void rotateTowardsTarget(Transform target) {
        Vector3 horizontalDir = target.position - myTransform.position;
        horizontalDir.y = 0;
        Vector3 desired = Vector3.RotateTowards(myTransform.forward, horizontalDir, 2f * Time.deltaTime, 0f);
        myTransform.rotation = Quaternion.LookRotation(desired);
    }

    public override void TargetSpotted(IDamage target) {
        flashLight.color = seenEnemyColour;
        if (target != null && !targetSelection.AlreadyHasTarget(target)) {
            targetSelection.AddTarget(target);
        }

        if (!rotatingLight) {
            StartCoroutine(rotateLight());
        }

        if (!trackingTarget) {
            trackingTarget = true;
            StartCoroutine(trackTarget());
        }
        
    }

    private void FinishTurn() {
        base.FinishedTurn();
    }

}
