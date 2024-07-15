using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TouchScript.Gestures;
using UnityEngine;
using UnityEngine.UI;

public class GameScreenManager : MonoBehaviour
{
	[SerializeField] private Image startPageBackground;
	[SerializeField] private Image finalPageBackground;

	[SerializeField] private GameObject startPageElementsGroup;
	[SerializeField] private GameObject gamePageElementsGroup;
	[SerializeField] private GameObject finalPageElementsGroup;

	[SerializeField] private TextMeshProUGUI CountdownRestartText;
	[SerializeField] private TapGesture StartingButton;
	[SerializeField] private TapGesture RestartingButton;

	private ChainRunner toFinalPageChainRunner;
	private ChainRunner toGamePageChainRunner;
	private ChainRunner fromFinalToStartPageChainRunner;

	public event EventHandler FinalPageEntered;
	public event EventHandler FinalPageLeave;
	public event EventHandler GamePageEntering;
	public event EventHandler GamePageEntered;
	public event EventHandler StartPageEntered;

	public event EventHandler UpdateFinalTextRequired;

	// Start is called before the first frame update
	void Start()
	{
		RestartingButton.Tapped += RestartingButton_Tapped;
		StartingButton.Tapped += StartingButton_Tapped;

		toFinalPageChainRunner = new ChainRunner();

		toFinalPageChainRunner
		.AddStep(new OperationStep
		{
			Do = () =>
			{
				FinalPageEntered?.Invoke(this, EventArgs.Empty);

				gamePageElementsGroup.SetActive(false);
				finalPageBackground.gameObject.SetActive(true);
			}
		})
		.AddStep(new AnimateFloat
		{
			duration = 0.8f,
			valueA = 0.0f,
			valueB = 0.95f,
			OnAnimationStep = (x) =>
			{
				Color newColor = finalPageBackground.color;
				newColor.a = x;
				finalPageBackground.color = newColor;
			}
		})
		.AddStep(new OperationStep
		{
			Do = () =>
			{
				UpdateFinalTextRequired?.Invoke(this, EventArgs.Empty);
				finalPageElementsGroup.SetActive(true);
			}
		})
		.AddStep(new WaitForContitionStep
		{
			Init = () =>
			{
				CountdownRestartText.gameObject.SetActive(true);
				StartCoroutine(countDownToRestart());
			},
			While = () => waitForRestart
		})
		.AddStep(new OperationStep
		{
			Do = () =>
			{
				CountdownRestartText.gameObject.SetActive(false);
				RestartingButton.gameObject.SetActive(true);
			}
		});

		toGamePageChainRunner = new ChainRunner();
		toGamePageChainRunner
			.AddStep(new OperationStep
			{
				Do = () =>
				{
					GamePageEntering?.Invoke(this, EventArgs.Empty);
					startPageElementsGroup.SetActive(false);
				}
			})
			.AddStep(new AnimateFloat
			{
				duration = 1.0f,
				valueA = 1.0f,
				valueB = 0.0f,
				OnAnimationStep = (x) =>
				{
					Color newColor = startPageBackground.color;
					newColor.a = x;
					startPageBackground.color = newColor;
				}
			})
			.AddStep(new OperationStep
			{
				Do = () =>
				{
					gamePageElementsGroup.SetActive(true);
					startPageBackground.gameObject.SetActive(false);
					GamePageEntered?.Invoke(this, EventArgs.Empty);
				}
			});

		fromFinalToStartPageChainRunner = new ChainRunner();
		fromFinalToStartPageChainRunner
			.AddStep(new OperationStep
			{
				Do = () =>
				{
					FinalPageLeave?.Invoke(this, EventArgs.Empty);
					finalPageElementsGroup.SetActive(false);
					startPageBackground.gameObject.SetActive(true);
					RestartingButton.gameObject.SetActive(false);
				}
			})
			.AddStep(new AnimateFloat
			{
				duration = 1.0f,
				valueA = 0.0f,
				valueB = 1.0f,
				OnAnimationStep = (x) =>
				{
					Color newColor = startPageBackground.color;
					newColor.a = x;
					startPageBackground.color = newColor;
				}
			})
			.AddStep(new OperationStep
			{
				Do = () =>
				{
					startPageElementsGroup.SetActive(true);
					finalPageBackground.gameObject.SetActive(false);
					StartPageEntered?.Invoke(this, EventArgs.Empty);
				}
			});
	}

	private void StartingButton_Tapped(object sender, EventArgs e)
	{
		ToGamePage();
	}

	private void RestartingButton_Tapped(object sender, EventArgs e)
	{
		FromFinalToStartPage();
	}

	private IEnumerator transitionToFinalPage()
	{
		toFinalPageChainRunner.StartChain();
		while (toFinalPageChainRunner.Update())
		{
			yield return null;
		}
	}

	private IEnumerator transitionToGamePage()
	{
		toGamePageChainRunner.RestartChain();
		while (toGamePageChainRunner.Update())
		{
			yield return null;
		}
	}

	private IEnumerator transitionFromFinalToStartPage()
	{
		fromFinalToStartPageChainRunner.RestartChain();
		while (fromFinalToStartPageChainRunner.Update())
		{
			yield return null;
		}
	}

	public void ToFinalPage()
	{
		StartCoroutine(transitionToFinalPage());
	}

	private void ToGamePage()
	{
		StartCoroutine(transitionToGamePage());
	}

	private void FromFinalToStartPage()
	{
		StartCoroutine(transitionFromFinalToStartPage());
	}

	public void SetStartPage()
	{
		finalPageBackground.gameObject.SetActive(false);
		finalPageElementsGroup.SetActive(false);
		gamePageElementsGroup.SetActive(false);

		startPageBackground.gameObject.SetActive(true);
		startPageElementsGroup.SetActive(true);
		StartPageEntered?.Invoke(this, EventArgs.Empty);
	}

	private bool waitForRestart = false;

	private IEnumerator countDownToRestart()
	{
		waitForRestart = true;

		int remainingTime = 9;

		while (remainingTime > 0)
		{
			CountdownRestartText.text = $"RESTART IN {remainingTime}...";
			yield return new WaitForSeconds(1f);
			remainingTime--;
		}

		waitForRestart = false;
	}
}
