using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A test class that kills a unit instantly. Used to check what happens when a unit dies
/// </summary>
public class DieInstantlyTest : MonoBehaviour
{

    public int damage = 5000;

    public DamageType damageType = DamageType.REGULAR;

    public float delay = 1f;

    private IDamage myTarget;

    void Start()
    {
        myTarget = GetComponent<IDamage>();
        Invoke("DieDieDie", delay);
    }

    void DieDieDie() {
        myTarget.Damage(damageType, damage, Vector3.up);
    }

    
}
