using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a player-controlled unit.
/// </summary>
public class PlayerUnit : BaseUnit {


    private PlayerUnitControl unitControl;

    protected override void Start() {
        unitControl = GetComponent<PlayerUnitControl>();
        base.Start();
    }

    /// <summary>
    /// Select the unit and activate their camera.
    /// </summary>
    public override void SelectUnit() {
        Debug.LogFormat("Selecting {0}", unitName);
        unitControl.enabled = true;
        base.SelectUnit();
        unitControl.AllowMovement();    // work around a timing issue of some kind
    }

    /// <summary>
    /// Deselect the unit. In this case, deactivate their camera.
    /// </summary>
    public override void DeselectUnit() {
        Debug.LogFormat("Deselecting {0}", unitName);
        unitControl.enabled = false;
        base.DeselectUnit();
    }

    public override void TargetLocated(IDamage target) {
        Debug.LogFormat("{0}: made contact! {1}", unitName, target.GetTransform());
        // waiting for vocal confirmation mechanism
    }

    public override void StandDown() {
        Debug.LogFormat("{0}, stand down for this turn!", unitName);
        unitControl.ForbidMovement();
    }

    public override bool IsStillMoving() {
        return !Mathf.Approximately(navMeshAgent.velocity.magnitude, 0);
    }

    public override void Crouch() {
        base.Crouch();
        float offset = isCrouched ? -0.6f : 0.6f;
        unitControl.MoveCamera(offset);
    }
}