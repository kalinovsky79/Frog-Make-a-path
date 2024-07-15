using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor;
using UnityEngine;

public class CountUpTimer : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI timeText;

	public event EventHandler<int> Expired;

	private bool stopped = false;
	private bool paused = false;

	public void Go()
	{
		stopped = false;
		paused = false;
		StartCoroutine(timerCoroutine());
	}

	public void StopTimer()
	{
		stopped = true;
	}

	private IEnumerator timerCoroutine()
	{
		int gameTime = 0;

		while (!stopped)
		{
			timeText.text = gameTime.ToString("0");
			yield return new WaitForSeconds(1f);
			if (!paused) gameTime++;
		}

		timeText.text = gameTime.ToString();

		Expired?.Invoke(this, gameTime);
	}
}
