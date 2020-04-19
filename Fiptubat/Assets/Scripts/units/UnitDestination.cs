using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Works around the whole "am I there yet?" issue by adding a trigger for a specific unit.
/// </summary>
[RequireComponent(typeof(Collider))]
public class UnitDestination : MonoBehaviour
{
    private Transform myTransform, parent;

    private BaseUnit myUnit;

    public float offset;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = transform;
    }

    void OnTriggerEnter(Collider coll)
    {
        if (!coll.isTrigger && coll.transform.root == parent) {
            myUnit.ReachedDestination();
        }
    }

    public void SetPosition(Vector3 position) 
    {
        myTransform.position = position + (Vector3.up * offset);
    }

    public void SetUnit(BaseUnit unit) 
    {
        myUnit = unit;
        parent = myUnit.transform;
    }
}
