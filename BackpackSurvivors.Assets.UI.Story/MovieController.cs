using System;
using System.Collections;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Shared;
using Tymski;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BackpackSurvivors.Assets.UI.Story;

internal class MovieController : MonoBehaviour
{
	[SerializeField]
	private MovieStep[] _movieSteps;

	[SerializeField]
	private SerializableDictionaryBase<int, AudioClip> _movieAudio;

	[SerializeField]
	private bool _playFromStart;

	[SerializeField]
	private int _startForStep;

	[SerializeField]
	private SceneReference _sceneAfterComplition;

	private void Start()
	{
		SingletonController<InputController>.Instance.OnCancelHandler += Instance_OnCancelHandler;
		if (_playFromStart)
		{
			StartCoroutine(PlayMovie());
		}
	}

	private void Instance_OnCancelHandler(object sender, EventArgs e)
	{
		if (base.isActiveAndEnabled)
		{
			StopCoroutine(PlayMovie());
			SingletonController<SceneChangeController>.Instance.ChangeScene(_sceneAfterComplition, LoadSceneMode.Single, isLevelScene: false, showLoadingScreen: true, "Fortress");
		}
	}

	private void PlayStep(int movieStepIndex)
	{
		_movieSteps[movieStepIndex].gameObject.SetActive(value: true);
		ImageAnimation component = _movieSteps[movieStepIndex].GetComponent<ImageAnimation>();
		if (component != null)
		{
			component.ResetToBeginning();
		}
		_movieSteps[movieStepIndex].Play();
	}

	public void DEBUG_PlayStep(int movieStepIndex)
	{
		MovieStep[] movieSteps = _movieSteps;
		for (int i = 0; i < movieSteps.Length; i++)
		{
			movieSteps[i].gameObject.SetActive(value: false);
		}
		PlayStep(movieStepIndex);
	}

	private IEnumerator PlayMovie()
	{
		Debug.Log("Starting movie");
		yield return new WaitForSeconds(0.1f);
		for (int movieStepIndex = _startForStep; movieStepIndex < _movieSteps.Length; movieStepIndex++)
		{
			MovieStep activeMovieStep = _movieSteps[movieStepIndex];
			float duration = activeMovieStep.Duration;
			Debug.Log($"Playing moviestep {movieStepIndex} for {duration}");
			PlayStep(movieStepIndex);
			if (_movieAudio.ContainsKey(movieStepIndex))
			{
				SingletonController<AudioController>.Instance.SetFadeOutTime(3f);
				SingletonController<AudioController>.Instance.SetFadeInTime(3f);
				SingletonController<AudioController>.Instance.PlayMusicClip(_movieAudio[movieStepIndex], 1f, loop: false);
			}
			yield return new WaitForSeconds(duration);
			activeMovieStep.Finish();
		}
		Debug.Log("Movie completed");
		SingletonController<SceneChangeController>.Instance.ChangeScene(_sceneAfterComplition, LoadSceneMode.Single, isLevelScene: false, showLoadingScreen: true, "Fortress");
	}

	private void OnDestroy()
	{
		SingletonController<InputController>.Instance.OnCancelHandler -= Instance_OnCancelHandler;
	}
}
