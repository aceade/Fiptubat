using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyDamage : MonoBehaviour, IDamage
{
    public int health = 1;

    void Start() {
        Debug.LogFormat("DummyDamage {0} online", this);
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().WakeUp();
    }

    public Transform GetTransform() {
        return transform;
    }

    public int GetRemainingHealth() {
        return 0;
    }

    public void Damage(DamageType damageType, int damageAmount) {
        Debug.LogFormat("{0} took {1} points of {2} damage", this, damageAmount, damageType);
        if (damageAmount > health) {
            Die();
        }
    }

    private void Die() {
        Debug.LogFormat("{0} is deaded!!!1!", this);
    }

    public int GetPotentialDamage() {
        return 0;
    }
}
