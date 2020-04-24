using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aceade.AI {

	/// <summary>
	/// A basic line of sight implementation.
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class BasicLineOfSight : MonoBehaviour, IDetection {

		public float detectionInterval = 0.2f;
		WaitForSeconds delay;

		private List<IDamage> currentColliders = new List<IDamage>();

		public List<int> detectionLayers;

		protected bool isProcessing = false;

		protected BaseUnit brain;

		private Collider coll;

		public float maxDetectionRange = 30f;

		public Vector3 viewingOffset = Vector3.up;

		// Use this for initialization
		void Start () 
		{
			coll = GetComponent<Collider>();
			coll.isTrigger = true;
			delay = new WaitForSeconds (detectionInterval);

			// ignore subcolliders
			Collider[] childrenCollders = transform.root.GetComponentsInChildren<Collider>();
			for (int i = 0; i < childrenCollders.Length; i++) {
				Physics.IgnoreCollision(coll, childrenCollders[i]);
			}
		}

		public void SetBrain(BaseUnit unit) 
		{
			this.brain = unit;
		}

		public virtual void OnTriggerEnter(Collider coll) 
		{
			if (!coll.isTrigger) {
				int layer = coll.transform.root.gameObject.layer;

				if (detectionLayers.Contains(layer)) {
					var damageScript = coll.transform.root.GetComponent<IDamage>();
					if (damageScript != null && !currentColliders.Contains(damageScript)) {
						Debug.LogFormat("{0} should consider {1}", this, damageScript);
						currentColliders.Add(damageScript);

						if (!isProcessing) {
							isProcessing = true;
							StartCoroutine(ProcessColliders());
						}
					}
					
				}
			}
		}

		public virtual void OnTriggerExit(Collider coll)
		{
			var damageScript = coll.transform.root.GetComponent<IDamage>();
			if (damageScript != null && currentColliders.Contains(damageScript)) {
				currentColliders.Remove(damageScript);
			}
			if (currentColliders.Count == 0) {
				isProcessing = false;
			}
		}

		private IEnumerator ProcessColliders()
		{
			while (isProcessing) {
				foreach (IDamage target in currentColliders) {
					AnalyseTarget(target);
				}
				yield return delay;
			}
			
		}

		private void AnalyseTarget(IDamage target) {
			RaycastHit hit;
			
			if (Physics.Raycast(transform.position, target.GetTransform().position - transform.position, out hit, maxDetectionRange, Physics.AllLayers, QueryTriggerInteraction.Ignore)) {
				Debug.DrawRay(transform.position, hit.point - transform.position, Color.blue, 3f);
				if (hit.transform == target.GetTransform()) {
					brain.TargetSpotted(target);
				}
			}
		}

		/// <summary>
		/// Check if I can see the specific target from a specific location
		/// </summary>
		/// <param name="target">The target I'm looking for</param>
		/// <param name="fromPosition">Either where I'm standing, or another position entirely</param>
		/// <returns></returns>
		private bool AnalyseTargetVisibility(Transform target, Vector3 fromPosition) {
			Debug.LogFormat("{0} analysing {1} visibility from {2}", this, target, fromPosition);
			RaycastHit hit;
			bool canSeeTarget = false;
			
			if (Physics.Raycast(fromPosition, target.position - fromPosition, out hit, maxDetectionRange, Physics.AllLayers, QueryTriggerInteraction.Ignore)) {
				Debug.DrawRay(fromPosition, hit.point - fromPosition, Color.yellow, 2f);
				Debug.LogFormat("Examining target {0}, saw {1} with root {2}", target, hit.transform, hit.transform.root);
				if (hit.transform.root == target) {
					canSeeTarget = true;
				}
			}
			return canSeeTarget;
		}

		public virtual void ClearColliders() 
		{
			isProcessing = false;
			currentColliders.Clear();
		}

		void OnDisable() {
			coll.enabled = false;
		}

		/// <summary>
		/// Check if we can currently see the target.
		/// Might be better to create a wrapper class for detection results?
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public bool CanSeeTarget(IDamage target) {
			if (!currentColliders.Contains(target)) {
				return false;
			}
			else {
				return AnalyseTargetVisibility(target.GetTransform(), transform.position + viewingOffset);
			}
		}

		public bool CanSeeTargetFromLocation(IDamage target, Vector3 location) {
			if (!currentColliders.Contains(target)) {
				return false;
			}
			else {
				return AnalyseTargetVisibility(target.GetTransform(), location);
			}
		}
	}
}