using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Patrols along a path until out of points and punches with a giant hammer.
/// <summary>
public class PatrolBot : BaseUnit
{

    public List<PatrolPoint> patrolRoute;
    private int patrolIndex;

    public override void SelectUnit(bool isMyTurn) {
        base.SelectUnit(isMyTurn);
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

    protected override void TrackTarget(IDamage target) {
        Vector3 targetDir = target.GetTransform().position - myTransform.position;
        Vector3 horizontalDir = targetDir;
        horizontalDir.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(horizontalDir);
        myTransform.rotation = Quaternion.RotateTowards(myTransform.rotation, targetRot, navMeshAgent.angularSpeed * Time.deltaTime);
        float angle = 90f - Vector3.Angle(myTransform.up, targetDir);
        animator.SetAim(1f);
        if (angle > 5f) {
            animator.SetVerticalAimAngle(angle);
        }
        
    }

    void PerformAttack(IDamage target) {
        if (target.GetRemainingHealth() > 0) {
            if (Vector3.Distance(myTransform.position, target.GetTransform().position) <= weapon.maxDistance) {
                Debug.LogFormat("{0} attacking. Action points: {1}. Firing cost: {2}", this, currentActionPoints, weapon.GetCurrentFireCost());
                
                if (weapon.CanAttack() && currentActionPoints >= weapon.GetCurrentFireCost()) {
                    Attack();
                } else {
                    unitManager.UnitFinished(this);
                }
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
        if (point == patrolRoute[patrolIndex]) {
            IncrementPatrolIndex();
        }
    }

    public new void HitNearby() {
        // no-op - bots don't care
    }
}
