//using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class FrogThinker : MonoBehaviour
{
	public Animator animator;

	private SectionWayOptRunner sectionWayRunner;

	public event EventHandler WayCompleted;

	// Start is called before the first frame update
	void Start()
	{
		sectionWayRunner = new SectionWayOptRunner(transform,
			new AnimateRotationStep
			{
				movableObject = transform,
				rotationSpeed = 12.0f,
			},
			new StayStep
			{
				stayForSeconds = 0.01f,
				OnEnter = (x) => { animator.SetTrigger("idle"); }
			},
			new JumpParabolaStep
			{
				movableObject = transform,
				jumpDuration = 0.7f,
				jumpHeight = 1.0f,
				OnStartMotion = (x) =>
				{
					animator.SetTrigger("jump");
				}
			}
			);

		sectionWayRunner.Completed = () =>
		{
			WayCompleted?.Invoke(this, EventArgs.Empty);
		};
	}

	public void SetWay(Vector3[] points)
	{
		sectionWayRunner.SetWay(points);
	}

	public void Go()
	{
		isGoing = true;
	}

	bool isGoing = false;
	// Update is called once per frame
	void Update()
	{
		//if(isFinished) return;
		if(!isGoing) return;

		if (isGoing)
		{
			if (!sectionWayRunner.Update())
			{
				isGoing = false;
			}
		}
	}

	Vector3 CalculatePointForJumpToObject(Vector3 A, Vector3 B, float distance)
	{
		// Direction vector from A to B
		Vector3 direction = B - A;

		// Normalize the direction vector to get the unit vector
		Vector3 unitDirection = direction.normalized;

		// Calculate point C
		Vector3 pointC = B - unitDirection * distance;

		return pointC;
	}

	private Vector3 CalculateEndPointWhenLeaving(float targetYLevel, float jdist)
	{
		// Calculate the end point based on the frog's forward direction

		var res = transform.position + transform.forward * jdist;

		res.y = targetYLevel;

		return res;
	}
}