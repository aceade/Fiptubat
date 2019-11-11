using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Patrols along a path until out of points
/// <summary>
public class PatrolBot : BaseUnit
{

    public List<PatrolPoint> patrolRoute;
    private int patrolIndex;

    protected override void Start()
    {
        base.Start();
    }

    void Patrol() {

    }

    private void IncrementPatrolIndex() {
        patrolIndex++;
        if (patrolIndex >= patrolRoute.Count) {
            patrolIndex = 0;
        }
        SetDestination(patrolRoute[patrolIndex].GetPosition());
    }

    public override void ReachedPatrolPoint(PatrolPoint point) {
        if (point == patrolRoute[patrolIndex]) {
            IncrementPatrolIndex();
        }
    }
}
