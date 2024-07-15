using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathRoot : MonoBehaviour
{
	[SerializeField] private Transform startPoint;
	[SerializeField] private PathPoint[] points;
	[SerializeField] private Transform lastPoint;

	[SerializeField] private GameObject flowerWayItem;

	public float flowerWayInterval = 0.5f;
	public float flowerSkipRadius = 0.5f;
	public float flowerWayWaterLevel = 0;

	private int pointIndex;
	private bool increasingBuildWay = true;

	private List<FlowerWayItem> flowerWay = new List<FlowerWayItem>();

	private bool pathDone = false;

	public event EventHandler<Vector3[]> WayIsBult;
	public event EventHandler<bool> UserAnswer;

	private bool _stopped = false;

	public void TurnOff()
	{
		_stopped = true;
	}

	public void TurnOn()
	{
		_stopped = false;
	}

	private PathPoint[] buildWayOrderPoints;

	// Start is called before the first frame update
	void Start()
	{
		flowerWay.Clear();

		foreach (var point in points)
		{
			point.SetPathRoot(this);
		}

		TurnIncreaseBuildWay();
	}

	public void ResetWay()
	{
		pathDone = false;

		foreach (var point in points)
		{
			point.SetShine(false);
		}

		foreach (var item in flowerWay)
		{
			item.Disappear();
		}

		flowerWay.Clear();
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void TurnIncreaseBuildWay()
	{
		increasingBuildWay = true;
		buildWayOrderPoints = points.OrderBy(x => x.pointNo).ToArray();
		pointIndex = 0;
	}

	public void TurnDecreaseBuildWay()
	{
		increasingBuildWay = false;
		buildWayOrderPoints = points.OrderByDescending(x => x.pointNo).ToArray();
		pointIndex = 0;
	}

	public void numberIsPressed(PathPoint p)// отаботать обратную игру;
											// там где сделал хитрообратный отсчет - отменить.
											// сделать натуральную инверсию последовательности точек
	{
		if(_stopped) return;
		if(pathDone) return;

		if(p.pointNo == buildWayOrderPoints[pointIndex].pointNo)
		{
			p.SetShine(true);
			UserAnswer?.Invoke(this, true);

			if (pointIndex >= 1)
			{
				addFlowerWaySection(buildWayOrderPoints[pointIndex], buildWayOrderPoints[pointIndex - 1],
					flowerWayInterval, flowerSkipRadius, flowerWayWaterLevel);
			}

			pointIndex++;

			if(pointIndex == buildWayOrderPoints.Length)
			{
				pathDone = true;

				var pnts = buildWayOrderPoints.Select(x => x.transform.position).ToList();
				if(increasingBuildWay) pnts.Add(lastPoint.position);
				else pnts.Add(startPoint.position);
				WayIsBult?.Invoke(this, pnts.ToArray());
			}
		}
		else
		{
			UserAnswer?.Invoke(this, false);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(startPoint.position, 0.1f);

		Gizmos.color = Color.magenta;
		for (int i = 0; i < points.Length - 1; i++) 
		{
			Gizmos.DrawLine(points[i].gameObject.transform.position, points[i + 1].gameObject.transform.position);
		}
	}

	private void addFlowerWaySection(PathPoint a, PathPoint b, float interval, float skipRadius, float waterLevel)
	{
		var points = GenerateFlowerPoints(a.transform.position, b.transform.position,
			interval, skipRadius, waterLevel);

		foreach (var point in points)
		{
			var wayItem = Instantiate(flowerWayItem, point, Quaternion.identity);
			flowerWay.Add(wayItem.GetComponent<FlowerWayItem>());
		}
	}

	private List<Vector3> GenerateFlowerPoints(Vector3 pointA, Vector3 pointB, float interval, float skipRadius, float waterLevel)
	{
		var generatedPoints = new List<Vector3>();
		if (pointA == null || pointB == null) return null;

		pointA.y = waterLevel;
		pointB.y = waterLevel;

		Vector3 centerPoint = (pointA + pointB) / 2.0f;
		Vector3 directionAtoB = (pointB - pointA).normalized;
		Vector3 directionBtoA = (pointA - pointB).normalized;

		float halfDistance = Vector3.Distance(pointA, centerPoint);
		int numberOfPoints = Mathf.FloorToInt(halfDistance / interval);

		for (int i = 0; i <= numberOfPoints; i++)
		{
			Vector3 pointTowardsA = centerPoint + directionBtoA * (i * interval);
			Vector3 pointTowardsB = centerPoint + directionAtoB * (i * interval);

			if (Vector3.Distance(pointTowardsA, pointA) > skipRadius && !generatedPoints.Contains(pointTowardsA))
			{
				generatedPoints.Add(pointTowardsA);
			}

			if (Vector3.Distance(pointTowardsB, pointB) > skipRadius && !generatedPoints.Contains(pointTowardsB))
			{
				generatedPoints.Add(pointTowardsB);
			}
		}

		// Ensure the endpoints are included if they are outside the skip radius
		//if (Vector3.Distance(pointA.position, centerPoint) > skipRadius && !generatedPoints.Contains(pointA.position))
		//{
		//	generatedPoints.Add(pointA.position);
		//}

		//if (Vector3.Distance(pointB.position, centerPoint) > skipRadius && !generatedPoints.Contains(pointB.position))
		//{
		//	generatedPoints.Add(pointB.position);
		//}

		return generatedPoints;
	}
}
