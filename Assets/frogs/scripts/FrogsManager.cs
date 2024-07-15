using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FrogsManager : MonoBehaviour
{
	[SerializeField] int totalFrogs = 20;

	[SerializeField] TextMeshProUGUI scores;

	private int caughtFrogs = 0;
	private int passedFrogs = 0;// не может стать больше totalFrogs

	private showEndGame _showEndGame;
	private delayBeforePlay _delayBeforePlay;

	// Start is called before the first frame update
	void Start()
	{
		_showEndGame = FindAnyObjectByType<showEndGame>();
		_delayBeforePlay = FindAnyObjectByType<delayBeforePlay>();

		updateScoreText();
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void AddCaughtScore()
	{
		caughtFrogs++;
		passedFrogs++;
		updateScoreText();
		if (passedFrogs == totalFrogs) StartCoroutine(endGame());// так же прекращаем спавн лягушек
	}

	public void leaveCity()
	{
		passedFrogs++;

		if (passedFrogs == totalFrogs) StartCoroutine(endGame());// так же прекращаем спавн лягушек
	}

	private IEnumerator endGame()
	{
		yield return new WaitForSeconds(2.0f);

		_showEndGame.endGame(caughtFrogs, totalFrogs);
	}

	private void updateScoreText()
	{
		scores.text = $"{caughtFrogs}/{totalFrogs} SCORE";
	}

	public bool letsGo()
	{
		return _delayBeforePlay.youCanGo();
	}
}
