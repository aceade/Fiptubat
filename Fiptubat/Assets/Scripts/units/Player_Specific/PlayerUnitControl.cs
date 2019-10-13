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

    public UIManager uiManager;

    public PlayerMoveMarker moveMarker;

    public float maxDistance = 20f;

    public float rotationSpeed = 20f;

    private Vector3 lastStationaryPosition;

    private bool canMove = true;

    void Start () {
        myTransform = transform;
        unit = GetComponent<PlayerUnit>();
        myCamera = GetComponentInChildren<Camera>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        lastStationaryPosition = myTransform.position;
    }

    void OnEnable() {
        if (myCamera == null) {
            myCamera = GetComponentInChildren<Camera>();
        }
        myCamera.enabled = true;
        canMove = true;
    }

    void OnDisable() {
        myCamera.enabled = false;
        canMove = false;
    }

    public void MoveCamera(float yOffset) {
        myCamera.transform.Translate(0f, yOffset, 0f);
    }

    public void AllowMovement() {
        canMove = true;
        myCamera.transform.forward = myTransform.forward;
        Debug.LogFormat("{0} should be able to move now", unit.unitName);
    }

    public void ForbidMovement() {
        canMove = false;
        Debug.LogFormat("{0} should NOT be able to move now", unit.unitName);
    }

    void Update() {

        myPosition = myTransform.position;
        myTransform.Rotate(0f, rotationSpeed * Input.GetAxis("Mouse X") * Time.deltaTime, 0f);
        myCamera.transform.Rotate(-rotationSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime, 0f, 0f);

        // allow arrow keys to rotate (Input.GetAxis has limits on Linux)
        // TODO: move ALL keycodes into InputManager
        if (Input.GetKey(KeyCode.LeftArrow)) {
            myTransform.Rotate(-Vector3.up * rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            myTransform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.UpArrow)) {
            myCamera.transform.Rotate(-rotationSpeed * Time.deltaTime, 0f, 0f);
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            myCamera.transform.Rotate(rotationSpeed * Time.deltaTime, 0f, 0f);
        }

        // hold down the right mouse button to get paths/positions
        bool selectingPath = Input.GetButton("Fire2") && canMove;
        HandlePath(selectingPath);
        
        if(Input.GetKeyDown(KeyCode.C)) {
            unit.Crouch();
        }

        if (Input.GetKeyDown(KeyCode.Tab)) {
            uiManager.CycleUnit();
            canMove = false;
        }
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            uiManager.EndTurn();
            canMove = false;
        }
        
    }

    private void HandlePath(bool selectingPath) {
        if (selectingPath) {
            Debug.DrawLine(myPosition, myPosition + (myCamera.transform.forward * maxDistance), Color.red);

            Vector3 possibleDestination;
            
            if (Physics.Raycast(myPosition + Vector3.up, myCamera.transform.forward, out raycastHit, maxDistance)) {
                possibleDestination = raycastHit.point;
   
            } else {
                possibleDestination = myPosition + (myCamera.transform.forward * maxDistance);
            }
            moveMarker.SetPosition(possibleDestination, true);

            // show the player how much it costs
            int moveCost = unit.GetMoveCost(myPosition, raycastHit.point);
            int remainingPoints = unit.GetRemainingActionPoints();
            float distance = Vector3.Distance(myPosition, raycastHit.point);
            uiManager.ShowDistanceCost(distance, moveCost, remainingPoints);

            // left click to select it
            if (Input.GetButtonDown("Fire1")) {
                lastStationaryPosition = myPosition;
                bool canReachDestination = unit.SetDestination(possibleDestination);
                moveMarker.SetPosition(possibleDestination, canReachDestination);
            }
            
        } else {
            uiManager.ClearDistanceText();
            moveMarker.Hide();
        }
    }
}