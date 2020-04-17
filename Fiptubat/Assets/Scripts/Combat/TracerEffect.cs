using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gives a tracer effect to show where a "bullet" is going.
/// Also applies the suppression effect.
/// </summary>
public class TracerEffect : MonoBehaviour
{
    private Transform myTransform;

    private Rigidbody myBody;

    private MeshRenderer myRenderer;

    public float velocity = 2f;

    public float suppressionRadius = 5f;

    [Tooltip("Vector dot product that defines angle of suppression")]
    public float suppressionAngle = -0.2f;

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
        Collider[] colliders = Physics.OverlapSphere(myTransform.position, suppressionRadius, layerMask, QueryTriggerInteraction.Ignore);
        List<IDamage> actualTargets = new List<IDamage>();
        for (int i= 0; i < colliders.Length; i++) {
            var collider = colliders[i];
            var damageScript = collider.transform.root.GetComponent<IDamage>();
            if (damageScript != null && !actualTargets.Contains(damageScript)) {
                actualTargets.Add(damageScript);
            }
        }

        for (int i = 0; i < actualTargets.Count; i++) {
            float dot = Vector3.Dot(myTransform.forward, actualTargets[i].GetTransform().position);
            // don't suppress myself!
            if (dot > suppressionAngle ) {
                actualTargets[i].HitNearby();
            }
        }
    }
}
