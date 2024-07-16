using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEngine;


public class PathPoint : MonoBehaviour
{
	[SerializeField] private Color qizmosColor = Color.yellow;
	[SerializeField] private Renderer[] renderersToShine;

	// так как свечение зашито в шейдерах, я пока зне знаю как передать из инспектора переполненные цвета
	[SerializeField] private Color fadedColor;
	[SerializeField] private Color shineColor;

	//Color(1.58884215,0.868569791,0.332757533,1)

	public int pointNo = 0;

	private ScaleAppear numberHead;

	private PathRoot _pathRoot;
	private PressGesture pressGesture;

	private AnimateColor animateShineColor;
	//private AnimateFloat animateShineColor;

	private bool isShining = false;

	private Color cShineEmission = new Color(1.58884215f, 0.868569791f, 0.332757533f, 1);
	private Color cFadedEmission = new Color(1.58884215f, 0.868569791f, 0.332757533f, 1);

	public void SetPathRoot(PathRoot pr)
	{
		_pathRoot = pr;
	}

	public void SetShine(bool shine)
	{
		if (goingToShine) return;

		StartCoroutine(letsShine(shine));
	}

	// Start is called before the first frame update
	void Start()
	{
		pressGesture = GetComponent<PressGesture>();
		if(pressGesture != null)
			pressGesture.Pressed += PressGesture_Pressed;

		animateShineColor = new AnimateColor
		{
			duration = 0.7f,
			valueA = fadedColor,
			//valueB = cShineEmission,
			valueB = shineColor,
			OnAnimationStep = (c) =>
			{
				changeColor(renderersToShine, c);
			}
		};

		StartCoroutine(init());
	}

	private bool goingToShine = false;

	private IEnumerator init()
	{
		yield return new WaitForEndOfFrame();
		numberHead = GetComponentInChildren<ScaleAppear>();
	}

	public void AppearNumber(bool a)
	{
		numberHead.Appear(a);
	}

	private IEnumerator letsShine(bool shine)
	{
		goingToShine = true;

		if (shine)
		{
			animateShineColor.valueA = fadedColor;
			//animateShineColor.valueB = cShineEmission;
			animateShineColor.valueB = shineColor;
		}
		else
		{
			animateShineColor.valueB = fadedColor;
			//animateShineColor.valueA = cShineEmission;
			animateShineColor.valueA = shineColor;
		}

		while (animateShineColor.update(null))
		{
			yield return null;
		}

		goingToShine = false;
		isShining = shine;

	}

	private void changeColor(Renderer[] renderers, Color newColor)
	{
		foreach (var r in renderers)
		{
			r.material.color = newColor;

			//r.material.SetColor("_EmissionColor", newColor * Mathf.LinearToGammaSpace(1.0f));
		}
	}

	private void changeEmission(Renderer[] renderers, float newEmission)
	{
		foreach (var r in renderers)
		{

		// Update the emission color based on the intensity
			Color baseEmissionColor = r.material.GetColor("_EmissionColor");
			Color finalColor = baseEmissionColor * Mathf.LinearToGammaSpace(newEmission);

			// Apply the emission color to the material
			r.material.SetColor("_EmissionColor", finalColor);
		}
	}

	private void PressGesture_Pressed(object sender, System.EventArgs e)
	{
		if (isShining) return;
		if(goingToShine) return;
		_pathRoot.numberIsPressed(this);
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = qizmosColor;
		Gizmos.DrawSphere(transform.position, 0.1f);
	}
}
