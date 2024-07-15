using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * управление объектами игры
 */
public class GameManager : MonoBehaviour
{
	// некоторые части когда пойдут в game manager
	[SerializeField] private GameObject okEffect;
	[SerializeField] private GameObject wrongEffect;
	[SerializeField] private Transform motivePoint;

	// набор путей для одной сцены; один путь берется на одну сессию
	[SerializeField] private PathRoot[] paths;
	[SerializeField] private FrogThinker[] frogs;

	public event EventHandler GameWin;

	private RandomList<PathRoot> randomListPath;
	private RandomList<FrogThinker> randomListFrog;

	private FrogThinker currentFrog;
	private PathRoot currentPath;

	private int passCount = 0;

	// Start is called before the first frame update
	void Start()
	{
		randomListPath = new RandomList<PathRoot>(paths);
		randomListFrog = new RandomList<FrogThinker>(frogs);

		currentPath = randomListPath.Next();
		currentFrog = randomListFrog.Next();

		currentPath.UserAnswer += CurrentPath_UserAnswer;
		currentPath.WayIsBult += CurrentPath_WayIsBult;
		currentFrog.WayCompleted += CurrentFrog_WayCompleted;

		currentPath.TurnOff();
		currentPath.TurnIncreaseBuildWay();
	}

	public void RunGame()
	{
		// так же можно вызвать
		//currentPath = randomListPath.Next();
		//currentFrog = randomListFrog.Next();
		// и так же создать из префаба, в установленной точке создания
		//currentPath.ResetWay();
		currentPath.TurnIncreaseBuildWay();
		currentPath.TurnOn();
	}

	public void StopGame()
	{
		currentPath.TurnOff();
	}

	private void CurrentPath_UserAnswer(object sender, bool e)
	{
		if (e)
		{
			if(okEffect != null)
				Instantiate(okEffect, motivePoint.position, Quaternion.identity);
		}
		else
		{
			if (wrongEffect != null)
				Instantiate(wrongEffect, motivePoint.position, Quaternion.identity);
		}
	}

	private void CurrentPath_WayIsBult(object sender, Vector3[] e)
	{
		currentFrog.SetWay(e);
		currentFrog.Go();
	}

	private void CurrentFrog_WayCompleted(object sender, System.EventArgs e)
	{
		passCount++;
		if(passCount < 2)
		{
			currentPath.ResetWay();
			currentPath.TurnDecreaseBuildWay();
		}
		else
		{
			currentPath.ResetWay();
			currentPath.TurnOff();
			GameWin?.Invoke(this, EventArgs.Empty);
		}
	}
}
