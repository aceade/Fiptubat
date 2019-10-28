﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Turrets that have gone haywire. They don't move, crouch or whatever - just stand still and shoot whatever annoys them
/// <summary>
public class Turret : BaseUnit
{

    private bool targetSpotted;

    private Transform myTransform, barrel;

    public float rotationSpeed = 3f;

    protected override void Start(){
        base.Start();
        myTransform = transform;
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
                    if (!targetSelection.HasTargetsLeft()) {
                        targetSpotted = false;
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
    }

    private void Scan() {
        // scan animation
        myTransform.Rotate(Vector3.up * 10f * Time.deltaTime);
    }

    private void TrackTarget(IDamage target) {
        Vector3 horizontalDir = target.GetTransform().position - myTransform.position;
        horizontalDir.y = 0;
        Vector3 desired = Vector3.RotateTowards(myTransform.forward, horizontalDir, rotationSpeed * Time.deltaTime, 0f);
        myTransform.rotation = Quaternion.LookRotation(desired);
    }

    public override void TargetSpotted(IDamage target) {
        base.TargetSpotted(target);
        targetSpotted = true;
    }

    public override void SelectUnit() {
        base.SelectUnit();
        if (targetSpotted) {
            IDamage target = targetSelection.SelectTarget();
            TrackTarget(target);
        } else {
            Invoke("FinishedTurn", 5f);
        }
    }

}
