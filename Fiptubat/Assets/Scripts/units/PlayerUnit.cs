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
    }

    /// <summary>
    /// Deselect the unit. In this case, deactivate their camera.
    /// </summary>
    public override void DeselectUnit() {
        Debug.LogFormat("Deselecting {0}", unitName);
        unitControl.enabled = false;
        base.DeselectUnit();
    }

    void Update() {
        if (!navMeshAgent.isStopped) {
            
        }
    }
}