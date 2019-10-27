using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Turrets that have gone haywire. They don't move, crouch or whatever - just stand still and shoot whatever annoys them
/// <summary>
public class Turret : BaseUnit
{

    private bool targetSpotted;

    protected override void Start(){
        base.Start();
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
        }
    }

    private void Scan() {
        // scan animation
    }

}
