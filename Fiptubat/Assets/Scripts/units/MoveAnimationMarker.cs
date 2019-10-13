using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class MoveAnimationMarker : MonoBehaviour {

    public enum MoveType {
        CROUCH,
        CLIMB,
        VAULT
    }

    private List<BaseUnit> currentUnits = new List<BaseUnit>();

    public MoveType moveType;

    void Start () {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider coll) {
        if (!coll.isTrigger) {
            var unit = coll.GetComponent<BaseUnit>();
            switch(moveType) {
                case MoveType.CROUCH:
                    Debug.LogFormat("{0} should crouch!", coll);
                    unit.CrouchAnimation(true);
                    break;
                case MoveType.VAULT:
                    Debug.LogFormat("{0} should vault!", coll);
                    unit.Vault();
                    break;
                case MoveType.CLIMB:
                    Debug.LogFormat("{0} should climb!", coll);
                    unit.Climb();
                    break;
            }
            
        }
    }

    void OnTriggerExit(Collider coll) {
        if (!coll.isTrigger) {
            var unit = coll.GetComponent<BaseUnit>();
            if (moveType == MoveType.CROUCH) {
                Debug.LogFormat("{0} should stand up!", coll);
                unit.CrouchAnimation(false);
            }
        }
    }
}