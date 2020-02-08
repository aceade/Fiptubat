using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Turrets that have gone haywire. They don't move, crouch or whatever - just stand still and shoot whatever annoys them
/// <summary>
public class Turret : BaseUnit
{

    private Transform barrel;

    public float scanRotationSpeed = 10f;
    public float combatRotationSpeed = 10f;

    public float maxScanAngle = 60f;
    private Vector3 startDir;

    protected override void Start(){
        base.Start();
        startDir = myTransform.forward;
        barrel = myTransform.Find("Barrel");
    }

    public override void Crouch() {
        throw new System.NotImplementedException("Turrets can't crouch!");
    }

    void Update() {
        if (!targetSpotted) {
            Scan();
        } else {
            // track the current target
            IDamage target = targetSelection.SelectTarget();
            
            if (target == null) {
                FinishedTurn();
            } else {
                TrackTarget(target);
            }

            if (isSelected) {
                if (target.GetRemainingHealth() > 0) {
                    Attack();
                } else {
                    targetSelection.RemoveTarget(target);
                    voiceSystem.TargetDestroyed();
                    if (!targetSelection.HasTargetsLeft()) {
                        targetSpotted = false;
                        // reload after target defeated
                        Reload();
                        FinishedTurn();
                    }
                }
                
            }
        }
    }

    public override void Attack() {
        Debug.LogFormat("Turret {0} attacking", this);

        // turrets don't care if they miss or run out of ammo
        // the sounds are for diagnostic purposes :)
        if (weapon.CanAttack()) {
            if (!weapon.Fire()) {
                currentActionPoints -= weapon.GetCurrentFireCost();
                voiceSystem.TargetMissed();
            }
        } else {
            if (weapon.GetRemainingAmmo() == 0 && currentActionPoints >= weapon.reloadCost) {
                currentActionPoints -= weapon.reloadCost;
                weapon.Reload();
            }
        }
        if (currentActionPoints < weapon.GetCurrentFireCost()) {
            FinishedTurn();
        }
    }

    private void Scan() {
        // scan animation
        myTransform.Rotate(Vector3.up * scanRotationSpeed * Time.deltaTime);
        float angle = Vector3.Angle(myTransform.forward, startDir);
        if (angle > maxScanAngle) {
            
            scanRotationSpeed *= -1f;
        }
        
    }

    private void TrackTarget(IDamage target) {
        Vector3 horizontalDir = target.GetTransform().position - myTransform.position;
        horizontalDir.y = 0;
        Vector3 desired = Vector3.RotateTowards(myTransform.forward, horizontalDir, combatRotationSpeed * Time.deltaTime, 0f);
        myTransform.rotation = Quaternion.LookRotation(desired);
    }

    public override void TargetSpotted(IDamage target) {
        if (health > 0) {
            targetSpotted = true;
            base.TargetSpotted(target);
        } else {
            unitManager.AlertAllUnits(target);
        }
    }

    public override void SelectUnit(bool isMyTurn) {
        base.SelectUnit(isMyTurn);
        if (targetSpotted) {
            IDamage target = targetSelection.SelectTarget();
            TrackTarget(target);
        } else {
            Invoke("FinishedTurn", 2f);
        }
    }

    protected override void Die() {
        targetSpotted = false;
        targetSelection.enabled = false;
		unitManager.UnitDied(this);
        lineOfSight.maxDetectionRange = 10f;
    }

    public new void HitNearby() {
		// no-op - turrets don't care about suppression
	}

}
