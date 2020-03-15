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

        if (targetSpotted || beenAttacked) {
            IDamage target = targetSelection.SelectTarget();
            if (lineOfSight.CanSeeTarget(target)) {
                PerformAttack();
            } else {
                FindCover(target.GetTransform().position, target.GetTransform().position - myTransform.position);
            }
        } else {
            Patrol();
        }
    }

    private void PerformAttack() {
        if (weapon.GetCurrentFireCost() >= currentActionPoints) {
            Attack();
        } else {
            // they're drilled to reload where possible
            if (weapon.GetRemainingAmmo() < 2) {
                Debug.LogFormat("{0} can't fire - reloading!", this);
                Reload();
            }
            FinishedTurn();
        }
    }

    /// <summary>
    /// Should only occur when moving to cover.
    /// </summary>
    public override void ReachedDestination() {
            IDamage target = targetSelection.SelectTarget();
            int attackCost = weapon.GetCurrentFireCost();
            if (lineOfSight.CanSeeTarget(target)) {
                Debug.LogFormat("{0} in cover at {1}! Opening fire on {2}", this, myTransform.position, target.GetTransform());
                TrackTarget(target);
                PerformAttack();
            } else {
                // call in the hostile sighting
                Debug.LogFormat("{0} in cover at {1}, but can't see {2}! Calling it in", this, myTransform.position, target.GetTransform());
                unitManager.AlertAllUnits(target);
                FinishedTurn();
            }
    }

    void Patrol() {
        if (patrolRoute.Count > 0) {
            PatrolPoint nextStep = patrolRoute[patrolIndex];
            if (!SetDestination(nextStep.GetPosition())) {
                Debug.LogWarningFormat("{0} can't reach their destination: {1}. Length: {2}", this, nextStep, GetPathLength());
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

    /// <summary>
    /// Unlike bots, should take cover
    /// </summary>
    /// <param name="damageType">What type of damage</param>
    /// <param name="damageAmount">How much</param>
    public override void Damage(DamageType damageType, int damageAmount) {
        base.Damage(damageType, damageAmount);
        if (health > 0) {
            beenAttacked = true;
        } else {
            Die();
        }
    }

    public override void FindCover(Vector3 position, Vector3 direction) {
        CoverResult target = coverFinder.FindCover(position, direction);
		destinationTrigger.SetPosition(target.GetPosition());
		if (!SetDestination(target.GetPosition())) {
			Debug.LogFormat("{0} can't reach cover at {1}!", this, target);
            // at least crouching makes them harder to hit.
            Crouch();
            FinishedTurn();
		}
    }

    void Update() {
        if (targetSpotted) {
            IDamage target = targetSelection.SelectTarget();
            TrackTarget(target);
        }
    }

}
