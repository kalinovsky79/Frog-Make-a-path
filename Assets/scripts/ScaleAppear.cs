using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAppear : MonoBehaviour
{
	private AnimateFloat animateFloat;

	// Start is called before the first frame update
	void Start()
	{
		animateFloat = new AnimateFloat
		{
			duration = 0.5f,
			OnAnimationStep = (x) =>
			{
				transform.localScale = new Vector3(x, x, x);
			}
		};

		transform.localScale = new Vector3(0, 0, 0);
	}

	public void Appear(bool a)
	{
		if (a) StartCoroutine(appear());
		else StartCoroutine(disappear());
	}

	private IEnumerator appear()
	{
		animateFloat.valueA = 0;
		animateFloat.valueB = 1.0f;

		while (animateFloat.update(null))
		{
			yield return null;
		}
	}

	private IEnumerator disappear()
	{
		animateFloat.valueA = 1.0f;
		animateFloat.valueB = 0.0f;

		while (animateFloat.update(null))
		{
			yield return null;
		}
	}
}
