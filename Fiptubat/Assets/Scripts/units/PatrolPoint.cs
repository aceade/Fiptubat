using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Acts as a placeholder for patrol waypoints.
/// Child object will be a yellow Editor-Only cube for debugging
/// </summary>
public class PatrolPoint : MonoBehaviour
{

    private Vector3 position;

    void Start()
    {
        position = transform.position;
    }

    public Vector3 GetPosition() {
        return position;
    }

    void OnTriggerEnter(Collider coll) {
        if (!coll.isTrigger) {
            var unitBase = coll.transform.root.GetComponent<BaseUnit>();
            if (unitBase != null) {
                unitBase.ReachedPatrolPoint(this);
            }
        }
        
    }
}
