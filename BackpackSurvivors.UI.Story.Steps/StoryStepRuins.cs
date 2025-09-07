using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Story.Steps;

public class StoryStepRuins : StoryStep
{
	[SerializeField]
	private Image _background;

	[SerializeField]
	private Transform _targetPosition;

	[SerializeField]
	private AudioClip _speechAudio;

	internal override void BeforeStart()
	{
		base.BeforeStart();
		_background.color = new Color(255f, 255f, 255f, 0f);
		FadeAlpha(_background, 1f, StartDuration);
		MoveToPositionOverTime(_targetPosition, _background.gameObject);
		ZoomInOverTime(_background.gameObject, 1.1f);
	}

	internal override void BeforeFinish()
	{
		base.BeforeFinish();
		FadeAlpha(_background, 0f, FinishDuration);
	}

	internal override void AfterFinish()
	{
		base.AfterFinish();
		FadeAlpha(_background, 0f, FinishDuration, 4f);
	}
}
