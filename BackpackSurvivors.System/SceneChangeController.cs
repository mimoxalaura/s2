using System;
using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.UI.Stats;
using Tymski;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BackpackSurvivors.System;

public class SceneChangeController : SingletonController<SceneChangeController>
{
	public delegate void LevelSceneLoadedHandler(object sender, LevelSceneLoadedEventArgs e);

	[SerializeField]
	private Image _fadeImage;

	[SerializeField]
	private float _fadeDuration;

	[SerializeField]
	private SceneReference _startReference;

	[SerializeField]
	private AsyncManager _asyncManager;

	private Enums.FadeStatus _currentFadeStatus = Enums.FadeStatus.None;

	private float _fadeTimer;

	private string _sceneToLoad;

	private LoadSceneMode _sceneModeToLoad;

	private bool _informLevelSceneIsLoaded;

	public bool AllowFading;

	private List<string> _openedScenes;

	public string CurrentSceneName { get; private set; }

	public event LevelSceneLoadedHandler OnLevelSceneLoaded;

	public event Action OnNewSceneLoading;

	private void Update()
	{
		FadeScene();
	}

	private void Start()
	{
		base.IsInitialized = true;
	}

	public bool WasOpened(string sceneName)
	{
		return _openedScenes.Contains(sceneName);
	}

	public override void AfterBaseAwake()
	{
		base.AfterBaseAwake();
		_openedScenes = new List<string>();
		SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
	}

	private void SceneManager_OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		_currentFadeStatus = Enums.FadeStatus.FadingIn;
		if (_informLevelSceneIsLoaded)
		{
			this.OnLevelSceneLoaded?.Invoke(this, new LevelSceneLoadedEventArgs(scene));
			_informLevelSceneIsLoaded = false;
		}
		SingletonController<GameController>.Instance.RefreshPlayer();
		ResetPreventOpeningSettings();
	}

	private void ResetPreventOpeningSettings()
	{
		GameMenuController gameMenuController = UnityEngine.Object.FindObjectOfType<GameMenuController>();
		if (!(gameMenuController == null))
		{
			gameMenuController.ResetPreventOpeningSettings();
		}
	}

	public void AddOpenedScene(string sceneName)
	{
		_openedScenes.Add(sceneName);
	}

	public void ChangeScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool isLevelScene = false, bool showLoadingScreen = false, string sceneVisualName = "")
	{
		if (!_openedScenes.Contains(sceneName))
		{
			_openedScenes.Add(sceneName);
		}
		if (showLoadingScreen)
		{
			this.OnNewSceneLoading?.Invoke();
			SingletonController<GameController>.Instance.RefreshPlayer();
			Time.timeScale = 1f;
			_sceneToLoad = sceneName;
			_sceneModeToLoad = mode;
			_currentFadeStatus = Enums.FadeStatus.FadingOut;
			_informLevelSceneIsLoaded = isLevelScene;
			_asyncManager.LoadScene(sceneName, sceneVisualName);
		}
		else
		{
			StartCoroutine(FadeOutAndLoadScene(sceneName, mode, isLevelScene));
		}
		CurrentSceneName = sceneName;
	}

	private IEnumerator FadeOutAndLoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool isLevelScene = false)
	{
		_currentFadeStatus = Enums.FadeStatus.FadingOut;
		_fadeTimer = 0f;
		this.OnNewSceneLoading?.Invoke();
		SingletonController<GameController>.Instance.RefreshPlayer();
		Time.timeScale = 1f;
		_sceneToLoad = sceneName;
		_sceneModeToLoad = mode;
		_currentFadeStatus = Enums.FadeStatus.FadingOut;
		_informLevelSceneIsLoaded = isLevelScene;
		yield return new WaitForSeconds(_fadeDuration);
	}

	private void FadeScene()
	{
		if (_currentFadeStatus == Enums.FadeStatus.None || !AllowFading)
		{
			return;
		}
		_fadeTimer += Time.unscaledDeltaTime;
		if (_fadeTimer > _fadeDuration)
		{
			_fadeTimer = 0f;
			if (_currentFadeStatus == Enums.FadeStatus.FadingOut)
			{
				SceneManager.LoadScene(_sceneToLoad, _sceneModeToLoad);
				_fadeImage.color = Color.black;
			}
			else
			{
				_fadeImage.color = Color.clear;
			}
			_currentFadeStatus = Enums.FadeStatus.None;
		}
		else
		{
			float num = 0f;
			num = ((_currentFadeStatus != Enums.FadeStatus.FadingOut) ? Mathf.Lerp(1f, 0f, _fadeTimer / _fadeDuration) : Mathf.Lerp(0f, 1f, _fadeTimer / _fadeDuration));
			_fadeImage.color = new Color(0f, 0f, 0f, num);
		}
	}

	public override void Clear()
	{
	}

	public override void ClearAdventure()
	{
	}
}
