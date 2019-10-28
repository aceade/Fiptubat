using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretWeapon : WeaponBase
{
    private int missedShots = 0;

    public override bool Fire(){
        StartCoroutine(turretAttackCycle());
        return false;
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
            Debug.LogFormat("I hit {0}", hitTransform);
            var damageScript = hitTransform.root.GetComponent<IDamage>();
            if (damageScript == null) {
                return false;
            } else {
                Debug.LogFormat("I shot {0}", damageScript);
                damageScript.Damage(DamageType.REGULAR, damage);
                return true;
            }
        }
        return false; 
    }
}
