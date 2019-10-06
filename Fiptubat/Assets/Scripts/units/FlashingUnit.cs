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

    public Color selectedColour, deselectedColour, seenEnemyColour, heardEnemyColour;
    
    private bool rotatingLight = false;

    protected override void Start() {
        flashLight = GetComponentInChildren<Light>();
        base.Start();
    }

    public override void SelectUnit() {
        if (flashLight == null) {
            flashLight = GetComponentInChildren<Light>();
        }
        flashLight.color = selectedColour;
        if (!rotatingLight) {
            StartCoroutine(rotateLight());
        }
        Invoke("FinishTurn", 3f);
        
    }

    private IEnumerator rotateLight() {
        if (!rotatingLight) {
            rotatingLight = true;
        }
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

    private void rotateTowardsTarget(Transform target) {
        Vector3 direction = target.position - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    public override void TargetLocated(IDamage target) {
        Debug.LogFormat("I can see {0}! But all I can do is change colour and stare at them!", target.GetTransform());
        flashLight.color = seenEnemyColour;
        rotateTowardsTarget(target.GetTransform());
        if (!rotatingLight) {
            StartCoroutine(rotateLight());
        }
        
    }

    private void FinishTurn() {
        base.FinishedTurn();
    }

}
