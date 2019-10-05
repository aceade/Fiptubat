using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aceade.AI {

	/// <summary>
	/// A basic line of sight implementation.
	/// </summary>
	public class BasicLineOfSight : MonoBehaviour, IDetection {

		public float detectionInterval = 0.2f;
		WaitForSeconds delay;


		// Use this for initialization
		void Start () 
		{
			delay = new WaitForSeconds (detectionInterval);
		}

		public void OnTriggerEnter(Collider coll) 
		{
			
		}

		public void OnTriggerExit(Collider coll)
		{
		}

		public IEnumerator ProcessColliders()
		{
			yield return delay;	
		}
	}
}