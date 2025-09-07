using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Story.Steps;

public class StoryStepPortal : StoryStep
{
	[SerializeField]
	private Image _background;

	[SerializeField]
	private Transform _targetPosition;

	[SerializeField]
	private AudioClip _speechAudio;

	[SerializeField]
	private Image _ally1;

	[SerializeField]
	private Image _ally2;

	[SerializeField]
	private Image _ally3;

	[SerializeField]
	private TextMeshProUGUI _text;

	internal override void BeforeStart()
	{
		base.BeforeStart();
		FadeAlpha(_ally1, 0f, 1f);
		FadeAlpha(_ally2, 0f, 1f);
		FadeAlpha(_ally3, 0f, 1f);
		_background.color = new Color(255f, 255f, 255f, 0f);
		FadeAlpha(_background, 1f, StartDuration);
		MoveToPositionOverTime(_targetPosition, _background.gameObject);
	}

	internal override void AfterStart()
	{
		base.AfterStart();
		_ally1.gameObject.SetActive(value: false);
		_ally2.gameObject.SetActive(value: false);
		_ally3.gameObject.SetActive(value: false);
		_text.gameObject.SetActive(value: false);
	}

	internal override void BeforeFinish()
	{
		base.BeforeFinish();
		FadeAlpha(_background, 0f, FinishDuration);
	}
}
