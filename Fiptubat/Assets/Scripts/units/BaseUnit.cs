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
	protected int currentActionPoints;
	
	public bool isCrouched = false;
	
	protected NavMeshAgent navMeshAgent;

	protected UnitManager unitManager;

	private NavMeshHit navMeshHit;

	protected bool isStillMoving;

	protected bool isSelected;

	protected Rigidbody myBody;

	protected BasicLineOfSight lineOfSight;

	protected Vector3 targetLocation;

	protected UnitVoiceSystem voiceSystem;

	protected UnitTargetSelection targetSelection;

	protected Transform myTransform;

	protected WeaponBase weapon;
	
	protected virtual void Start() {
		myTransform = transform;
		currentActionPoints = actionPoints;
		lineOfSight = GetComponentInChildren<BasicLineOfSight>();
		lineOfSight.SetBrain(this);
		voiceSystem = GetComponentInChildren<UnitVoiceSystem>();
		myBody = GetComponent<Rigidbody>();
		navMeshAgent = GetComponent<NavMeshAgent>();
		targetSelection = GetComponent<UnitTargetSelection>();
		weapon = GetComponentInChildren<WeaponBase>();
		isStillMoving = false;
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
			targetLocation = newDestination;
			isStillMoving = true;
			navMeshAgent.SetDestination(newDestination);
			voiceSystem.Moving();
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
		//isStillMoving = true;
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

	/// <summary>
	/// Select the current unit.
	/// </summary>
	/// <param name="isMyTurn">Only used in player units. If <code>false</code>, they can't do anything but look around</param>
	public virtual void SelectUnit(bool isMyTurn) {
		isSelected = true;
        // no-op
    }

	public virtual void DeselectUnit() {
		isSelected = false;
        // no-op
    }

	/// <summary>
	/// Sidestep in a particular direction to step into/out of cover.
	/// </summary>
	/// <param name="startPosition">Where they start</param>
	/// <param name="direction">The direction in which to step</param>
	public void SideStep(Vector3 startPosition, Vector3 direction) {
		direction.y = 0f;
		myTransform.Translate(direction * Time.deltaTime);
		float distance = Vector3.Distance(myTransform.position, startPosition);
		if (distance >= 1.5f) {
			myTransform.Translate(direction * -0.02f);
		}
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
		weapon.ToggleCrouch(isCrouched);
	}

	/// <summary>
	/// Separate animations from code
	/// </summary>
	/// 
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

	public virtual void TargetSpotted(IDamage target) {
		if (target != null && !targetSelection.AlreadyHasTarget(target)) {
			targetSelection.AddTarget(target);
			voiceSystem.TargetSpotted();
		}
		
	}

	public virtual void TargetHeard(IDamage target) {
		voiceSystem.TargetHeard();
	}

	public void Damage(DamageType damageType, int damageAmount) {
		if (damageType != DamageType.ARMOUR_PIERCING) {
			damageAmount -= armour;
		} 

		health -= damageAmount;
		if (health <= 0) {
			voiceSystem.Die();
			Die();
		} else {
			voiceSystem.Hit();
		}
	}

	public int GetRemainingHealth() {
		return health;
	}

	public int GetPotentialDamage() {
		return weapon.damage;
	}

	public Transform GetTransform() {
		return transform;
	}

	public Vector3 GetCurrentDestination() {
		return targetLocation;
	}

	public int GetCurrentActionPoints() {
		return currentActionPoints;
	}


	public void SetUnitManager(UnitManager manager) {
		DeselectUnit();
		this.unitManager = manager;
	}

	public virtual void Attack() {
		int attackCost = weapon.GetCurrentFireCost();
		if (IsWeaponReady() && currentActionPoints > 0 && currentActionPoints >= attackCost) {
			
			if (!weapon.Fire()) {
				if (weapon.GetRemainingAmmo() == 0) {
					voiceSystem.OutOfAmmo();
				} else {
					voiceSystem.TargetMissed();
				}
			}
			currentActionPoints -= attackCost;
		}
		
	}

	public void ChangeFireMode() {
		weapon.CycleFireMode();
	}

	public void Reload() {
		if (weapon.GetRemainingAmmo() < weapon.magSize) {
			int reloadCost = weapon.reloadCost;
			if (currentActionPoints > 0 && currentActionPoints >= reloadCost) {
				voiceSystem.Reloading();
				weapon.Reload();
				currentActionPoints -= reloadCost;
			}
		}
	}

	public bool IsWeaponReady() {
		return weapon.CanAttack();
	}

	protected virtual void Die() {
		Debug.LogFormat("{0} is dead!", this.unitName);
		lineOfSight.ClearColliders();
		lineOfSight.StopAllCoroutines();
		lineOfSight.enabled = false;
		targetSelection.enabled = false;
		unitManager.UnitDied(this);
		this.enabled = false;
	}

	public int GetMoveCost(Vector3 start, Vector3 destination) {
		return GetMoveCost(Vector3.Distance(start, destination));
	}

	public int GetMoveCost(float pathDistance) {
		int baseCost = Mathf.RoundToInt(pathDistance) / 2;
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

	public virtual void ReachedPatrolPoint(PatrolPoint point) {
		// no-op for most units
	}


	protected float GetPathLength() {
		if (navMeshAgent.hasPath) {
			NavMeshPath path = navMeshAgent.path;
			if (path.corners.Length < 2) {
				return 0f;
			}
			// stolen from https://docs.unity3d.com/540/Documentation/ScriptReference/NavMeshPath-corners.html
			Vector3 previousCorner = path.corners[0];
			float lengthSoFar = 0.0F;
			int i = 1;
			while (i < path.corners.Length) {
				Vector3 currentCorner = path.corners[i];
				lengthSoFar += Vector3.Distance(previousCorner, currentCorner);
				previousCorner = currentCorner;
				i++;
			}
			return lengthSoFar;
		} else {
			return 0f;
		}
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
