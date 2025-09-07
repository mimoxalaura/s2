using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Story.Steps;

public class StoryStepCallToAction : StoryStep
{
	[SerializeField]
	private Image _face1;

	[SerializeField]
	private Image _face2;

	[SerializeField]
	private TextMeshProUGUI _text;

	[SerializeField]
	private AudioClip _speechAudio;

	public override void Run()
	{
		StartCoroutine(DoRun());
	}

	private IEnumerator DoRun()
	{
		yield return new WaitForSecondsRealtime(1f);
		_face1.gameObject.SetActive(value: true);
		yield return new WaitForSecondsRealtime(1.5f);
		_face1.gameObject.SetActive(value: false);
		_face2.gameObject.SetActive(value: true);
		yield return new WaitForSecondsRealtime(2f);
	}

	internal override void BeforeStart()
	{
		base.BeforeStart();
		_text.gameObject.SetActive(value: false);
	}
}
