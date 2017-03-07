// Aiden Melendez

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]

abstract public class Vehicle : MonoBehaviour {

	protected Vector3 acceleration;
	protected Vector3 velocity;
	protected Vector3 desired;

	public float maxSpeed = 6.0f;
	public float maxForce = 12.0f;
	public float mass = 1.0f;
	public float radius = 1.0f;
	public float slowingRadius = 30.0f;

	CharacterController charControl;

	// access to game manager script
	protected GameManager gm;

	public Vector3 Velocity {
		get { return velocity; }
	}    

	virtual public void Start(){
		acceleration = Vector3.zero;
		velocity = transform.forward;
		desired = Vector3.zero;

		//store access to character controller component
		charControl = GetComponent<CharacterController>();

		// access to game obj holding gm script
		gm = GameObject.Find("GameManagerObj").GetComponent<GameManager>();
	}


	// Update is called once per frame
	protected void Update () {
		CalcSteeringForces ();

		velocity += acceleration * Time.deltaTime;
		velocity.y = 0;
		velocity = Vector3.ClampMagnitude (velocity, maxSpeed);
		transform.forward = velocity.normalized;
		charControl.Move (velocity * Time.deltaTime);
		acceleration = Vector3.zero;

	}

	abstract protected void CalcSteeringForces();

	protected void ApplyForce(Vector3 steeringForce){
		acceleration += steeringForce / mass;
	}

	// Steering Algorithms------------------------------------------------------------
	#region

	protected Vector3 Seek(Vector3 targetPosition){ 
		desired = targetPosition - transform.position;
		desired = desired.normalized * maxSpeed;
		Vector3 seekingForce = desired - velocity;
		seekingForce.y = 0; 
		return seekingForce; 
	}

	protected Vector3 Arrive(Vector3 targetPosition)
	{
		desired = targetPosition - transform.position;
		float distance = desired.magnitude;

		// check the distance to detect whether the character is inside the slowing area
		if (distance < slowingRadius)
		{
			// inside slowing area
			desired = desired.normalized * maxSpeed * (distance / slowingRadius);
		}
		else
		{
			// outside slowing area
			desired = desired.normalized * maxSpeed;
		}

		Vector3 arriveForce = desired - velocity;
		arriveForce.y = 0;
		return arriveForce;
	}

	protected Vector3 LeaderFollow(Vehicle leader)
	{
		// calculate negative velocity vector of leader
		Vector3 tv = leader.velocity * -1;
		tv.Normalize();

		// get behind vector
		Vector3 behind = leader.transform.position + tv;

		Vector3 followForce = Arrive(behind);
		return followForce;
	}

	///<summary>
	///Separation: Keeps flockers seperated by a certain distance from each other
	///</summary>
	public Vector3 Separation()
	{
		float desiredSeparation = 25.0f;
		Vector3 steer = Vector3.zero;
		int count = 0;

		foreach (GameObject gO in gm.Flock)
		{
			float dist = Vector3.Distance(transform.position, gO.transform.position);

			// if the distance is greater than 0 and less than an arbitrary about (0 is yourself)
			if ((dist > 0) &&(dist < desiredSeparation))
			{
				// calculate the vector pointing away from the neighbor
				Vector3 diff = transform.position - gO.transform.position;
				diff.Normalize();
				diff = diff / dist;
				steer += diff;
				count++;
			}
		}

		// get average
		if (count > 0)
		{
			steer /= (float)count;
		}

		if (steer.magnitude > 0) {
			steer.Normalize();
			steer *= maxSpeed;
			steer -= velocity;
			steer = Vector3.ClampMagnitude(steer, maxForce);
		}
		return steer;

	}

	public Vector3 Alignment(Vector3 alignVector)
	{
		Vector3 vtc = Vector3.zero;
		vtc = Vector3.Normalize (alignVector - transform.position);
		return vtc;
	}

	public Vector3 Cohesion(Vector3 cohesionVector){
		Vector3 vtc = Vector3.zero;
		vtc = Vector3.Normalize (cohesionVector - transform.position);
		return vtc;
	}

	public Vector3 StayInBounds(float radius, Vector3 center){return Vector3.zero; }


	#endregion
}
