using System.Collections;
using System.Linq;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.System.Video;
using BackpackSurvivors.UI.Stats;
using BackpackSurvivors.UI.Tooltip;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BackpackSurvivors.System;

[RequireComponent(typeof(CameraEnabler))]
public class AsyncManager : MonoBehaviour
{
	[SerializeField]
	private Image _background;

	[SerializeField]
	private GameObject _loadingComponents;

	[SerializeField]
	private Image _loadingBar;

	[SerializeField]
	private GameObject _loadingScreen;

	[SerializeField]
	private TextMeshProUGUI _loadingScreenText;

	[SerializeField]
	private GameObject[] _tips;

	private CameraEnabler _cameraEnabler;

	private void Awake()
	{
		_cameraEnabler = GetComponent<CameraEnabler>();
	}

	public void LoadScene(string scenePath, string name)
	{
		_cameraEnabler.SetCamerasEnabled(enabled: true);
		_loadingBar.fillAmount = 0f;
		DeactivateAllTips();
		ActivateRandomTip();
		StartCoroutine(LoadSceneAsync(scenePath));
		_loadingScreenText.SetText("Loading " + name);
	}

	private void ActivateRandomTip()
	{
		int randomRoll = RandomHelper.GetRandomRoll(_tips.Count());
		_tips[randomRoll].SetActive(value: true);
	}

	private void DeactivateAllTips()
	{
		GameObject[] tips = _tips;
		for (int i = 0; i < tips.Length; i++)
		{
			tips[i].SetActive(value: false);
		}
	}

	private IEnumerator TestTips()
	{
		DeactivateAllTips();
		_loadingComponents.SetActive(value: true);
		_loadingScreen.SetActive(value: true);
		LeanTween.value(_background.gameObject, delegate(float val)
		{
			_background.color = new Color(1f, 1f, 1f, val);
		}, 0f, 1f, 0.3f).setIgnoreTimeScale(useUnScaledTime: true);
		for (int i = 0; i < _tips.Length; i++)
		{
			if (i > 0)
			{
				_tips[i - 1].SetActive(value: false);
			}
			_tips[i].SetActive(value: true);
			yield return new WaitForSeconds(3f);
		}
	}

	private IEnumerator LoadSceneAsync(string scenePath)
	{
		_loadingComponents.SetActive(value: true);
		_loadingScreen.SetActive(value: true);
		LeanTween.value(_background.gameObject, delegate(float val)
		{
			_background.color = new Color(1f, 1f, 1f, val);
		}, 0f, 1f, 0.3f).setIgnoreTimeScale(useUnScaledTime: true);
		yield return new WaitForSecondsRealtime(0.3f);
		SingletonController<SceneChangeController>.Instance.AllowFading = false;
		ModalUiController modalUiController = Object.FindAnyObjectByType<ModalUiController>();
		if (modalUiController != null)
		{
			modalUiController.CloseAll();
		}
		AsyncOperation operation = SceneManager.LoadSceneAsync(scenePath);
		while (!operation.isDone)
		{
			float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
			yield return new WaitForSecondsRealtime(0.1f);
			_loadingBar.fillAmount = progressValue;
			yield return null;
		}
		yield return new WaitForSecondsRealtime(0.7f);
		LeanTween.value(_background.gameObject, delegate(float val)
		{
			_background.color = new Color(1f, 1f, 1f, val);
		}, 1f, 0f, 0.3f).setIgnoreTimeScale(useUnScaledTime: true);
		_loadingComponents.SetActive(value: false);
		yield return new WaitForSecondsRealtime(0.3f);
		_loadingScreen.SetActive(value: false);
		SingletonController<GameController>.Instance.ClearSceneChangeStates();
		SingletonController<TooltipController>.Instance.Hide(null);
		_cameraEnabler.SetCamerasEnabled(enabled: false);
	}
}
