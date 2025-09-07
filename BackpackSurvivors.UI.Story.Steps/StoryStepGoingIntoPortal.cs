using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Story.Steps;

public class StoryStepGoingIntoPortal : StoryStep
{
	[SerializeField]
	private Image _face2;

	[SerializeField]
	private Image _portal;

	[SerializeField]
	private GameObject _portalEffect;

	[SerializeField]
	private AudioClip _speechAudio;

	internal override void BeforeStart()
	{
		base.BeforeStart();
		FadeAlpha(_face2, 0f, 0.5f);
		ZoomInOverTime(_portal.gameObject, 5f);
		ZoomInOverTime(_portalEffect, 5f);
	}
}
