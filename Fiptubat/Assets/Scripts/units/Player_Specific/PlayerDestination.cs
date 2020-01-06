using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Works around the whole "am I there yet?" issue by adding a trigger for a specific unit.
/// Could probably be merged with PlayerMoveMarker - or repurposed for NPCs as well.
/// </summary>
[RequireComponent(typeof(Collider))]
public class PlayerDestination : MonoBehaviour
{
    private Transform myTransform, parent;

    private PlayerUnitControl myUnit;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = transform;
        parent = myTransform.parent;
    }

    void OnTriggerEnter(Collider coll)
    {
        Debug.LogFormat("Destination entered by {0}", coll);
        if (coll.transform == parent) {
            
            myUnit.ReachedDestination(true);
        }
    }

    public void SetPosition(Vector3 position) 
    {
        myTransform.position = position;
    }

    public void SetUnit(PlayerUnitControl unit) 
    {
        myUnit = unit;
        parent = myUnit.transform;
    }
}
