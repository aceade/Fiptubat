using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracerEffect : MonoBehaviour
{
    private Transform myTransform;

    private Rigidbody myBody;

    private MeshRenderer myRenderer;

    public float velocity = 2f;

    public float radius = 10f;

    private int layerMask;

    void Start()
    {
        myTransform = transform;
        myBody = GetComponent<Rigidbody>();
        myRenderer = GetComponent<MeshRenderer>();
        toggleDisplay(false);
        // NPC or Player only
        layerMask = 1 << 10 | 11;
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
        Collider[] colliders = Physics.OverlapSphere(myTransform.position, radius, layerMask, QueryTriggerInteraction.Ignore);
        for (int i= 0; i < colliders.Length; i++) {
            var collider = colliders[i];
            Debug.LogFormat("Layermask {0} hit collder {1}", layerMask, collider);
            float dot = Vector3.Dot(myTransform.forward, collider.transform.position);
            // don't suppress myself!
            if (dot > -0.2f ) {
                var damageScript = collider.transform.root.GetComponent<IDamage>();
                if (damageScript != null) {
                    damageScript.HitNearby();
                }
            }
        }
    }
}
