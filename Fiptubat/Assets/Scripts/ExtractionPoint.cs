using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player's victory condition is to reach this.
/// </summary>
public class ExtractionPoint : MonoBehaviour
{

    public UnitManager playerUnitManager;

    private ParticleSystem particles;
    private int currentUnits;

    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponent<ParticleSystem>();
        if (playerUnitManager == null) {
            Debug.LogErrorFormat("You forgot to set the player's unit manager, numpty!");
        }
    }

    void OnTriggerEnter()
    {
        currentUnits++;
        if (currentUnits >= playerUnitManager.GetRemainingUnitCount()) {
            playerUnitManager.AllUnitsExtracted();
        }
    }
}
