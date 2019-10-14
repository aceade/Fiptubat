using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Aceade.AI;

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

	protected bool isStillMoving;

	protected bool isSelected;

	private Rigidbody myBody;

	protected BasicLineOfSight lineOfSight;
	
	protected virtual void Start() {
		currentActionPoints = actionPoints;
		lineOfSight = GetComponentInChildren<BasicLineOfSight>();
		lineOfSight.SetBrain(this);
		myBody = GetComponent<Rigidbody>();
		navMeshAgent = GetComponent<NavMeshAgent>();
	}

	private void LogPath() {
		bool hasPath = navMeshAgent.hasPath;
		Debug.LogFormat("[{0}] moving to [{1}]. Does it have a path: [{2}]. What status is the path in: [{3}]. Remaining distance: [{4}]. Is it actually stopped? [{5}]", 
		unitName, navMeshAgent.destination, 
			hasPath,  navMeshAgent.path.status, 
			navMeshAgent.remainingDistance, navMeshAgent.isStopped);
	}
	
	/// <summary>
	/// Set the targeted destination, if possible
	/// </summary>
	/// <param name="newDestination">Coordinates of the endpoint</param>
	/// <returns>True if able to set the destination, false otherwise</returns>
	public bool SetDestination(Vector3 newDestination) {
		int potentialCost = GetMoveCost(transform.position, newDestination);
		if (potentialCost <= GetRemainingActionPoints()) {
			navMeshAgent.SetDestination(newDestination);
			currentActionPoints -= potentialCost;
			LogPath();
			return true;
		} else {
			Debug.LogFormat("{0}'s selected destination ({1}) is too far away!", unitName, newDestination);
			return false;
		}
		
	}

	public void StartTurn() {
		currentActionPoints = actionPoints;
		isStillMoving = true;
	}

	protected virtual void FinishedTurn() {
		isStillMoving = false;
		unitManager.UnitFinished(this);
	}

	public virtual bool IsStillMoving() {
		return isStillMoving;
	}

	public bool IsSelected() {
		return isSelected;
	}

	public virtual void SelectUnit() {
		isSelected = true;
        // no-op
    }

	public virtual void DeselectUnit() {
		isSelected = false;
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
		CrouchAnimation(isCrouched);
	}

	/// <summary>
	/// Separate animations from code
	/// </summary>
	public virtual void CrouchAnimation(bool crouched) {
		if (crouched) {
			// TODO: cache the collider and add animation class.
			GetComponent<CapsuleCollider>().height = 1f;
		} else {
			GetComponent<CapsuleCollider>().height = 2f;
		}
	}

	public virtual void Vault() {
		GetComponent<Rigidbody>().AddForce(Vector3.up * 2f, ForceMode.Impulse);
		// will need animations
	}

	public virtual void Climb() {
		// no-op - will need animations
	}

	public virtual void StandDown() {
		// no-op. Mainly used to tell player they can't move!
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

	public Transform GetTransform() {
		return transform;
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

	
	void OnCollisionEnter(Collision coll) {
		int layer = coll.gameObject.layer;

		// avoid being repelled by steps
		if (layer == LayerMask.NameToLayer("Scenery") && !coll.collider.isTrigger) {
			Debug.LogFormat("{0} trying not to suffer Doorstep Repulsion Effect from {1}", unitName, coll.gameObject);
			myBody.isKinematic = true;
			myBody.isKinematic = false;	
		}
	}

}
