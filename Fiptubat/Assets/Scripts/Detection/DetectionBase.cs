using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Aceade.AI {
	
	/// <summary>
	/// Defines how detection will work
	/// </summary>
	public interface IDetection {

		void OnTriggerEnter(Collider coll);

		void OnTriggerExit(Collider coll);

		IEnumerator ProcessColliders();
	}

}