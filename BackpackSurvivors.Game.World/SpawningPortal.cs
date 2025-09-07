using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.System;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.World;

public class SpawningPortal : MonoBehaviour
{
	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private Collider2D _collider;

	[SerializeField]
	private SpriteRenderer _fadeOutAnimationSpriteRender;

	[SerializeField]
	private SpriteRenderer[] _infernalPillarSpriteRenderers;

	[SerializeField]
	private GameObject[] _infernalPillarCandles;

	[SerializeField]
	private CinemachineVirtualCamera _virtualCamera;

	private float _shakeTimer;

	[SerializeField]
	private AudioClip _infernoStoneAudioClip;

	[SerializeField]
	private TextMeshProUGUI _infernoTextMeshPro;

	[SerializeField]
	private Image _infernoTextBackdrop;

	private Dictionary<int, float> _animationLevelAndDuration;

	private void Awake()
	{
		SetAnimationsTimes();
		SpriteRenderer[] infernalPillarSpriteRenderers = _infernalPillarSpriteRenderers;
		for (int i = 0; i < infernalPillarSpriteRenderers.Length; i++)
		{
			_ = infernalPillarSpriteRenderers[i];
			_fadeOutAnimationSpriteRender.material.SetFloat("_FadeAmount", -0.1f);
		}
		_fadeOutAnimationSpriteRender.material.SetFloat("_Glow", 0f);
		_fadeOutAnimationSpriteRender.material.SetFloat("_FadeAmount", -0.1f);
		_fadeOutAnimationSpriteRender.material.SetFloat("_DistortAmount", 0f);
	}

	public void SetInfernoLevel(int infernoLevel)
	{
		if (infernoLevel > 0)
		{
			_infernoTextMeshPro.SetText($"Hellfire {infernoLevel}");
		}
		else
		{
			_infernoTextMeshPro.SetText(string.Empty);
		}
		_animator.SetInteger("Infernolevel", infernoLevel);
		PlayAudioPoints(infernoLevel);
	}

	private void PlayAudioPoints(int infernoLevel)
	{
		if (infernoLevel > 0)
		{
			LeanTween.value(_infernoTextMeshPro.gameObject, delegate(float val)
			{
				_infernoTextMeshPro.color = new Color(255f, 0f, 0f, val);
			}, 0f, 1f, 1f).setDelay(1f);
			LeanTween.value(_infernoTextBackdrop.gameObject, delegate(float val)
			{
				_infernoTextBackdrop.color = new Color(0f, 0f, 0f, val);
			}, 0f, 0.6f, 1f).setDelay(1f);
			LeanTween.value(_infernoTextMeshPro.gameObject, delegate(float val)
			{
				_infernoTextMeshPro.color = new Color(255f, 0f, 0f, val);
			}, 1f, 0f, 2f).setDelay(3f);
			LeanTween.value(_infernoTextBackdrop.gameObject, delegate(float val)
			{
				_infernoTextBackdrop.color = new Color(0f, 0f, 0f, val);
			}, 0.6f, 0f, 2f).setDelay(3f);
		}
		float num = 0f;
		if (infernoLevel > 0)
		{
			StartCoroutine(PlayDelayedSFX(0f));
		}
		if (infernoLevel > 3)
		{
			num += _animationLevelAndDuration[5];
			StartCoroutine(PlayDelayedSFX(num - 0.1f));
		}
		if (infernoLevel > 5)
		{
			num += _animationLevelAndDuration[7];
			StartCoroutine(PlayDelayedSFX(num - 0.2f));
		}
		if (infernoLevel > 7)
		{
			num += _animationLevelAndDuration[8];
			StartCoroutine(PlayDelayedSFX(num - 0.3f));
		}
		if (infernoLevel > 8)
		{
			num += _animationLevelAndDuration[9];
			StartCoroutine(PlayDelayedSFX(num - 0.4f));
		}
		StartCoroutine(StartFadeOut(num));
	}

	private IEnumerator StartFadeOut(float delayStack)
	{
		float fadeOutDuration = 5f;
		float num = 5f;
		yield return new WaitForSeconds(delayStack + num);
		_collider.enabled = false;
		GameObject[] infernalPillarCandles = _infernalPillarCandles;
		for (int i = 0; i < infernalPillarCandles.Length; i++)
		{
			infernalPillarCandles[i].SetActive(value: false);
		}
		LeanTween.value(base.gameObject, delegate(float val)
		{
			_fadeOutAnimationSpriteRender.material.SetFloat("_Glow", val);
		}, 0f, 3f, fadeOutDuration);
		LeanTween.value(base.gameObject, delegate(float val)
		{
			_fadeOutAnimationSpriteRender.material.SetFloat("_FadeAmount", val);
		}, -0.1f, 1f, fadeOutDuration);
		LeanTween.value(base.gameObject, delegate(float val)
		{
			_fadeOutAnimationSpriteRender.material.SetFloat("_DistortAmount", val);
		}, 0f, 2f, fadeOutDuration);
		SpriteRenderer[] infernalPillarSpriteRenderers = _infernalPillarSpriteRenderers;
		foreach (SpriteRenderer infernalPillarSpriteRenderers2 in infernalPillarSpriteRenderers)
		{
			LeanTween.value(base.gameObject, delegate(float val)
			{
				infernalPillarSpriteRenderers2.material.SetFloat("_FadeAmount", val);
			}, -0.1f, 1f, fadeOutDuration);
		}
		yield return new WaitForSeconds(fadeOutDuration);
		base.gameObject.SetActive(value: false);
		infernalPillarSpriteRenderers = _infernalPillarSpriteRenderers;
		for (int i = 0; i < infernalPillarSpriteRenderers.Length; i++)
		{
			infernalPillarSpriteRenderers[i].gameObject.SetActive(value: false);
		}
	}

	private IEnumerator PlayDelayedSFX(float delay)
	{
		StopShaking();
		yield return new WaitForSeconds(delay);
		SingletonController<AudioController>.Instance.PlaySFXClip(_infernoStoneAudioClip, 1f);
		ShakeCamera(1f, 1f);
	}

	private void ShakeCamera(float intensity, float time)
	{
		_virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = intensity;
		_shakeTimer = time;
	}

	public void StopShaking()
	{
		_virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
	}

	public float GetAnimationDuration(int infernoLevel)
	{
		return _animationLevelAndDuration.Where((KeyValuePair<int, float> x) => x.Key < infernoLevel).Sum((KeyValuePair<int, float> x) => x.Value);
	}

	internal void SetAnimationsTimes()
	{
		_animationLevelAndDuration = new Dictionary<int, float>();
		AnimationClip[] animationClips = _animator.runtimeAnimatorController.animationClips;
		foreach (AnimationClip animationClip in animationClips)
		{
			_animationLevelAndDuration.Add(Convert.ToInt32(animationClip.name), animationClip.length);
		}
	}
}
