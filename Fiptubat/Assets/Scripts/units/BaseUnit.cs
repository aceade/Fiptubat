using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Common methods and properties for a unit.
/// </summary>
public class BaseUnit : MonoBehaviour, IDamage {

	public string unitName;

	public int health = 10;

	public int armour = 0;
	
	public int actionPoints = 50;
	private int currentActionPoints;
	
	public bool isCrouched = false;
	
	protected NavMeshAgent navMeshAgent;

	protected UnitManager unitManager;

	private NavMeshHit navMeshHit;
	
	protected virtual void Start() {
		currentActionPoints = actionPoints;
		navMeshAgent = GetComponent<NavMeshAgent>();
	}

	private void LogPath() {
		bool hasPath = navMeshAgent.hasPath;
		Debug.LogFormat("[{0}] moving to [{1}]. Does it have a path: [{2}]. What status is the path in: [{3}]. Remaining distance: [{4}]. Is it actually stopped? [{5}]", 
		unitName, navMeshAgent.destination, 
			hasPath,  navMeshAgent.path.status, 
			navMeshAgent.remainingDistance, navMeshAgent.isStopped);
	}
	
	public void SetDestination(Vector3 position) {
		navMeshAgent.SetDestination(position);
		currentActionPoints -= GetMoveCost(transform.position, position);
		LogPath();
		
	}

	public void StartTurn() {
		currentActionPoints = actionPoints;
	}

	public virtual void SelectUnit() {
        // no-op
    }

	public virtual void DeselectUnit() {
        // no-op
    }

	public virtual void FindCover(Vector3 position, Vector3 direction) {
		// find the closest edge

		if (navMeshAgent.FindClosestEdge(out navMeshHit)) {
			SetDestination(navMeshHit.position);
		} else {
			Debug.LogFormat("{0} cannot find cover!", unitName);
		}
	}

	/// <summary>
	/// Crouching will result in greater move costs, increased accuracy, 
	/// and decreased chance to be hit.
	/// </summary>
	public virtual void Crouch() {
		isCrouched = !isCrouched;
		currentActionPoints -= 4;
		if (isCrouched) {
			navMeshAgent.height = 1f;
			// TODO: cache the collider
			GetComponent<CapsuleCollider>().height = 1f;
		} else {
			navMeshAgent.height = 2f;
			GetComponent<CapsuleCollider>().height = 2f;
		}
	}

	public virtual void TargetLocated(IDamage target) {
		// no-op
	}

	public void Damage(DamageType damageType, int damageAmount) {
		if (damageType != DamageType.ARMOUR_PIERCING) {
			damageAmount -= armour;
		} 

		health -= damageAmount;
		if (health <= 0) {
			Die();
		}
	}

	public void SetUnitManager(UnitManager manager) {
		DeselectUnit();
		this.unitManager = manager;
	}

	protected virtual void Die() {
		Debug.LogFormat("{0} is dead!", this.unitName);
		unitManager.UnitDied(this);
	}

	public int GetMoveCost(Vector3 start, Vector3 destination) {
		int baseCost = Mathf.RoundToInt((Vector3.Distance(start, destination) / 2));
		if (isCrouched) {
			return Mathf.RoundToInt(baseCost * 1.5f);
		}
		return baseCost;
	}

	public int GetMaxMoveDistance() {
		int baseDistance = actionPoints / 2;
		if (isCrouched) {
			return Mathf.RoundToInt(baseDistance / 1.5f);
		}
		return baseDistance;
	}

	public int GetRemainingActionPoints() {
		return currentActionPoints;
	}
}
