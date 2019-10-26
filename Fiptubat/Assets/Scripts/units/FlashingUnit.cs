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

    public override void SelectUnit() {
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
            // find the nearest target only
            IDamage nearestTarget = targetSelection.SelectTarget();
            rotateTowardsTarget(nearestTarget.GetTransform());
            yield return flashlightRotationCycle;
        }
        
    }

    private void rotateTowardsTarget(Transform target) {
        Vector3 direction = target.position - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    public override void TargetSpotted(IDamage target) {
        flashLight.color = seenEnemyColour;
        if (!rotatingLight) {
            base.TargetSpotted(target);
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
