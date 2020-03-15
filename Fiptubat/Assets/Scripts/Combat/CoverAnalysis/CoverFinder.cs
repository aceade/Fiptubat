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

    [Tooltip("If we can't find cover, return this position")]
    public Vector3 nullPosition = Vector3.down * 200f;

    [Tooltip("Defines angles at which to find cover. > 0 means likely to be flanked (stupid); less than that will pick good cover")]
    public float coverAngleCriteria = 0f;

    public int maxAttempts = 3;
    private int currentAttempts = 0;

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
        currentAttempts = 0;
        float radius = initialRadius;

        Dictionary<Vector3, CoverResult> samples = new Dictionary<Vector3, CoverResult>();

        while (currentAttempts < maxAttempts) {
            // Obtain random positions around my current position.
            samples = samplePositions(currentPosition, radius, direction);
            if (samples.Count == 0 ) {
                currentAttempts++;
                radius *= 2;
            } else  {
                break;
            }
        }

        if (samples.Count == 0) {
            Debug.LogWarningFormat("{0} can't find cover!", this);
            return new CoverResult(nullPosition);
        }

        // Then discard any that don't sufficiently point away from the target direction and choose the closest remaining
        var sortedByDot = samples.OrderBy(d => d.Value.GetNormal()).Where(d => d.Value.GetNormal() > coverAngleCriteria);
        try {
		    CoverResult target = sortedByDot.Aggregate((x,y) => Vector3.Distance(x.Value.GetPosition(), currentPosition) 
                < Vector3.Distance(y.Value.GetPosition(), currentPosition) ? x : y).Value; 
            Debug.LogFormat("{0} found cover at {1}", this, target);
            return target;
        } catch (System.InvalidOperationException e) {
            Debug.LogWarningFormat("{0} can't find ANY cover! {1}", this, e.Message);
            return new CoverResult(nullPosition);
        }

    }

    private Dictionary<Vector3, CoverResult> samplePositions(Vector3 startPosition, float radius, Vector3 targetDirection) {
		Dictionary<Vector3, CoverResult> samples = new Dictionary<Vector3, CoverResult>();
		samples.Add(startPosition + myTransform.forward * radius, new CoverResult());
		samples.Add(startPosition - myTransform.forward * radius, new CoverResult());
		samples.Add(startPosition + myTransform.right * radius, new CoverResult());
		samples.Add(startPosition - myTransform.right * radius, new CoverResult());

        foreach (Vector3 point in samples.Keys.ToList()) {
            // find the closet edge to each one
			if(NavMesh.FindClosestEdge(point, out navMeshHit, navMeshAgent.areaMask)) {
				float normal = Vector3.Dot(navMeshHit.normal, (targetDirection));
				Debug.DrawRay(navMeshHit.position, navMeshHit.normal, Color.yellow, 3f);
				samples[point].SetPosition(navMeshHit.position);
				samples[point].SetNormal(normal);
			}
		}

		return samples;
	}

}
