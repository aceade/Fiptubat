using Unity;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Handles target selection.
/// </summary>
public class UnitTargetSelection : MonoBehaviour {

    protected List<IDamage> knownTargets = new List<IDamage>();

    private Transform myTransform;

    public TargetPriority selectionAlgorithm = TargetPriority.CLOSEST;

    void Start() {
        myTransform = transform;
    }

    public void AddTarget(IDamage target) {
        if (!knownTargets.Contains(target)) {
            Debug.LogFormat("{0} adding {1} as possible target", this, target.GetTransform());
            knownTargets.Add(target);
        }
    }

    public bool AlreadyHasTarget(IDamage target) {
        return knownTargets.Contains(target);
    }

    public bool HasTargetsLeft() {
        return knownTargets.Count > 0;
    }

    public void RemoveTarget(IDamage target) {
        knownTargets.Remove(target);
    }

    /// <summary>
    /// Default implementation - return the closest.
    /// </summary>
    public virtual IDamage SelectTarget() {
        return knownTargets.Aggregate((firstTarget, secondTarget) => 
            Vector3.Distance(myTransform.position, firstTarget.GetTransform().position) < 
            Vector3.Distance(myTransform.position, secondTarget.GetTransform().position) ? firstTarget : secondTarget);
    }

    void OnDisable() {
        knownTargets.Clear();
    }
}