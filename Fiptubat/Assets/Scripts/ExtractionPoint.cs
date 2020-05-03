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

    private List<PlayerUnit> activeUnits = new List<PlayerUnit>();

    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponent<ParticleSystem>();
        if (playerUnitManager == null) {
            Debug.LogErrorFormat("You forgot to set the player's unit manager, numpty!");
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (!coll.isTrigger) {
            var unitScript = coll.transform.root.GetComponent<PlayerUnit>();
            if (!activeUnits.Contains(unitScript)) {
                Debug.LogFormat("{0} has reached the extraction point", unitScript);
                activeUnits.Add(unitScript);
            }

            
            if (activeUnits.Count >= playerUnitManager.GetRemainingUnitCount()) {
                playerUnitManager.AllUnitsExtracted();
            }
        }
    }
}
