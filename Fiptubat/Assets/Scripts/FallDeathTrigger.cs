using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deals fatal damage to anything that enters it. Purely in case some idiot walks off a cliff.
/// </summary>
public class FallDeathTrigger : MonoBehaviour
{
    private int damage = 20000000;

    // Update is called once per frame
    void OnTriggerEnter(Collider collider)
    {
        if (!collider.isTrigger) {
            var damageScript = collider.transform.root.GetComponent<IDamage>();
            damageScript.Damage(DamageType.REGULAR, damage);
        }
    }
}
