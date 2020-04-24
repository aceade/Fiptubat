using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretWeapon : WeaponBase
{
    private int missedShots = 0;

    /// <summary>
    /// Unlike the main implementation, we don't care about missing shots
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
            canAttack = true;
        }
        
    }

    private bool checkTargetHit() {
        RaycastHit hit;
        Vector3 fireDir = CalculateFireDirection();
        ShowTracers(fireDir);
        audioSource.PlayOneShot(firingNoise);
        if (Physics.Raycast(muzzle.position, fireDir, out hit, maxDistance, layerMask, QueryTriggerInteraction.Ignore)) {
            
            var hitTransform = hit.transform;
            Debug.LogFormat("Turret hit {0}", hitTransform);
            var damageScript = hitTransform.root.GetComponent<IDamage>();
            if (damageScript == null) {
                return false;
            } else {
                damageScript.Damage(DamageType.REGULAR, damage, fireDir);
                return true;
            }
        }
        return false; 
    }
}
