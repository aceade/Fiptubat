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

		private Dictionary<Collider, IDamage> currentColliders = new Dictionary<Collider, IDamage>();

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
		}

		public void SetBrain(BaseUnit unit) 
		{
			this.brain = unit;
		}

		public virtual void OnTriggerEnter(Collider coll) 
		{
			if (!coll.isTrigger) {
				int layer = coll.transform.root.gameObject.layer;

				if (detectionLayers.Contains(layer) && !currentColliders.ContainsKey(coll)) {
					Debug.LogFormat("{0} should consider {1}", this, coll);
					var damageScript = coll.transform.root.GetComponent<IDamage>();
					currentColliders.Add(coll, damageScript);

					if (!isProcessing) {
						isProcessing = true;
						StartCoroutine(ProcessColliders());
					}
				}
			}
		}

		public virtual void OnTriggerExit(Collider coll)
		{
			if (currentColliders.ContainsKey(coll)) {
				currentColliders.Remove(coll);
			}
			if (currentColliders.Count == 0) {
				isProcessing = false;
			}
		}

		private IEnumerator ProcessColliders()
		{
			while (isProcessing) {
				foreach (Collider coll in currentColliders.Keys) {
					AnalyseTarget(coll);
				}
				yield return delay;
			}
			
		}

		private void AnalyseTarget(Collider coll) {
			RaycastHit hit;
			
			if (Physics.Raycast(transform.position, coll.transform.position - transform.position, out hit, maxDetectionRange, Physics.AllLayers, QueryTriggerInteraction.Ignore)) {
				if (hit.transform == coll.transform) {
					var damageScript = currentColliders[coll];
					brain.TargetSpotted(damageScript);
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
			if (!currentColliders.ContainsValue(target)) {
				return false;
			}
			else {
				return AnalyseTargetVisibility(target.GetTransform(), transform.position + viewingOffset);
			}
		}

		public bool CanSeeTargetFromLocation(IDamage target, Vector3 location) {
			if (!currentColliders.ContainsValue(target)) {
				return false;
			}
			else {
				return AnalyseTargetVisibility(target.GetTransform(), location);
			}
		}
	}
}