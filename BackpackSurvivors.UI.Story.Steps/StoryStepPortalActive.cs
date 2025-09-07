using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Story.Steps;

public class StoryStepPortalActive : StoryStep
{
	[SerializeField]
	private Image _background;

	[SerializeField]
	private GameObject _portalEffect;

	[SerializeField]
	private AudioClip _speechAudio;

	internal override void BeforeStart()
	{
		base.BeforeStart();
		_portalEffect.SetActive(value: true);
		_background.color = new Color(255f, 255f, 255f, 0f);
		FadeAlpha(_background, 1f, StartDuration);
	}
}
