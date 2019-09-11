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
        unitControl.enabled = true;
    }

    /// <summary>
    /// Deselect the unit. In this case, deactivate their camera.
    /// </summary>
    public override void DeselectUnit() {
        unitControl.enabled = false;
    }
}