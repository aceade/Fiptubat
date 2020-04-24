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

    public override void SelectUnit(bool isMyTurn) {
        base.SelectUnit(isMyTurn);

        // they're trained to reload where possible
        if (weapon.GetRemainingAmmo() <= 2) {
            Reload();
        }

        if (targetSpotted || beenAttacked) {
            IDamage target = targetSelection.SelectTarget();
            if (lineOfSight.CanSeeTarget(target)) {
                Debug.LogFormat("{0} opening fire on {1}", this, target.GetTransform());
                 StartCoroutine(PerformAttack());
            } else {
                FindCover(target.GetTransform().position, target.GetTransform().position - myTransform.position);
            }
        } else {
            Patrol();
        }
    }

    private IEnumerator PerformAttack() {
        while (weapon.GetCurrentFireCost() <= currentActionPoints) {
            Attack();

            if (weapon.GetRemainingAmmo() == 0) {
                Debug.LogFormat("{0} can't fire - reloading!", this);
                Reload();
            }
            yield return new WaitForSeconds(weapon.fireRate);
        }
        // if they can't attack any more, then they're done for this turn
        FinishedTurn();
    }
    

    /// <summary>
    /// Should only occur when moving to cover.
    /// </summary>
    public override void ReachedDestination() {
        base.ReachedDestination();
        IDamage target = targetSelection.SelectTarget();
        int attackCost = weapon.GetCurrentFireCost();
        if (lineOfSight.CanSeeTarget(target)) {
            Debug.LogFormat("{0} in cover at {1}! Opening fire on {2}", this, myTransform.position, target.GetTransform());
            TrackTarget(target);
            StartCoroutine(PerformAttack());
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
    /// <param name="attackDirection">Roughly form where they were attacked</param>
    public override void Damage(DamageType damageType, int damageAmount, Vector3 attackDirection) {
        base.Damage(damageType, damageAmount, attackDirection);
        if (health > 0) {
            beenAttacked = true;
        } else {
            Die();
        }
    }

    public override void FindCover(Vector3 position, Vector3 direction) {
        CoverResult target = coverFinder.FindCover(position, direction);
        if (Vector3.Distance(myTransform.position, target.GetPosition()) > 1f) {
            destinationTrigger.SetPosition(target.GetPosition());
            if (!SetDestination(target.GetPosition())) {
                Debug.LogFormat("{0} can't reach cover at {1}!", this, target);
                // at least crouching makes them harder to hit.
                Crouch();
                FinishedTurn();
            }
        } else {
            // if already in position, fire at will
            StartCoroutine(PerformAttack());
        }
    }

    protected override void TrackTarget(IDamage target) {
        Vector3 horizontalDir = target.GetTransform().position - myTransform.position;
        horizontalDir.y = 0;
        Vector3 desired = Vector3.RotateTowards(myTransform.forward, horizontalDir, navMeshAgent.angularSpeed * Time.deltaTime, 0f);
        myTransform.rotation = Quaternion.LookRotation(desired);
        weapon.transform.LookAt(target.GetTransform());
    }

    void Update() {
        if (targetSpotted) {
            IDamage target = targetSelection.SelectTarget();
            TrackTarget(target);
        }
    }

}
