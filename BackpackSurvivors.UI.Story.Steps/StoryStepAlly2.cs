using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Story.Steps;

public class StoryStepAlly2 : StoryStep
{
	[SerializeField]
	private Image _background;

	[SerializeField]
	private AudioClip _speechAudio;

	internal override void BeforeStart()
	{
		base.BeforeStart();
		_background.color = new Color(255f, 255f, 255f, 0f);
		FadeAlpha(_background, 1f, StartDuration);
	}
}
