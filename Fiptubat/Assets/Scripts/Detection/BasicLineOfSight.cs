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

		private List<Collider> currentColliders = new List<Collider>();

		public List<int> detectionLayers;

		protected bool isProcessing = false;

		protected BaseUnit brain;

		// Use this for initialization
		void Start () 
		{
			Collider coll = GetComponent<Collider>();
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

				if (detectionLayers.Contains(layer) && !currentColliders.Contains(coll)) {
					Debug.LogFormat("{0} should consider {1}", this, coll);
					currentColliders.Add(coll);

					if (!isProcessing) {
						isProcessing = true;
						StartCoroutine(ProcessColliders());
					}
				}
			}
		}

		public virtual void OnTriggerExit(Collider coll)
		{
			if (currentColliders.Contains(coll)) {
				currentColliders.Remove(coll);
			}
			if (currentColliders.Count == 0) {
				isProcessing = false;
			}
		}

		private IEnumerator ProcessColliders()
		{
			while (isProcessing) {
				currentColliders.ForEach(target => AnalyseTarget(target));
				yield return delay;
			}
			
		}

		private void AnalyseTarget(Collider coll) {
			RaycastHit hit;
			Debug.LogFormat("{0} analysing LOS for {1}", brain.unitName, coll);
			if (Physics.Raycast(transform.position, coll.transform.position - transform.position, out hit, 20f)) {
				Debug.LogFormat("{0} can see {1}. Is this the target? {2}", brain.unitName, hit.transform, hit.transform == coll.transform);
				if (hit.transform == coll.transform) {
					var damageScript = coll.GetComponent<IDamage>();
					brain.TargetSpotted(damageScript);
				}
			}
		}

		public virtual void ClearColliders() 
		{
			isProcessing = false;
		}
	}
}