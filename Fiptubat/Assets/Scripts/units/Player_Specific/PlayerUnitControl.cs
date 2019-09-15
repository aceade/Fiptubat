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
    private Vector3 myPosition;

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

        myPosition = myTransform.position;
        myTransform.Rotate(0f, rotationSpeed * Input.GetAxis("Mouse X") * Time.deltaTime, 0f);
        myCamera.transform.Rotate(-rotationSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime, 0f, 0f);

        // hold down the right mouse button to get paths/positions
        bool selectingPath = Input.GetButton("Fire2");

        
        if (selectingPath) {
            Debug.DrawLine(myPosition, myPosition + (myCamera.transform.forward * maxDistance), Color.red);

            Vector3 possibleDestination;
            
            if (Physics.Raycast(myPosition + Vector3.up, myTransform.forward, out raycastHit, maxDistance)) {
                // TODO: put arrow here to show where they're probably going and check the width
                // TODO: check for ladders or other interactable objects

                possibleDestination = raycastHit.point;
   
            } else {
                possibleDestination = myPosition + (myTransform.forward * maxDistance);
            }

            // left click to select it
            Debug.LogFormat("Possible destination is {0}", possibleDestination);
            if (Input.GetButtonDown("Fire1")) {
                unit.SetDestination(possibleDestination);
            }

            
        }
    }
}