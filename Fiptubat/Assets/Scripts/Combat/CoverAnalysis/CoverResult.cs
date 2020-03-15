using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class for examining cover.
/// </summary>
public class CoverResult
{
    // where to find it
    private Vector3 position;

    // used to check if this position points sufficiently away from the target
    private float normal;

    // how high is the cover here
    private float height;

    private int weight;

    public CoverResult() {
        // no-op - will set defaults automatically
    }

    public CoverResult(Vector3 position) {
        this.position = position;
    }

    public void SetPosition(Vector3 position) {
        this.position = position;
    }

    public Vector3 GetPosition() {
        return position;
    }

    public void SetNormal(float normal) {
        this.normal = normal;
    }

    public float GetNormal() {
        return normal;
    }

    public void SetHeight(float height) {
        this.height = height;
    }

    public void SetWeight(int weight) {
        this.weight = weight;
    }

    public int GetWeight() {
        return weight;
    }

    public override string ToString() {
        return string.Format("CoverResult[position: {0}, normal: {1}, weight: {2}]", position, normal, weight);
    }
    
}
