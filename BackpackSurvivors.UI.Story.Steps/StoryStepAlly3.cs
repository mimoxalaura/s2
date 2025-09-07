using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Story.Steps;

public class StoryStepAlly3 : StoryStep
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
	}

	internal override void AfterStart()
	{
		base.AfterStart();
	}
}
