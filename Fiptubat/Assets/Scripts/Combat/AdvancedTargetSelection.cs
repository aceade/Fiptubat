using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A more expansive version of target selection. Will default to base (closest enemy) as fallback.
/// <summary>
public class AdvancedTargetSelection : UnitTargetSelection
{
    public float calculationInterval = 0.5f;

    private WaitForSeconds calculationCycle;

    private bool calculatingExposure = false;

    private Dictionary<IDamage, int> targetsByExposure;

    protected override void Start() {
        base.Start();
        if (selectionAlgorithm == TargetPriority.MOST_EXPOSED) {
            calculationCycle = new WaitForSeconds(calculationInterval);
            targetsByExposure = new Dictionary<IDamage, int>();
        }
    }

    public override void AddTarget(IDamage target) {
        base.AddTarget(target);
        if (selectionAlgorithm == TargetPriority.MOST_EXPOSED) {
            if (!targetsByExposure.ContainsKey(target)) {
                targetsByExposure.Add(target, 0);
            }
            if (!calculatingExposure) {
                calculatingExposure = true;
                StartCoroutine(performExposureCalculations());
            }
        }
    }

    /// <summary>
    /// Remove a target. If we're calculating exposure and there are no enemies left, stop.
    /// </summary>
    /// <param name="target"></param>
    public override void RemoveTarget(IDamage target) {
        base.RemoveTarget(target);
        if (!HasTargetsLeft()) {
            calculatingExposure = false;
        }
    }

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
            case TargetPriority.MOST_EXPOSED:
                return getMostExposedEnemy();
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

    private IDamage getMostExposedEnemy() {
        var sortedTargets = targetsByExposure.OrderByDescending(x => x.Value).ToList();
        return sortedTargets[0].Key;
    }

    private IEnumerator performExposureCalculations() {
        while (calculatingExposure) {
            Debug.LogFormat("Calculating exposure of {0} enemies", knownTargets.Count);
            knownTargets.ForEach(target => targetsByExposure[target] = calculateExposure(target));
            yield return calculationCycle;
        }
        
    }

    private int calculateExposure(IDamage target) {
        Transform targetTransform = target.GetTransform();
        Bounds targetBounds = new Bounds(targetTransform.position, Vector3.zero);
        Collider[] colls = targetTransform.GetComponentsInChildren<Collider>();
        for (int i = 0; i < colls.Length; i++) {
            // ignore triggers
            if (colls[i].isTrigger) {
                continue;
            }
            var collBounds = colls[i].bounds;
            targetBounds.Encapsulate(collBounds);
        }
        float height = targetBounds.extents.y;
        Vector3 centre = targetBounds.center;
        Vector3 minPosition = targetBounds.min + (targetTransform.right * 0.2f);
        minPosition.y = centre.y;
        Vector3 maxPosition = targetBounds.max - (targetTransform.right * 0.2f);
        maxPosition.y = centre.y;
        int hitCount = castExposureRay(minPosition, targetTransform);
        hitCount += castExposureRay(maxPosition, targetTransform);
        hitCount += castExposureRay(centre + (Vector3.up * height), targetTransform);
        hitCount += castExposureRay(centre - (Vector3.up * height), targetTransform);

        return hitCount;
    }

    /// <summary>
    /// Cast a ray towards the target position.
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="intendedTarget"></param>
    /// <returns>1 if it hits the intended target, 0 otherwise</returns>
    private int castExposureRay(Vector3 targetPosition, Transform intendedTarget) {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, targetPosition - transform.position, out hit, 30f, Physics.AllLayers, QueryTriggerInteraction.Ignore)) {
            Debug.DrawRay(transform.position, hit.point - transform.position, Color.red);
            return hit.transform == intendedTarget ? 1 : 0;
        }
        return 0;
    }
}
