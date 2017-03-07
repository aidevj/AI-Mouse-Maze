﻿// Aiden Melendez

using UnityEngine;
using System.Collections;

/// <summary>
/// In which a vehicle arrives at a point behind a leader.
/// Given to Flocker obejcts
/// </summary>
public class LeaderFollower : Vehicle {

	public GameObject leader;

	//ultimate steering force that will be applied to acceleration
	private Vector3 force;

	// Seeking Weights
	public float seekWeight = 75.0f;
	public float safeDistance = 10.0f;
	public float avoidWeight = 100.0f;
	public float sepWeight = 10.0f;
	public float alignWeight = 40.0f;
	public float coWeight = 40.0f;

	// Call Inherited Start and then do our own
	override public void Start()
	{
		base.Start();
		force = Vector3.zero;
	}

	protected override void CalcSteeringForces()
	{
		//reset ultimate force
		force = Vector3.zero;

		//get a seeking force (based on char movement - for now, just seek)
		//add that seeking force to the ultimate steering force
		force += LeaderFollow(leader.GetComponent<Vehicle>()) * seekWeight; //////// leader does not have vehicle component, or child of

		force += Separation() * sepWeight;
		force += Alignment(gm.FlockDirection) * alignWeight;
		force += Cohesion(gm.Centroid) * coWeight;

		//limit steering force
		force = Vector3.ClampMagnitude(force, maxForce);

		//applyForce to acceleration
		ApplyForce(force);
	}

}

