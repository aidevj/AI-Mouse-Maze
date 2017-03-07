// Aiden Melendez

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///  Must be a component of the GameManagerObj.
/// </summary>
public class GameManager : MonoBehaviour {

	// Game objects
	[HideInInspector]public GameObject flocker;
	[HideInInspector]public GameObject player;
	[HideInInspector]public GameObject target;

	// Prefabs
	public GameObject flockerPrefab;
	//public GameObject playerPrefab;

	// Flocking Variables--------------------------------------------

	public int numberFlockers;	// available to adjust in Inspector

	private Vector3 centroid;
	private Vector3 flockDirection;
	private List<GameObject> flock;

	// outer edge boundaries
	private const float X_RANGE_MAX = 9.31f;
	private const float X_RANGE_MIN = -8.43f;
	private const float Z_RANGE_MAX = 11.44f;
	private const float Z_RANGE_MIN = -11.89f;

	// Getter Methods
	public Vector3 Centroid { get { return centroid; } }

	public Vector3 FlockDirection { get { return flockDirection; } }

	public List<GameObject> Flock { get { return flock; } }

	// Use this for initialization
	void Start () {
		// Get player obj in scene
		player = GameObject.Find("Mouse_Player");

		// Create a list of flockers
		flock = new List<GameObject>();
		Vector3 pos_f = new Vector3(-6.4f, 7.83f, -9.28f);	// initial flocker position
		flocker = (GameObject)Instantiate(flockerPrefab, pos_f, Quaternion.identity);

		for (int i = 0; i < numberFlockers; i++)
		{
			Vector3 pos2 = new Vector3(Random.Range(X_RANGE_MIN, X_RANGE_MAX), 8.0f, Random.Range(Z_RANGE_MIN, Z_RANGE_MAX));
			flocker = (GameObject)Instantiate(flocker, pos2, Quaternion.identity);
			flock.Add(flocker);
		}

		// Flock -> target
		// NOTE: Can change this depending on what behaviors we will give the flockers in the future
		target = player;

		// Assign a target to the seeker component of each flock member
		for (int i = 0; i < numberFlockers; i++)
		{
			flock[i].GetComponent<LeaderFollower>().leader = target;
		}

	}

	// Update is called once per frame
	void Update () {
		CalcCentroid();
	}

	// CalcCentroid: Calculate centroid about which flockers will flock
	public void CalcCentroid()
	{
		for (int i = 0; i < numberFlockers; i++)
		{
			centroid += flock[i].transform.position;
			centroid /= flock.Count;
		}
	}

	/// CalcFlockDirection: Add flocking forces
	public void CalcFlockDirection()
	{
		for (int i = 0; i < flock.Count; i++)
		{
			flockDirection += flock[i].transform.forward;
			flockDirection = flockDirection / flock.Count;
			flockDirection = flockDirection.normalized;
		}
	}
}
