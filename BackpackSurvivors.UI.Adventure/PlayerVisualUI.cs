using BackpackSurvivors.System;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.UI.Adventure;

public class PlayerVisualUI : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem _levelupParticleSystem;

	[SerializeField]
	private Animator _levelUpAnimator;

	[SerializeField]
	private SpriteRenderer _playerRenderer;

	[SerializeField]
	private Material _levelUpMat;

	[SerializeField]
	private AudioClip _levelAudioClip;

	private float _showAnimationTime = 0.5f;

	private float _hideAnimationTime = 0.3f;

	private float _delay = 0.5f;

	private string _shaderNameMetal = "_MetalFade";

	[Command("player.animation.levelup", Platform.AllPlatforms, MonoTargetType.Single)]
	public void LevelUpAnimation()
	{
		_levelupParticleSystem.gameObject.SetActive(value: true);
		_levelupParticleSystem.Play();
		_playerRenderer.material = _levelUpMat;
		_playerRenderer.material.SetFloat(_shaderNameMetal, 0f);
		LeanTween.cancel(_playerRenderer.gameObject);
		LeanTween.value(_playerRenderer.gameObject, UpdateMetalFade, 0f, 1f, _showAnimationTime).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_playerRenderer.gameObject, UpdateMetalFade, 1f, 0f, _hideAnimationTime).setDelay(_delay + _showAnimationTime).setIgnoreTimeScale(useUnScaledTime: true);
		SingletonController<AudioController>.Instance.PlaySFXClip(_levelAudioClip, 1f);
		_levelUpAnimator.SetTrigger("LevelUp");
	}

	private void UpdateMetalFade(float val)
	{
		_playerRenderer.material.SetFloat(_shaderNameMetal, val);
	}
}
