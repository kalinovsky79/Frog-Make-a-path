using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * управление основными объектами и состоянием игры
 * - менеджер музыки
 * - наблюдение менеджера таймера - разрешение работы game manager
 * - game manager оповещает что условия игры выполнены, определяет результат
 * - вычисление что условия игры выполнены - сигнал менеджеру страниц
 * 
 * 
 */
public class GameStateManager : MonoBehaviour
{
	[SerializeField] private GameScreenManager gameScreenManager;
	[SerializeField] private GameManager gameManager;
	[SerializeField] private GameMusicManager gameMusicManager;
	[SerializeField] private CountdownTimer cdTimer;
	[SerializeField] private CountUpTimer upTimer;

	public float gameDelay = 5;

	// Start is called before the first frame update
	void Start()
	{
		gameScreenManager.GamePageEntering += GameScreenManager_GamePageEntering;
		gameScreenManager.GamePageEntered += GameScreenManager_GamePageEntered;
		gameScreenManager.FinalPageEntered += GameScreenManager_FinalPageEntered;
		gameScreenManager.FinalPageLeave += GameScreenManager_FinalPageLeave;
		gameScreenManager.StartPageEntered += GameScreenManager_StartPageEntered;
		gameScreenManager.UpdateFinalTextRequired += GameScreenManager_UpdateFinalTextRequired;

		cdTimer.Expired += CdTimer_Expired;
		upTimer.Expired += UpTimer_Expired;

		gameManager.GameWin += GameManager_GameWin;

		//gameManager.StopGame();
		gameScreenManager.SetStartPage();
	}

	private void GameManager_GameWin(object sender, System.EventArgs e)
	{
		gameScreenManager.ToFinalPage();
	}

	private void UpTimer_Expired(object sender, int e)
	{
		
	}

	private void CdTimer_Expired(object sender, System.EventArgs e)
	{
		gameManager.RunGame();
		upTimer.Go();
	}

	private void GameScreenManager_UpdateFinalTextRequired(object sender, System.EventArgs e)
	{
		
	}

	private void GameScreenManager_StartPageEntered(object sender, System.EventArgs e)
	{
		
	}

	private void GameScreenManager_GamePageEntered(object sender, System.EventArgs e)
	{
		cdTimer.delay = gameDelay;
		cdTimer.showGoOnEnd = true;
		cdTimer.Go();
	}

	private void GameScreenManager_FinalPageLeave(object sender, System.EventArgs e)
	{
		
	}

	private void GameScreenManager_FinalPageEntered(object sender, System.EventArgs e)
	{
		
	}

	private void GameScreenManager_GamePageEntering(object sender, System.EventArgs e)
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
