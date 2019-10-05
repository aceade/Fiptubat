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
    
    protected override void Start() {
        flashLight = GetComponentInChildren<Light>();
        base.Start();
    }

    public override void SelectUnit() {
        if (flashLight == null) {
            flashLight = GetComponentInChildren<Light>();
        }
        flashLight.color = selectedColour;
        StartCoroutine(rotateLight());
        Invoke("FinishTurn", 3f);
    }

    IEnumerator rotateLight() {
        int rotationCount = 0;
        
        while (rotationCount < 3) {
            float angle = flashLight.transform.rotation.y;
            flashLight.transform.Rotate(Vector3.up * 100f * Time.deltaTime);
            if (Mathf.Approximately(angle, 0f)) {
                rotationCount ++;
            }
            yield return flashlightRotationCycle;
        }
        
    }

    private void FinishTurn() {
        base.FinishedTurn();
    }

}
