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

    private bool hasReachedDestination = true;

    public UIManager uiManager;

    public PlayerMoveMarker moveMarker;

    public float maxDistance = 20f;

    private float rotationSpeed;

    private float upAngle;

    private Vector3 destination;

    private bool canMove = true;

    private bool usingUI = true;

    private bool attacking = false;
    private bool reloading = false;

    public LineRenderer pathDisplay;

    private bool sideStepping;
    private bool strafingForward;

    private bool selectingPath;

    private Transform gunBase;

    void Start () {
        myTransform = transform;
        unit = GetComponent<PlayerUnit>();
        unitDisplay = GetComponent<PlayerUnitDisplay>();
        myCamera = GetComponentInChildren<Camera>();
        upAngle = myCamera.transform.rotation.x;
        navMeshAgent = GetComponent<NavMeshAgent>();
        rotationSpeed = PlayerPrefs.GetFloat("CameraSpeed", 20f);
        unitDisplay.ToggleUsingUi(usingUI);
        gunBase = GetComponentInChildren<WeaponBase>().transform;
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

    public void AllowMovement() {
        canMove = true;
        myCamera.transform.forward = myTransform.forward;
        Debug.LogFormat("{0} should be able to move now", unit.unitName);
    }

    public void ForbidMovement() {
        canMove = false;
        Debug.LogFormat("{0} should NOT be able to move now", unit.unitName);
        usingUI = true;
    }

    void Update() {

        if (Input.GetButtonDown("ToggleUi")) {
            usingUI = !usingUI;
            unitDisplay.ToggleUsingUi(usingUI);
            uiManager.ToggleUiStatus(usingUI);
        }

        // default keymapping for Escape
        if (Input.GetButtonDown("Cancel")) {
            uiManager.Pause();
        }

        if (!usingUI && !uiManager.isPaused()) {

            // hold down the right mouse button to get paths/positions
            selectingPath = Input.GetButton("Fire2") && canMove;
            HandlePath(selectingPath);
            unit.SetPathStatus(selectingPath);

            myPosition = myTransform.position;
            myTransform.Rotate(0f, rotationSpeed * Input.GetAxis("Mouse X") * Time.deltaTime, 0f);
            myTransform.Rotate(-Vector3.up * rotationSpeed * Input.GetAxis("Horizontal") * Time.deltaTime);

            // use a consistent angle
            upAngle += (Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime);
            upAngle += (Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime);

            if (!selectingPath && hasReachedDestination) {
                unit.RotateVertically(upAngle);
            } else {
                myCamera.transform.Rotate(-rotationSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime, 0f, 0f);
                myCamera.transform.Rotate(-rotationSpeed * Input.GetAxis("Vertical") * Time.deltaTime, 0f, 0f);
            }
            

            if (Input.GetButtonDown("CycleUnit")) {
                uiManager.CycleUnit();
                canMove = false;
            }
            if (Input.GetButtonDown("EndTurn")) {
                uiManager.EndTurn();
                canMove = false;
            }

            if (hasReachedDestination && canMove) {

                            
                if(Input.GetButtonDown("Crouch")) {
                    unit.Crouch();
                }
                
                float sidestepSpeed = Input.GetAxisRaw("Sidestep");
                float frontstepSpeed = Input.GetAxisRaw("Step");

                // effectively equivalent to "GetAxisDown" - only sidestep if pressing, 
                // but not holding the buttons
                if (Mathf.Abs(sidestepSpeed) > 0f) {
                    if (!sideStepping) {
                        sideStepping = true;
                        unit.SideStep(myTransform.right * -sidestepSpeed);
                    }
                }
                else {
                    sideStepping = false;
                }

                if (Mathf.Abs(frontstepSpeed) > 0f) {
                    if (!strafingForward) {
                        strafingForward = true;
                        unit.SideStep(myTransform.forward * frontstepSpeed);
                    }
                }
                else {
                    strafingForward = false;
                }

                attacking = Input.GetButtonDown("Fire1");
                reloading = Input.GetButtonDown("Reload");

                if (attacking && unit.IsWeaponReady() && !selectingPath) {
                    unit.Attack();
                }

                if (reloading) {
                    unit.Reload();
                }

                if (Input.GetButtonDown("CycleFireMode")) {
                    unit.ChangeFireMode();
                }
            }
        }
    }

    void LateUpdate() {
        if (hasReachedDestination) {
            // keep the camera aimed along the gun barrel
            if(!attacking && !reloading && !selectingPath) {
                Vector3 gunForward = gunBase.forward;
                myCamera.transform.forward = gunForward;
            }
        } 
    }

    private void HandlePath(bool selectingPath) {
        if (selectingPath) {

            Vector3 possibleDestination;
            
            if (Physics.Raycast(myPosition + Vector3.up, myCamera.transform.forward, out raycastHit, maxDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore)) {
                possibleDestination = (raycastHit.point);
   
            } else {
                possibleDestination = myPosition + (myCamera.transform.forward * maxDistance);
            }

            NavMeshPath tempPath = new NavMeshPath();
            bool canFindPath = navMeshAgent.CalculatePath(possibleDestination, tempPath);
            moveMarker.SetPosition(possibleDestination, false);
            if (canFindPath) {
                moveMarker.SetPosition(possibleDestination, true);
                // show the player how much it costs
                DisplayPath(tempPath);
                float pathLength = unit.GetPathLength(tempPath);
                int moveCost = unit.GetMoveCost(pathLength);
                int remainingPoints = unit.GetRemainingActionPoints();
                uiManager.ShowMoveCost(moveCost, remainingPoints);

                // left click to select it
                if (Input.GetButtonDown("Fire1")) {
                    destination = possibleDestination;
                    bool canReachDestination = unit.SetDestination(possibleDestination);
                    moveMarker.SetPosition(possibleDestination, canReachDestination);
                    if (canReachDestination) {
                        pathDisplay.enabled = false;
                        ReachedDestination(false);
                    }
                }
            }
        } else {
            uiManager.ClearDistanceText();
            moveMarker.Hide();
        }
    }

    void DisplayPath(NavMeshPath path) {
        pathDisplay.enabled = true;
        pathDisplay.positionCount = path.corners.Length;
        pathDisplay.SetPositions(path.corners);
    }

    /// <summary>
    /// Check if the player unit is using the UI
    /// <summary>
    public bool isUsingUi() {
        return usingUI;
    }

    public void ReachedDestination(bool reached) {
        hasReachedDestination = reached;
        navMeshAgent.updateRotation = !reached;
    }

}