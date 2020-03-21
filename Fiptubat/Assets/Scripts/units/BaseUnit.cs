using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Aceade.AI;
using System.Linq;

/// <summary>
/// Common methods and properties for a unit.
/// </summary>
[RequireComponent(typeof(UnitTargetSelection))]
public class BaseUnit : MonoBehaviour, IDamage {

	public string unitName;

	public int health = 10;

	public int armour = 0;
	
	public int actionPoints = 50;
	protected int currentActionPoints;

	protected bool targetSpotted = false;

	protected bool beenAttacked = false;

	[Tooltip("This many shots hitting nearby will add a suppression effect")]
	public int suppressionCriteria = 10;
	private int suppressionCount = 0;

	[Tooltip("At what point should they announce that they missed")]
	public int tooManyMissedShotsCriteria = 3;

	private int shotsMissedThisTurn;
	
	public bool isCrouched = false;

	public float pathCostFactor = 2f;
	
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

	protected CoverFinder coverFinder;

	public UnitDestination destinationTrigger;
	
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
		coverFinder = GetComponent<CoverFinder>();
		isStillMoving = false;
		if (destinationTrigger != null) {
			destinationTrigger.SetUnit(this);
		}
	}

	/// <summary>
	/// Set the targeted destination, if possible
	/// </summary>
	/// <param name="newDestination">Coordinates of the endpoint</param>
	/// <returns>True if able to set the destination, false otherwise</returns>
	public bool SetDestination(Vector3 newDestination) {
		NavMeshPath path = new NavMeshPath();
		navMeshAgent.CalculatePath(newDestination, path);
		int potentialCost = GetMoveCost(GetPathLength(path));
		if (potentialCost <= GetRemainingActionPoints()) {
			targetLocation = newDestination;
			isStillMoving = true;
			navMeshAgent.path = path;
			voiceSystem.Moving();
			currentActionPoints -= potentialCost;
			return true;
		} else {
			Debug.LogFormat("{0}'s selected destination ({1}) is too far away!", unitName, newDestination);
			return false;
		}
		
	}

	public void StartTurn() {
		currentActionPoints = actionPoints;
		shotsMissedThisTurn = 0;
		if (suppressionCount >= suppressionCriteria) {
			Debug.LogFormat("{0} starting their turn suppressed", this);
			currentActionPoints /= 2;
			suppressionCount = 0;
		}
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

		if (currentActionPoints >= 2 ) {
			direction.y = 0f;
			myTransform.Translate(direction * Time.deltaTime);
			float distance = Vector3.Distance(myTransform.position, startPosition);
			// this seems to fire twice, so the cost is actually 4 points.
			// leave for now
			if (distance % 1f >= 0.99f) {
				currentActionPoints -= 2;
			}
		}
		
	}

	/// <summary>
	/// Find cover near my position that shields me in a particular direction
	/// </summary>
	/// <param name="position">My current position</param>
	/// <param name="direction">Direction from which I'm being shot or saw a target. Must start from my current position</param>
	public virtual void FindCover(Vector3 position, Vector3 direction) {
		CoverResult target = coverFinder.FindCover(position, direction);		
		destinationTrigger.SetPosition(target.GetPosition());
		if (!SetDestination(target.GetPosition())) {
			Debug.LogFormat("{0} can't reach cover at {1}!", this, target);
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
			targetSpotted = true;
			unitManager.AlertAllUnits(target);
			targetSelection.AddTarget(target);
			voiceSystem.TargetSpotted();
		}
		else if (target == null ) {
			targetSpotted = false;
		}
	}

	/// <summary>
	/// Used by "command" to let units know this has happened.
	/// </summary>
	/// <param name="target"></param>
	public virtual void TargetReported(IDamage target) {
		if (!targetSelection.AlreadyHasTarget(target)) {
			targetSelection.AddTarget(target);
		}
	}

	public virtual void TargetHeard(IDamage target) {
		voiceSystem.TargetHeard();
	}

	protected virtual void TrackTarget(IDamage target) {
		if (target == null) {
            targetSpotted = false;
        }

        Vector3 horizontalDir = target.GetTransform().position - myTransform.position;
        horizontalDir.y = 0;
        Vector3 desired = Vector3.RotateTowards(myTransform.forward, horizontalDir, navMeshAgent.angularSpeed * Time.deltaTime, 0f);
        myTransform.rotation = Quaternion.LookRotation(desired);
    }

	public virtual void Damage(DamageType damageType, int damageAmount) {
		beenAttacked = true;
		if (damageType != DamageType.ARMOUR_PIERCING) {
			damageAmount -= armour;
		} 

		health -= damageAmount;
		if (health <= 0) {
			Die();
		}
	}

	public void HitNearby() {
		suppressionCount++;
		if (suppressionCount >= suppressionCriteria) {
			Debug.LogFormat("{0}: Help, help, I'm being suppressed!", this);
			currentActionPoints = 0;
			voiceSystem.NearlyHit();
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
					shotsMissedThisTurn++;
					if (shotsMissedThisTurn > tooManyMissedShotsCriteria || weapon.UsingMostAccurateAttack()) {
						voiceSystem.TargetMissed();
					}
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
				shotsMissedThisTurn = 0;
			}
		}
	}

	public bool IsWeaponReady() {
		return weapon.CanAttack();
	}

	protected virtual void Die() {
		Debug.LogFormat("{0} is dead!", this.unitName);
		voiceSystem.Die();
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

	public float GetPathLength() {
		if (navMeshAgent.hasPath) {
			return GetPathLength(navMeshAgent.path);
		} else {
			return 0f;
		}
	}

	public float GetPathLength(NavMeshPath path) {
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
			return lengthSoFar * pathCostFactor;
	}

	public virtual void ReachedDestination() {
		// no-op for Turret or PatrolBot
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
