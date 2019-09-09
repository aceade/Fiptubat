using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a player-controlled unit.
/// </summary>
public class PlayerUnit : BaseUnit {

    private Camera myCamera;

    protected override void Start() {
        myCamera = GetComponent<Camera>();
        myCamera.enabled = false;
        base.Start();
    }

    /// <summary>
    /// Select the unit and activate their camera.
    /// </summary>
    public override void SelectUnit() {
        myCamera.enabled = true;
    }

    /// <summary>
    /// Deselect the unit. In this case, deactivate their camera.
    /// </summary>
    public override void DeselectUnit() {
        myCamera.enabled = false;
    }
}