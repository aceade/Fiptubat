using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls a player unit (rotation and selecting a position)
/// </summary>
public class PlayerUnitControl : MonoBehaviour {

    private NavMeshAgent navMeshAgent;

    private Camera myCamera;
    private Transform myTransform;

    private RaycastHit raycastHit;

    private PlayerUnit unit;

    public float maxDistance = 20f;

    void Start () {
        myTransform = transform;
        unit = GetComponent<PlayerUnit>();
        myCamera = GetComponentInChildren<Camera>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void OnEnable() {
        myCamera.enabled = true;
    }

    void OnDisable() {
        myCamera.enabled = false;
    }

    void Update() {

        // hold down the right mouse button to get paths/positions
        if (Input.GetButton("Fire 2")) {

            Debug.DrawRay(myTransform.position + Vector3.up, myCamera.transform.forward, Color.red);

            

            if (Physics.CapsuleCast(myTransform.position, myTransform.position + (Vector3.up * navMeshAgent.height), navMeshAgent.radius, myTransform.forward, maxDistance)) {
                // put arrow here to show where they're probably going and check the width




                // left click to select it
                if (Input.GetButtonDown("Fire 1")) {
                    unit.SetDestination(raycastHit.point);
                }
            }

            
        }
    }
}