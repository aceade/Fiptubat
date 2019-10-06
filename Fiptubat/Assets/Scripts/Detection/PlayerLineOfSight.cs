using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aceade.AI {

    /// <summary>
    /// Player line of sight will be used for atmospheric purposes (e.g. announcing a hostil contact)
    /// </summary>
    public class PlayerLineOfSight : BasicLineOfSight {

		public override void OnTriggerEnter(Collider coll) {
            // no-op
        }

		public override void OnTriggerExit(Collider coll) {
            // no-op
        }

		public override void ClearColliders() {
            // no-op
        }
    }
}