using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracerPool : MonoBehaviour
{

    private List<TracerEffect> tracers;

    private int tracerIndex;

    // Start is called before the first frame update
    void Start()
    {
        tracers = new List<TracerEffect>(GetComponentsInChildren<TracerEffect>());
    }

    public TracerEffect GetTracer() {
        TracerEffect tracer = tracers[tracerIndex];
        tracerIndex++;
        if (tracerIndex >= tracers.Count) {
            tracerIndex = 0;
        }
        return tracer;
    }

    
}
