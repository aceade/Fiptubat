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

    public float rotationSpeed = 20f;

    void Start () {
        myTransform = transform;
        unit = GetComponent<PlayerUnit>();
        myCamera = GetComponentInChildren<Camera>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void OnEnable() {
        if (myCamera == null) {
            myCamera = GetComponentInChildren<Camera>();
        }
        myCamera.enabled = true;
    }

    void OnDisable() {
        myCamera.enabled = false;
    }

    void Update() {

        myTransform.Rotate(0f, rotationSpeed * Input.GetAxis("Mouse X") * Time.deltaTime, 0f);

        // hold down the right mouse button to get paths/positions
        if (Input.GetButton("Fire2")) {
            Vector3 myPositoin = myTransform.position;
            Debug.DrawLine(myPositoin, myPositoin + (myCamera.transform.forward * maxDistance), Color.red);

            Vector3 possibleDestination;
            
            if (Physics.Raycast(myPositoin + Vector3.up, myTransform.forward, out raycastHit, maxDistance)) {
                // put arrow here to show where they're probably going and check the width
                // TODO: check for ladders or other interactable objects

                possibleDestination = raycastHit.point;
   
            } else {
                possibleDestination = myPositoin + (myTransform.forward * maxDistance);
            }

            // left click to select it
            Debug.LogFormat("Possible destination is {0}", possibleDestination);
            if (Input.GetButtonDown("Fire1")) {
                unit.SetDestination(possibleDestination);
            }

            
        }
    }
}