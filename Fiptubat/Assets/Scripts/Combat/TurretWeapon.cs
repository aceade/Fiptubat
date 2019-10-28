using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretWeapon : WeaponBase
{
    private int missedShots = 0;

    /// <summary>
    /// Unlike the main implementation, we don't care
    /// <summary>
    public override bool Fire(){
        StartCoroutine(turretAttackCycle());
        return (missedShots < 1);
    }
    
    private IEnumerator turretAttackCycle() {
        canAttack = false;
        int shotsToFire = GetCurrentFireMode().bulletsFired;
        if (currentAmmo < shotsToFire ) {
            shotsToFire = currentAmmo;
        }
        for (int i = 0; i < shotsToFire; i++) {
            currentAmmo--;
            if (!checkTargetHit()) {
                missedShots++;
            }
            yield return fireCycle;
        }
        if (currentAmmo > 0) {
            Debug.LogFormat("Turret {0} has {1} rounds remaining", this, currentAmmo);
            canAttack = true;
        }
        
    }

    private bool checkTargetHit() {
        RaycastHit hit;
        Vector3 fireDir = CalculateFireDirection();
        Debug.DrawRay(muzzle.position, fireDir * maxDistance, Color.red, 2f);
        Debug.DrawRay(muzzle.position, muzzle.forward, Color.cyan, 2f);
        if (Physics.Raycast(muzzle.position, fireDir, out hit, maxDistance)) {
            
            var hitTransform = hit.transform;
            Debug.LogFormat("Turret hit {0}", hitTransform);
            var damageScript = hitTransform.root.GetComponent<IDamage>();
            if (damageScript == null) {
                return false;
            } else {
                damageScript.Damage(DamageType.REGULAR, damage);
                return true;
            }
        }
        return false; 
    }
}
