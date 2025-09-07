using System;
using System.Collections;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.System;
using Tymski;
using UnityEngine;

namespace BackpackSurvivors.UI.Story;

public class StoryController : MonoBehaviour
{
	[SerializeField]
	private StoryStep[] _storySteps;

	[SerializeField]
	private AudioClip _audio;

	[SerializeField]
	private SceneReference _afterStoryScene;

	private StoryStep _currentStep;

	private void Start()
	{
		SingletonController<AudioController>.Instance.PlayMusicClip(_audio, 1f, loop: false);
		SingletonController<InputController>.Instance.OnCancelHandler += InputController_OnCancelHandler;
		StartCoroutine(RunStory());
	}

	private void InputController_OnCancelHandler(object sender, EventArgs e)
	{
		StopAllCoroutines();
		SingletonController<SceneChangeController>.Instance.ChangeScene(_afterStoryScene.ScenePath);
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
		SingletonController<InputController>.Instance.OnCancelHandler -= InputController_OnCancelHandler;
	}

	private IEnumerator RunStory()
	{
		StoryStep[] storySteps = _storySteps;
		foreach (StoryStep storyStep in storySteps)
		{
			if (_currentStep != null)
			{
				_currentStep.BeforeFinish();
				yield return new WaitForSecondsRealtime(_currentStep.FinishDuration);
				_currentStep.AfterFinish();
			}
			_currentStep = storyStep;
			storyStep.gameObject.SetActive(value: true);
			_currentStep.BeforeStart();
			yield return new WaitForSecondsRealtime(storyStep.StartDuration);
			_currentStep.AfterStart();
			storyStep.Run();
			yield return new WaitForSecondsRealtime(storyStep.RunDuration);
		}
		SingletonController<SceneChangeController>.Instance.ChangeScene(_afterStoryScene.ScenePath);
	}
}
