using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A more expansive version of target selection. Will default to base (closest enemy) as fallback.
/// <summary>
public class AdvancedTargetSelection : UnitTargetSelection
{
    public override IDamage SelectTarget() {
        switch (selectionAlgorithm) {
            case TargetPriority.CLOSEST:
                return base.SelectTarget();
            case TargetPriority.CLOSEST_TO_DEATH:
                return getLowestHealthEnemy();
            case TargetPriority.HIGHEST_DAMAGE_OUTPUT:
                return getHighestDamageOutputEnemy();
            case TargetPriority.HIGHEST_REMAINING_HEALTH:
                return getHealthiestEnemy();
            default:
                return base.SelectTarget();
        }
    }
    
    private IDamage getLowestHealthEnemy() {
        List<IDamage> sortedTargets = knownTargets.OrderBy(target => target.GetRemainingHealth()).ToList();
        return sortedTargets[0];
    }

    private IDamage getHighestDamageOutputEnemy() {
        List<IDamage> sortedTargets = knownTargets.OrderBy(target => target.GetPotentialDamage()).ToList();
        return sortedTargets[0];
    }

    private IDamage getHealthiestEnemy() {
        List<IDamage> sortedTargets = knownTargets.OrderByDescending(target => target.GetRemainingHealth()).ToList();
        return sortedTargets[0];
    }
}
