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
            //TrackTarget(target);
        } else {
            Patrol();
        }
    }

    void Patrol() {
        
    }

}
