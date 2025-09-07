using System.Collections;
using BackpackSurvivors.System;
using Tymski;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.MainMenu;

public class TitleDrop : MonoBehaviour
{
	[SerializeField]
	private Image _title;

	[SerializeField]
	private Image _shadow;

	[SerializeField]
	private Image _dust;

	[SerializeField]
	private Transform _spawnPosition;

	[SerializeField]
	private Transform _targetPosition;

	[SerializeField]
	private AudioClip _audioClipOnDrop;

	[SerializeField]
	private SceneReference _mainMenuScene;

	private void Start()
	{
		if (SingletonController<SceneChangeController>.Instance.WasOpened(_mainMenuScene.ScenePath))
		{
			_title.transform.localScale = new Vector3(1f, 1f, 1f);
			_title.color = new Color(255f, 255f, 255f, 255f);
			SingletonController<SceneChangeController>.Instance.AddOpenedScene(_mainMenuScene.ScenePath);
		}
		else
		{
			RunAnimation(2f);
		}
	}

	private void RunAnimation(float delay)
	{
		StartCoroutine(SpawnAsync(delay));
		StartCoroutine(SpawnShadowAsync(delay));
	}

	private IEnumerator SpawnAsync(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		float num = 0.8f;
		float num2 = 7f;
		_title.transform.localScale = new Vector3(num2, num2, num2);
		_title.color = new Color(255f, 255f, 255f, 0f);
		LeanTween.scale(_title.gameObject, new Vector3(1f, 1f, 1f), num).setEaseOutBounce().setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_title.gameObject, delegate(float val)
		{
			_title.color = new Color(255f, 255f, 255f, val);
		}, 0f, 1f, num / 2f).setIgnoreTimeScale(useUnScaledTime: true);
		SingletonController<AudioController>.Instance.PlaySFXClip(_audioClipOnDrop, 0.7f);
		yield return new WaitForSecondsRealtime(num);
		_dust.gameObject.SetActive(value: true);
	}

	private IEnumerator SpawnShadowAsync(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		float time = 0.8f;
		_shadow.transform.localScale = new Vector3(0f, 0f, 0f);
		LeanTween.scale(_shadow.gameObject, new Vector3(1f, 1f, 1f), time).setEaseOutBounce().setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_shadow.gameObject, delegate(float val)
		{
			_shadow.color = new Color(_shadow.color.r, _shadow.color.g, _shadow.color.b, val);
		}, 0f, 0.8f, time).setIgnoreTimeScale(useUnScaledTime: true);
	}
}
