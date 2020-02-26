using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

/// <summary>
/// Find cover near a particular position.
/// Adapated from: https://forum.unity.com/threads/navmesh-agent-take-cover.403292/#post-3139813
/// </summary>
public class CoverFinder : MonoBehaviour
{

    public float initialRadius = 5f;

    public float coverAngleCriteria = 0f;

    public float maxAttempts = 3;

    private Transform myTransform;

    private NavMeshHit navMeshHit;

    private NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
	/// Find cover near my position that shields me in a particular direction
	/// </summary>
	/// <param name="position">My current position</param>
	/// <param name="direction">Direction from which I'm being shot or saw a target</param>
    public CoverResult FindCover(Vector3 currentPosition, Vector3 direction) {

        // obtain random positions around my current position.
        Dictionary<Vector3, CoverResult> samples = samplePositions(currentPosition, initialRadius);
		foreach (Vector3 point in samples.Keys.ToList()) {
            // find the closet edge to each one
			if(NavMesh.FindClosestEdge(point, out navMeshHit, navMeshAgent.areaMask)) {
				float normal = Vector3.Dot(navMeshHit.normal, (direction));
				Debug.DrawRay(currentPosition, navMeshHit.position - currentPosition, Color.green, 3f);
				Debug.DrawRay(navMeshHit.position, navMeshHit.normal, Color.yellow, 3f);
				samples[point].SetPosition(navMeshHit.position);
				samples[point].SetNormal(normal);
			}
		}
        // discard any that don't sufficiently point away from the target direction and choose the closest remaining
		var sortedByDot = samples.OrderBy(d => d.Value.GetNormal()).Where(d => d.Value.GetNormal() < coverAngleCriteria);
		CoverResult target = sortedByDot.Aggregate((x,y) => Vector3.Distance(x.Value.GetPosition(), currentPosition) 
                < Vector3.Distance(y.Value.GetPosition(), currentPosition) ? x : y).Value;

        Debug.LogFormat("{0} found cover at {1}", this, target);

        return target;
    }

    private Dictionary<Vector3, CoverResult> samplePositions(Vector3 startPosition, float radius) {
		Dictionary<Vector3, CoverResult> samples = new Dictionary<Vector3, CoverResult>();
		samples.Add(startPosition + myTransform.forward * radius, new CoverResult());
		samples.Add(startPosition - myTransform.forward * radius, new CoverResult());
		samples.Add(startPosition + myTransform.right * radius, new CoverResult());
		samples.Add(startPosition - myTransform.right * radius, new CoverResult());
		return samples;
	}
}
