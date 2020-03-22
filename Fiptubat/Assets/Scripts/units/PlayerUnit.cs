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
    public override void SelectUnit(bool isMyTurn) {
        unitControl.enabled = true;
        base.SelectUnit(isMyTurn);
        if (isMyTurn) {
            unitControl.AllowMovement();    // work around a timing issue of some kind
        }
    }

    /// <summary>
    /// Deselect the unit. In this case, deactivate their camera.
    /// </summary>
    public override void DeselectUnit() {
        unitControl.enabled = false;
        base.DeselectUnit();
    }

    public override void TargetSpotted(IDamage target) {
        Debug.LogFormat("{0}: made contact! {1}", unitName, target.GetTransform());
        voiceSystem.TargetSpotted();
    }

    public override void StandDown() {
        Debug.LogFormat("{0}, stand down for this turn!", unitName);
        unitControl.ForbidMovement();
    }

    public override bool IsStillMoving() {
        return (navMeshAgent.velocity.sqrMagnitude > 0.1f);
    }

    public override void Crouch() {
        if (currentActionPoints >= 4) {
            base.Crouch();
            float offset = isCrouched ? -0.6f : 0.6f;
            unitControl.MoveCamera(offset);
        }
    }

    public override void FindCover(Vector3 position, Vector3 direction) {
        throw new System.NotSupportedException("Player units are manually controlled!");
    }

}