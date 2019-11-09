using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracerEffect : MonoBehaviour
{
    private Transform myTransform;

    private Rigidbody myBody;

    private MeshRenderer myRenderer;

    public float velocity = 2f;

    void Start()
    {
        myTransform = transform;
        myBody = GetComponent<Rigidbody>();
        myRenderer = GetComponent<MeshRenderer>();
        toggleDisplay(false);
    }

    private void toggleDisplay(bool show) {
        myRenderer.enabled = show;
        myBody.isKinematic = !show;
    }

    public void Launch(Vector3 startPosition, Vector3 fireDirection) {
        myTransform.position = startPosition;
        myTransform.forward = fireDirection;
        toggleDisplay(true);
        myBody.AddForce(myTransform.forward * velocity, ForceMode.VelocityChange);
    }

    void OnCollisionEnter(Collision coll) {
        toggleDisplay(false);
        // could be used for suppression later?
    }
}
