using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerWayItem : MonoBehaviour
{
	private AnimateFloat animateFloat;

	// Start is called before the first frame update
	void Start()
	{
		animateFloat = new AnimateFloat
		{
			duration = 1.0f,
			OnAnimationStep = (x) =>
			{
				transform.localScale = new Vector3(x, x, x);
			}
		};

		StartCoroutine(appear(true));
	}

	private IEnumerator appear(bool appearMe)
	{
		if (appearMe)
		{
			animateFloat.valueA = 0f;
			animateFloat.valueB = 1f;
			transform.localScale = new Vector3(0, 0, 0);
		}
		else
		{
			animateFloat.valueA = 1f;
			animateFloat.valueB = 0f;
			transform.localScale = new Vector3(1, 1, 1);
		}

		while(animateFloat.update(null))
			yield return null;

		if (!appearMe) Destroy(gameObject);
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void Disappear()
	{
		StartCoroutine(appear(false));
	}
}
