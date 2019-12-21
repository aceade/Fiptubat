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

    private PlayerUnitDisplay unitDisplay;

    public UIManager uiManager;

    public PlayerMoveMarker moveMarker;

    public float maxDistance = 20f;

    private float rotationSpeed;

    private Vector3 lastStationaryPosition, destination;

    private bool canMove = true;

    private bool usingUI = false;

    void Start () {
        myTransform = transform;
        unit = GetComponent<PlayerUnit>();
        unitDisplay = GetComponent<PlayerUnitDisplay>();
        myCamera = GetComponentInChildren<Camera>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        lastStationaryPosition = myTransform.position;
        rotationSpeed = PlayerPrefs.GetFloat("CameraSpeed", 20f);
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

        usingUI = Input.GetButton("ToggleUi");
        unitDisplay.ToggleUsingUi(usingUI);

        if (!usingUI) {
            myPosition = myTransform.position;
            myTransform.Rotate(0f, rotationSpeed * Input.GetAxis("Mouse X") * Time.deltaTime, 0f);
            myCamera.transform.Rotate(-rotationSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime, 0f, 0f);

            // allow arrow keys to rotate (Input.GetAxis has limits on Linux)
            myTransform.Rotate(-Vector3.up * rotationSpeed * Input.GetAxis("Horizontal") * Time.deltaTime);
            myCamera.transform.Rotate(-rotationSpeed * Input.GetAxis("Vertical") * Time.deltaTime, 0f, 0f);

            // hold down the right mouse button to get paths/positions
            bool selectingPath = Input.GetButton("Fire2") && canMove;
            HandlePath(selectingPath);
            
            if(Input.GetButtonDown("Crouch")) {
                unit.Crouch();
            }

            if (Input.GetButtonDown("CycleUnit")) {
                uiManager.CycleUnit();
                canMove = false;
            }
            if (Input.GetButtonDown("EndTurn")) {
                uiManager.EndTurn();
                canMove = false;
            }

            if (!unit.IsStillMoving() && canMove) {
                bool steppingLeft = Input.GetButton("StepLeft");
                bool steppingRight = Input.GetButton("StepRight");

                // side-step. For some reason, this has to be "Vector3.left" or Vector3.right!
                // navigation appears to interfere with this
                if (steppingLeft) {
                    unit.SideStep(lastStationaryPosition, Vector3.left);
                    navMeshAgent.enabled = false;
                }
                else if (steppingRight) {
                    unit.SideStep(lastStationaryPosition, Vector3.right);
                    navMeshAgent.enabled = false;
                } else {
                    navMeshAgent.enabled = true;
                    RevertToStationary();
                }

                if (Input.GetButtonDown("Fire1") && unit.IsWeaponReady() && !selectingPath) {
                    unit.Attack();
                }

                if (Input.GetButtonDown("Reload")) {
                    unit.Reload();
                }

                if (Input.GetButtonDown("CycleFireMode")) {
                    unit.ChangeFireMode();
                }
                
                
            } else {
                // if close to their destination, mark this as their last stationary position
                Vector3 displacement = destination-myPosition;
                displacement.y = 0;
                if (displacement.magnitude <= 1f) {
                    lastStationaryPosition = myPosition;
                }
            }
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
                destination = possibleDestination;
                bool canReachDestination = unit.SetDestination(possibleDestination);
                moveMarker.SetPosition(possibleDestination, canReachDestination);
            }
            
        } else {
            uiManager.ClearDistanceText();
            moveMarker.Hide();
        }
    }

    /// <summary>
    /// Undo sidestep. Note to self: do NOT use a while loop here!
    /// <summary>
    private void RevertToStationary() {
        Vector3 displacement = lastStationaryPosition - myPosition;
        float distance = displacement.magnitude;
        if (distance > 0.1f) {
            myTransform.Translate(displacement * Time.deltaTime);
        }
    }

    /// <summary>
    /// Check if the player unit is using the UI
    /// <summar>
    public bool isUsingUi() {
        return usingUI;
    }
}