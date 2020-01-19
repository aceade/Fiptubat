using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Marks where the player is looking.
/// </summary>
public class PlayerMoveMarker : MonoBehaviour {

    private Transform myTransform;

    private Renderer myRenderer;

    public Material defaultMaterial, blockedMaterial;

    void Start() {
        myTransform = transform;
        myRenderer = GetComponent<MeshRenderer>();
    }

    public void SetPosition(Vector3 position, bool isClear) {
        myTransform.position = position + Vector3.up;
        myRenderer.enabled = true;
        if (!isClear) {
            myRenderer.material = blockedMaterial;
        } else {
            myRenderer.material = defaultMaterial;
        }
    }

    public void Hide() {
        myRenderer.enabled = false;
    }
}