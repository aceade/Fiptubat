using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Patrols along a path until out of points and punches with a giant hammer.
/// <summary>
public class PatrolBot : BaseUnit
{

    private bool targetSpotted;

    public List<PatrolPoint> patrolRoute;
    private int patrolIndex;

    protected override void Start()
    {
        base.Start();
    }

    public override void SelectUnit() {
        base.SelectUnit();
        if (targetSpotted) {
            IDamage target = targetSelection.SelectTarget();
            TrackTarget(target);
        } else {
            Patrol();
        }
    }

    void Update() {
        if (targetSpotted) {
            
            IDamage currentTarget = targetSelection.SelectTarget();
            TrackTarget(currentTarget);
            if (isSelected) {
                PerformAttack(currentTarget);
            }
        }
    }

    void PerformAttack(IDamage target) {
        if (target.GetRemainingHealth() > 0) {
            if (Vector3.Distance(myTransform.position, target.GetTransform().position) <= weapon.maxDistance) {
                Attack();
            } else {
                SetDestination(target.GetTransform().position);
            }
            
        } else {
            targetSelection.RemoveTarget(target);
            voiceSystem.TargetDestroyed();
            if (!targetSelection.HasTargetsLeft()) {
                targetSpotted = false;
                // reload after target defeated
                Reload();
            }
        }
    }

    private void TrackTarget(IDamage target) {
        if (target == null) {
            targetSpotted = false;
        }

        Vector3 horizontalDir = target.GetTransform().position - myTransform.position;
        horizontalDir.y = 0;
        Vector3 desired = Vector3.RotateTowards(myTransform.forward, horizontalDir, 5f * Time.deltaTime, 0f);
        myTransform.rotation = Quaternion.LookRotation(desired);

    }

    public override void TargetSpotted(IDamage target) {
        base.TargetSpotted(target);
        targetSpotted = true;
    }

    void Patrol() {
        Vector3 targetPosition = patrolRoute[patrolIndex].GetPosition();
        float pathLength = GetPathLength();
        int moveCost = GetMoveCost(pathLength);
        if (moveCost <= GetRemainingActionPoints()) {
            if (SetDestination(targetPosition)) {
                actionPoints -= moveCost;
            } else {
                FinishedTurn();
            }
            
        } else {
            Debug.LogFormat("{0} does not have enough points to move. Scanning", this);
            // todo: scan
            FinishedTurn();
        }
    }

    private void IncrementPatrolIndex() {
        patrolIndex++;
        if (patrolIndex >= patrolRoute.Count) {
            patrolIndex = 0;
        }
        Patrol();
        
    }

    public override void ReachedPatrolPoint(PatrolPoint point) {
        Debug.LogFormat("{0} reached patrolpoint: {1}", this, point);
        if (point == patrolRoute[patrolIndex]) {
            IncrementPatrolIndex();
        }
    }
}
