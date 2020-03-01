using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Humanoid enemies. Can do the following:
/// * Stand guard if they have no patrol points and will patrol otherwise
/// * Move towards an enemy if they can't see it.
/// * If they can see an enemy, they will move to cover and then remain there
/// </summary>
public class HumanoidEnemy : BaseUnit
{

    public List<PatrolPoint> patrolRoute;

    private int patrolIndex;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
    }

    public override void SelectUnit(bool isMyTurn) {
        base.SelectUnit(isMyTurn);
        if (targetSpotted) {
            IDamage target = targetSelection.SelectTarget();
            if (lineOfSight.CanSeeTarget(target)) {
                FindCover(myTransform.position, target.GetTransform().position - myTransform.position);
            } else {
                SetDestination(target.GetTransform().position);
            }
        } else {
            Patrol();
        }
    }

    /// <summary>
    /// Should only occur when moving to cover.
    /// </summary>
    public override void ReachedDestination() {
            IDamage target = targetSelection.SelectTarget();
            int attackCost = weapon.GetCurrentFireCost();
            if (lineOfSight.CanSeeTarget(target)) {
                Attack();
            } else {
                // can we see them to the side?

            }
    }

    void Patrol() {
        if (patrolRoute.Count > 0) {
            PatrolPoint nextStep = patrolRoute[patrolIndex];
            if (!SetDestination(nextStep.GetPosition())) {
                Debug.LogWarningFormat("{0} can't reach their destination: {1}. Length: {2}", this, nextStep);
                Invoke("FinishedTurn", 1f);
            }
        } else {
            Invoke("FinishedTurn", 2f);
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
            if (!targetSpotted) {
                IncrementPatrolIndex();
            } else {

                IDamage target = targetSelection.SelectTarget();
                Vector3 targetPos = target.GetTransform().position;
                Vector3 dir = myTransform.position - targetPos;
                Debug.DrawRay(targetPos, dir, Color.magenta, 3f);
                Debug.LogFormat("{0} stopping patrol at {1}, found a target at {2}! Direction: {3}", this,
                    point.GetPosition(), targetPos, dir);
                FindCover(myTransform.position, dir);
            }
        }
    }

}
