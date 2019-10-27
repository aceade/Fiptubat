using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Turrets that have gone haywire. They don't move, crouch or whatever - just stand still and shoot whatever annoys them
/// <summary>
public class Turret : BaseUnit
{

    private bool targetSpotted;

    private Transform myTransform, barrel;

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
            TrackTarget(target);
        }
    }

    private void Scan() {
        // scan animation
        myTransform.Rotate(Vector3.up * 10f * Time.deltaTime);
    }

    private void TrackTarget(IDamage target) {
        Vector3 horizontalDir = target.GetTransform().position - myTransform.position;
        horizontalDir.y = 0;
        myTransform.forward = horizontalDir;
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
        }
    }

}
